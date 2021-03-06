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
@file ShiftEffect.cs
@brief 推動特效
@author NDark
@date 20121116 file started.

# 繼承 DamageEffect
# 作用在目標物件上 m_TargetObject
# 需要一個作用方向 m_ShiftVec
# 會累積在 m_ForceToMoveVec
# 被推動物件的質量會影響最終的推動長度
# m_PushSpeedBase 基本推動量 質量為1時的物件的每秒推動距離

@date 20121129 by NDark . add m_Mass
@date 20121204 by NDark . add class member m_PushSpeedBase, 調整質量1的基本推動量.

*/
using UnityEngine;

[System.Serializable]
public class ShiftEffect : DamageEffect
{
	public GameObject m_TargetObject = null ; // 作用的目標物件
	public Vector3 m_ShiftVec = Vector3.zero ; // 作用的向量
	
	private float m_Mass = 1.0f ; // 質量	
	private float m_PushSpeedBase = BaseDefine.STANDARD_PUSH_DISTANCE ; // 針對質量1.0的基本推動長度
	
	public override void Update()
	{
		if( null == m_TargetObject )
			return ;
		
		UnitData unitData = null ;
		unitData = m_TargetObject.GetComponent<UnitData>() ;
		if( null == unitData )
			return ;
		
		RetrieveMass() ;
		
		float pushspeed = m_PushSpeedBase / m_Mass ;
		switch( m_State )
		{
		case DamageState.NonActive :
			break ;
		case DamageState.Active :
			Push( unitData , m_ShiftVec , pushspeed ) ;
			break ;
		case DamageState.ActiveByTime :
			Push( unitData , m_ShiftVec , pushspeed ) ;
			
			if( true == m_CountDownTrigger.IsCountDownToZero() )
			{
				Stop() ;
			}
			break ;
		}
	}
	
	private void Push( UnitData _TargetUnitData , Vector3 _PushVec , float _PushSpeed )
	{
		if( null != _TargetUnitData )
		{
			// Debug.Log( "ShiftEffect::Push() _PushVec=" + _PushVec.ToString() ) ;
			_TargetUnitData.m_ForceToMoveVec += ( _PushVec * _PushSpeed * Time.deltaTime ) ;
		}
	}
	
	private void RetrieveMass()
	{
		if( null != m_TargetObject )
		{
			Rigidbody rbody = m_TargetObject.GetComponentInChildren<Rigidbody>() ;
			if( null != rbody )
				m_Mass = rbody.mass ;
		}
	}
}
