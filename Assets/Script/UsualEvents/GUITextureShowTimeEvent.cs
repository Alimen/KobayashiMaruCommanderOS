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
@date GUITextureShowTimeEvent.cs
@author NDark 

GUITexture顯示時間事件

參數
# m_Trigger 觸發開始與結束的計時器
# m_TargetObjectName 目標物件名稱
# m_AudioClipName 發出聲音名稱
# m_EventManagerObj 事件處理器 的物件(用來呼叫audio source)


@date 20121221 . file created and copy from UsualEvent
@date 20121224 by NDark . remvoe useRealTime
*/
using UnityEngine;
using System.Collections;
using System.Xml;

public class GUITextureShowTimeEvent : TimeEvent 
{
	NamedObject m_TargetGUIObject = new NamedObject() ; // 目標物件名稱
	string m_AudioClipName = "" ; // 發出聲音名稱
	AudioClip m_Audio = null ;
	private GameObject m_EventManagerObj = null ; // 事件處理器 的物件(用來呼叫audio source)
	
	public override bool ParseXML( XmlNode _Node )
	{
		if( null == _Node.Attributes["objectName"] ||
			null == _Node.Attributes["startSec"] ||
			null == _Node.Attributes["elapsedSec"] )
		{
			return false ;
		}
	
		// Debug.Log( "GUITextureShowTimeEvent::ParseXML()" ) ;
		string objectName = _Node.Attributes["objectName"].Value ;
		string startSecStr = _Node.Attributes["startSec"].Value ;
		string elapsedSecStr = _Node.Attributes["elapsedSec"].Value ;
		string audioClipName = "" ;
		if( null != _Node.Attributes["audioClipName"] )
			audioClipName = _Node.Attributes["audioClipName"].Value ;
		
		float startSec = 0.0f ;
		float.TryParse( startSecStr , out startSec ) ;
		
		float elapsedSec = 0.0f ;
		float.TryParse( elapsedSecStr , out elapsedSec ) ;
		
		this.Setup( startSec , 
					elapsedSec , 
					objectName , 
					audioClipName ) ;
		
		return true ;			
	}
		
	public void Setup( float _startTime , 
					   float _elapsedTime , 
					   string _TargetObjectName ,
					   string _AudioClipName )
	{
		m_Trigger.Setup( _startTime , _elapsedTime ) ;		
		
		m_TargetGUIObject.Setup( _TargetObjectName , null ) ;
		m_AudioClipName = _AudioClipName ;
	}
		
	public GUITextureShowTimeEvent()
	{
		
	}
	
	public GUITextureShowTimeEvent( GUITextureShowTimeEvent _src )
	{
		m_TargetGUIObject.Setup( _src.m_TargetGUIObject.Name , null ) ;
		m_AudioClipName = _src.m_AudioClipName ;
		m_Audio = _src.m_Audio ;
	}
	
	protected override void DoStartOfEvent()
	{
		// Debug.Log( "DoStartOfEvent()" ) ;
		
		TryInitialize() ;

		PlayAudio( true ) ;
		
		
		EnableGUITexture( true ) ;			
		
	}	
	
	protected override void DoKeepActive()
	{
		if( null != m_TargetGUIObject.Obj &&
			false == m_TargetGUIObject.Obj.guiTexture.enabled )
		{
			// Debug.Log( "被強制關閉了()" ) ;
			// 被強制關閉了
			DoCloseOfEvent() ;
			m_Trigger.Close() ;
		}
	}	
	
	protected override void DoCloseOfEvent()
	{
		// Debug.Log( "DoEndOfEvent()" ) ;
		
		TryInitialize() ;
		
		
		PlayAudio( false ) ;
		
		EnableGUITexture( false ) ;
		
	}	
	
	protected void PlayAudio( bool _Play )
	{
		if( null == m_EventManagerObj )
			return ;
		
		if( true == _Play )
		{
			if( null != m_Audio )
				m_EventManagerObj.audio.PlayOneShot( m_Audio ) ;			
		}
		else 
		{
			m_EventManagerObj.audio.Stop() ;
		}
	}
	
	protected void EnableGUITexture( bool _Enable )
	{
		if( null != m_TargetGUIObject.Obj )
			ShowGUITexture.Show( m_TargetGUIObject.Obj , _Enable , true , true ) ;
	}
	
	private void TryInitialize()
	{
		if( null == m_EventManagerObj )
		{
			m_EventManagerObj = GlobalSingleton.GetUsualEventManagerObject() ;
		}
		
		if( 0 != m_AudioClipName.Length &&
			null == m_Audio )
		{
			m_Audio = ResourceLoad.LoadAudio( m_AudioClipName ) ;
		}
	}
}
