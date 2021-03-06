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
@date EnableBattleEventCameraManagerConditionEvent.cs
@author NDark 

啟動及設定戰場特寫管理器事件

# 啟動
# 戰場特寫的播放時間

@date 20130126 by NDark . file started.
@date 20130126 by NDark 
. add class member m_IsSetElapsedSec
. add class member m_ElapsedSec

*/
// #define DEBUG
using UnityEngine;
using System.Xml;

public class EnableBattleEventCameraManagerConditionEvent : ConditionEvent 
{
	private bool m_Enable = false ;
	private bool m_IsSetElapsedSec = false ;
	private float m_ElapsedSec = 0.0f ;
/*
	<UsualEvent EventName="EnableBattleEventCameraManagerConditionEvent"
			Enable="true" >		
		<Condition ConditionName="Condition_Time" 
			StartTime="8.0" />			
	</UsualEvent>		
*/
	public override bool ParseXML( XmlNode _Node )
	{
#if DEBUG				
		Debug.Log( "EnableBattleEventCameraManagerConditionEvent::ParseXML()" ) ;
#endif
		ParseForChildren( _Node ) ;
		
		if( null == _Node.Attributes["Enable"] )
		{
			return false ;
		}

		if( null != _Node.Attributes["ElapsedSec"] )
		{
			m_IsSetElapsedSec = true ;
			string ElapsedSecStr = _Node.Attributes["ElapsedSec"].Value ;
			float.TryParse( ElapsedSecStr , out m_ElapsedSec ) ;

		}		
		
		string enableStr = _Node.Attributes["Enable"].Value ;
		m_Enable = ( enableStr == "true" ) ? true : false ;
		return true ;
	}
	
	public EnableBattleEventCameraManagerConditionEvent()
	{
	}
	
	public EnableBattleEventCameraManagerConditionEvent( EnableBattleEventCameraManagerConditionEvent _src )
	{
		m_Enable = _src.m_Enable ;
	}
	
	public override void DoEvent()
	{
#if DEBUG				
		Debug.Log( "EnableBattleEventCameraManagerConditionEvent::DoEvent()" ) ;
#endif		
		EnableCameraBattleEventCameraManager() ;
	}		
	
	private void EnableCameraBattleEventCameraManager()
	{
		BattleEventCameraManager manager = GlobalSingleton.GetBattleEventCameraManager() ;
		if( null != manager )
		{
			if( false == m_Enable )
			{
				manager.Close() ;
			}
			else
			{
				if( true == m_IsSetElapsedSec )
				{
					BaseDefine.BATTLE_EVENT_CAMERA_ELAPSED_SEC = m_ElapsedSec ;
				}
			}
			
			manager.enabled = m_Enable ;
		}
	}	
	
}
