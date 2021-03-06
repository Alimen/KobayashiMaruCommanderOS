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
@file SensorDamper.cs
@author NDark

# 掛載在單位上的感測阻尼
# 參數
## m_DamperSpeed 阻尼降低的速度
## m_ClearSpeed 阻尼還原的速度
## m_DamperMinimum 阻尼最低會降到多少
## m_AffectTimer 阻尼還原的計時時間
# DampTheSensorEffect() 修改單位的感測器數值
# ClearSensorDamper() 會去取得感測器並且調整目前功率到指定數值
# 會檢查計時器，計時結束就把功率還原，並且摧毀自己。
# 狀態
# AddMessage() 結束時會發出訊息


@date 20121207 file started.
@date 20121207 by NDark . modify sec of m_AffectTimer
@date 20121210 by NDark . change class member m_DamperMinimum to public
@date 20121225 by NDark . add class method AddMessage()
@date 20130204 by NDark . rename class method CheckSensorDamper() to DampTheSensorEffect()

*/
using UnityEngine;

public class SensorDamper : MonoBehaviour 
{
	public enum SensorDamperState
	{
		UnActive = 0 ,
		Damping ,
		ClearEffect ,
		End ,
	}
	
	public CountDownTrigger m_AffectTimer = new CountDownTrigger( 1.0f ) ;
	SensorDamperState m_State = SensorDamperState.UnActive ;
	float m_DamperSpeed = 40.0f ;
	float m_ClearSpeed = 120.0f ;
	public float m_DamperMinimum = 0.5f ;
	
	// Use this for initialization
	void Start () 
	{
		m_AffectTimer.Rewind() ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch( m_State )
		{
		case SensorDamperState.UnActive :
			m_State = SensorDamperState.Damping ;
			break ;
		case SensorDamperState.Damping :

			DampTheSensorEffect() ;
			
			if( true == m_AffectTimer.IsCountDownToZero() )
			{
				m_State = SensorDamperState.ClearEffect ;
			}						
			break ;
		case SensorDamperState.ClearEffect :
			ClearSensorDamper() ;
			break ;
		case SensorDamperState.End :
			if( this.gameObject.name == ConstName.MainCharacterObjectName )
				AddMessage() ;					
			Destroy( this ) ;
			break ;
		}
	}
	
	void DampTheSensorEffect()
	{
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( null == unitData )
			return ;
	    UnitComponentData sensor = unitData.GetSensorComponent() ;
		if( null == sensor )
			return ;
		if( sensor.m_Effect.now > sensor.m_Effect.max * m_DamperMinimum ) 
		{
			sensor.m_Effect.now -= ( m_DamperSpeed * Time.deltaTime ) ;
		}
	}
	
	private void ClearSensorDamper()
	{
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( null == unitData )
			return ;
	    UnitComponentData sensor = unitData.GetSensorComponent() ;
		if( null == sensor )
			return ;
		if( sensor.m_Effect.now < sensor.m_Effect.max ) 
		{
			sensor.m_Effect.now += ( m_ClearSpeed * Time.deltaTime ) ;
		}
		else
		{
			m_State = SensorDamperState.End ;
		}
	}

	// 結束時會發出訊息
	private void AddMessage()
	{
		MessageQueueManager manager = GlobalSingleton.GetMessageQueueManager() ;
		if( null != manager )
		{
			string message = StrsManager.Get( 1102 ) ;
			manager.AddMessage( message ) ;
		}
	}	
}
