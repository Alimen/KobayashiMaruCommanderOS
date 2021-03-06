/*
IMPORTANT: READ BEFORE DOWNLOADING, COPYING, INSTALLING OR USING. 

By downloading, copying, installing or using the software you agree to this license.
If you do not agree to this license, do not download, install, copy or use the software.

    License Agreement For Kobayashi Maru Commander Open Source

Copyright (C) 2013, Chih-Jen Teng(NDark) and Koguyue Entertainment, 
all rights reserved. Third party copyrights are property of their respective owners. 

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

  * Redistribution's of source code must retain the above copyright notice,
    this list of conditions and the following disclaimer.

  * Redistribution's in binary form must reproduce the above copyright notice,
    this list of conditions and the following disclaimer in the documentation
    and/or other materials provided with the distribution.

  * The name of Koguyue or all authors may not be used to endorse or promote products
    derived from this software without specific prior written permission.

This software is provided by the copyright holders and contributors "as is" and
any express or implied warranties, including, but not limited to, the implied
warranties of merchantability and fitness for a particular purpose are disclaimed.
In no event shall the Koguyue or all authors or contributors be liable for any direct,
indirect, incidental, special, exemplary, or consequential damages
(including, but not limited to, procurement of substitute goods or services;
loss of use, data, or profits; or business interruption) however caused
and on any theory of liability, whether in contract, strict liability,
or tort (including negligence or otherwise) arising in any way out of
the use of this software, even if advised of the possibility of such damage.  
*/
/*
@file CameraFollowMainCharacter.cs
@brief 攝影機跟隨主角  
@author NDark
 
# 掛在Camera上
# 會去取得 CameraInitialization 的距離原點參數 來作為持續保持 與玩家位置的距離
# UpdateCameraPosition() 
依照感測器的目前比例與上下限計算出縮放的比例，然後以標準的距離作縮放得到距離玩家的長度
由玩家的位置反推為攝影機的位置，最後傳給　CameraShakeEffect
# RetrieveSensorEffect() 取回感測器的目前功效，
再以變數 m_SensorTotalEffectStandard 標準感測器數值做參考，計算得出一個比例，
當單位感測器數值有變化時，就是攝影機要調整的距離值變化值。
請參考 STANDARD_SENSOR_TOTAL_EFFECT
# m_MinScaleDistanceFromCharacter 與 m_MaxScaleDistanceFromCharacter 是攝影機縮放的上下限。
自感測器比例0.0到2.0。
請參考 MIN_SCALE_FROM_STANDARD_DISTANCE 與 MAX_SCALE_FROM_STANDARD_DISTANCE。

@date 20121108 by NDark . refine code.
@date 20121203 by NDark . rename class member DistanceFromObject to m_DistanceFromObject.
@date 20121207 by NDark 
. add class member m_SensorTotalEffectStandard
. rename class member m_DistanceVecFromObject to m_DistanceVecFromObjectStandard
. add class method RetrieveSensorEffect()
@date 20121218 by NDark . comment.
@date 20121227 by NDark 
. add sensorEffectRatio checking at Update()
. add class member m_MinScaleDistanceFromCharacter
. add class member m_MaxScaleDistanceFromCharacter
. add class method UpdateCameraPosition()
@date 201030112 by NDark . refactor and comment.

*/
// #define DEBUG
using UnityEngine;

public class CameraFollowMainCharacter : MonoBehaviour 
{
	// 標準感測器數值
	private const float m_SensorTotalEffectStandard = BaseDefine.STANDARD_SENSOR_TOTAL_EFFECT ;
	
	// 是攝影機縮放的上下限
	private const float m_MinScaleDistanceFromCharacter = BaseDefine.MIN_SCALE_FROM_STANDARD_DISTANCE_CAMERA ;
	private const float m_MaxScaleDistanceFromCharacter = BaseDefine.MAX_SCALE_FROM_STANDARD_DISTANCE_CAMERA ;	
	
	// 與玩家的標準距離,取自 CameraInitialization
	private Vector3 m_DistanceVecFromObjectStandard = Vector3.zero ;
	
	private NamedObject m_MainCharacter = new NamedObject() ;
	
	// Use this for initialization
	void Start () 
	{
		CameraInitialization camInitScript = this.gameObject.GetComponent<CameraInitialization>() ;
		if( null != camInitScript )
		{
			m_DistanceVecFromObjectStandard = camInitScript.InitializationCameraPosition ;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		RetrieveMainCharacter() ;
		
		if( null == m_MainCharacter.GetObj() )
		{
			// Debug.Log( "null == MainCharacterObj" ) ;
			return ;
		}
	
		UpdateCameraPosition();	
	}
	
	/*
	 依照感測器的目前比例與上下限計算出縮放的比例，然後以標準的距離作縮放得到距離玩家的長度
	 由玩家的位置反推為攝影機的位置，最後傳給　CameraShakeEffect
	 */
	private void UpdateCameraPosition()
	{
		float sensorEffectRatio = 1.0f ;// 0.0-2.0 ?
		RetrieveSensorEffect( m_MainCharacter.Obj , ref sensorEffectRatio ) ;
		
		Vector3 OrgPos = m_MainCharacter.Obj.transform.position ;
		
		float resultScaleInDistance = MathmaticFunc.Interpolate( 0.0f , m_MinScaleDistanceFromCharacter ,
														  2.0f , m_MaxScaleDistanceFromCharacter ,
														  sensorEffectRatio ) ;
		if( resultScaleInDistance > m_MaxScaleDistanceFromCharacter )
			resultScaleInDistance = m_MaxScaleDistanceFromCharacter ;
		else if( resultScaleInDistance < m_MinScaleDistanceFromCharacter )
			resultScaleInDistance = m_MinScaleDistanceFromCharacter ;
		
#if DEBUG		
		Debug.Log( "CameraFollowMainCharacter::UpdateCameraPosition() resultScaleInDistance=" + resultScaleInDistance ) ;
#endif		
		Vector3 distanceFromObject = m_DistanceVecFromObjectStandard * resultScaleInDistance ;
		
		Vector3 CamPosItShouldBe = OrgPos + distanceFromObject ; 
		
		// set camera position by shake effect
		CameraShakeEffect shakeEffectObj = this.gameObject.GetComponent<CameraShakeEffect>() ;
		if( null == shakeEffectObj )
			this.gameObject.transform.position = CamPosItShouldBe ;
		else
			shakeEffectObj.SetCmaeraPos( CamPosItShouldBe ) ;
	}
	
	// 取回感測器的目前功效
	private void RetrieveSensorEffect( GameObject _MainCharacter , ref float _EffectRatio )
	{
		if( null == _MainCharacter )
			return ;
		UnitData unitData = _MainCharacter.GetComponent<UnitData>() ;
		if( null == unitData )
			return ;
		UnitComponentData sensor = unitData.GetSensorComponent()  ;
		if( null == sensor )
			return ;
		
		_EffectRatio = sensor.TotalEffect() / m_SensorTotalEffectStandard ;	
	}
	
	private void RetrieveMainCharacter()
	{
		if( null == m_MainCharacter.GetObj() )
		{
			m_MainCharacter.Obj = GlobalSingleton.GetMainCharacterObj() ;
			// Debug.Log( m_MainCharacter.Name ) ;
		}
	}
}
