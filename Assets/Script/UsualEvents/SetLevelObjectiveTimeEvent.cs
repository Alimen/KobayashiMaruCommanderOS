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
@date SetLevelObjectiveTimeEvent.cs
@author NDark 
 
設定勝利失敗目標圖示的時間事件

# 勝利與失敗的目標圖示並非一開始就顯示，可能到一定時間顯示
# 當玩家按掉關閉目標說明時才顯示
# objectName 要設定的勝利失敗目標GUI物件
# switchObjectName 切換按鈕名稱 

@date 20121221 . file created and copy from UsualEvent
@date 20121224 by NDark . remvoe useRealTime
*/
using UnityEngine;
using System.Xml;

public class SetLevelObjectiveTimeEvent : TimeEvent 
{
	NamedObject m_LevelObjectiveGUIObject = new NamedObject() ;// 顯示的牌卡物件
	NamedObject m_LevelObjectiveButtonObject = new NamedObject() ;// 顯示的按紐物件
	
	public void Setup( float _startTime ,
					   float _elapsedTime ,
					   string _LevelObjectiveGUIObjName,
					   string _LevelObjectiveButtonObjName )
	{
		m_Trigger.Setup( _startTime , _elapsedTime ) ;
		
		
		m_LevelObjectiveGUIObject.Name = _LevelObjectiveGUIObjName ;
		m_LevelObjectiveButtonObject.Name = _LevelObjectiveButtonObjName ;
	}	
	
	public override bool ParseXML( XmlNode _Node )
	{
		if( null == _Node.Attributes["objectName"] ||
			null == _Node.Attributes["switchObjectName"] ||
			null == _Node.Attributes["startSec"] ||
			null == _Node.Attributes["elapsedSec"] )
		{
			return false ;
		}
		// Debug.Log( "SetLevelObjectiveEvent::ParseXML()" ) ;

		string objectName = _Node.Attributes["objectName"].Value ;
		string switchObjectName = _Node.Attributes["switchObjectName"].Value ;
		string startSecStr = _Node.Attributes["startSec"].Value ;
		string elapsedSecStr = _Node.Attributes["elapsedSec"].Value ;
		
		float startSec = 0.0f ;
		float.TryParse( startSecStr , out startSec ) ;
		float elapsedSec = 0.0f ;
		float.TryParse( elapsedSecStr , out elapsedSec ) ;
		
		this.Setup( startSec , 
					elapsedSec , 
					objectName , 
					switchObjectName ) ;
		return true ;		
	}
	
	public SetLevelObjectiveTimeEvent()
	{
		
	}
	
	public SetLevelObjectiveTimeEvent( SetLevelObjectiveTimeEvent _src )
	{
		m_LevelObjectiveGUIObject.Name = _src.m_LevelObjectiveGUIObject.Name ;
		m_LevelObjectiveButtonObject.Name = _src.m_LevelObjectiveButtonObject.Name ;
		
	}
	
	protected override void DoStartOfEvent()
	{
		// Debug.Log( "DoStartOfEvent()" ) ;
		if( null != m_LevelObjectiveButtonObject.Obj )
		{
			ClickOnGUI_SwitchGUIObject switchGUI = m_LevelObjectiveButtonObject.Obj.GetComponent<ClickOnGUI_SwitchGUIObject>() ;
			if( null != switchGUI )
			{
				switchGUI.Setup( m_LevelObjectiveGUIObject.Name ) ;
			}
		}
	}	
	
	protected override void DoKeepActive()
	{
		if( null != m_LevelObjectiveGUIObject.Obj )
		{
			GUITexture guiTexture = m_LevelObjectiveGUIObject.Obj.GetComponent<GUITexture>() ;
			if( null != guiTexture &&
				false == guiTexture.enabled )
			{
				// Debug.Log( "被強制關閉了()" ) ;
				DoCloseOfEvent() ;
				m_Trigger.Close() ;
			}
		}
	}	
	
	protected override void DoCloseOfEvent()
	{
		// Debug.Log( "DoEndOfEvent()" ) ;
		
		ShowTheButton() ;
	}	
	
	private void ShowTheButton()
	{
		ShowGUITexture.Show( m_LevelObjectiveButtonObject.Obj , true , false , false ) ;
	}	
}
