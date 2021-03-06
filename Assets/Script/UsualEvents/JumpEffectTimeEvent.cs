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
@date JumpEffectTimeEvent.cs
@author NDark 

跳躍特效產生時間事件

# 目標地點
# 是否有漩渦

@date 20130104 . file created
@date 20130107 by NDark . add code of checking m_TargetObject at DoCloseOfEvent()

*/
using UnityEngine;
using System.Xml ;

public class JumpEffectTimeEvent : TimeEvent 
{
	NamedObject m_TargetObject = new NamedObject() ; // 目標物件名稱
	GameObject m_Vortex = null ;
	GameObject m_JumpFlash = null ;
	bool m_IsVortex = false ;
	/*
	<UsualEvent EventName="JumpEffectTimeEvent"
			TargetObjectName="BattleStarPegasusPosition" 
			Vortex="false"
			StartSec="1.0" 
			ElapsedSec="3.0"
			/>
	 */
	public override bool ParseXML( XmlNode _Node )
	{
		if( null == _Node.Attributes["TargetObjectName"] ||
			null == _Node.Attributes["StartSec"] ||
			null == _Node.Attributes["ElapsedSec"] )
		{
			return false ;
		}
	
		// Debug.Log( "GUITextureShowTimeEvent::ParseXML()" ) ;
		string objectName = _Node.Attributes["TargetObjectName"].Value ;
		string startSecStr = _Node.Attributes["StartSec"].Value ;
		string elapsedSecStr = _Node.Attributes["ElapsedSec"].Value ;
		
		bool IsVortex = false ;
		if( null != _Node.Attributes["Vortex"] )
		{
			string IsVortexStr = _Node.Attributes["Vortex"].Value ;
			IsVortex = IsVortexStr == "true" ? true : false ;
		}
		
		float startSec = 0.0f ;
		float.TryParse( startSecStr , out startSec ) ;
		
		float elapsedSec = 0.0f ;
		float.TryParse( elapsedSecStr , out elapsedSec ) ;
		
		this.Setup( startSec , 
					elapsedSec , 
					objectName ,
					IsVortex ) ;
		
		return true ;			
	}
	
	public void Setup( float _startTime , 
					   float _elapsedTime , 
					   string _TargetObjectName , 
					   bool _IsVortex )
	{
		m_Trigger.Setup( _startTime , _elapsedTime ) ;		
		
		m_TargetObject.Setup( _TargetObjectName , null ) ;
		m_IsVortex = _IsVortex ;
	}
	
	public JumpEffectTimeEvent()
	{
		
	}
	
	public JumpEffectTimeEvent( JumpEffectTimeEvent _src )
	{
		m_TargetObject.Setup( _src.m_TargetObject ) ;
	}	
		
	

	protected override void DoStartOfEvent()
	{
		// Debug.Log( "DoStartOfEvent()" ) ;
		if( true == m_IsVortex )
		{
			m_Vortex = PrefabInstantiate.CreateByInit( "Template_Effect_Vortex01" , 
				"Template_Effect_Vortex01" + ConstName.GenerateIterateString() , 
				m_TargetObject.Obj.transform.position , 
				m_TargetObject.Obj.transform.rotation ) ;
		}
	}	
	
	protected override void DoCloseOfEvent()
	{
		// Debug.Log( "DoEndOfEvent()" ) ;
		if( null != m_Vortex )
			GameObject.Destroy( m_Vortex ) ;
		
		if( null != m_TargetObject.Obj )
		{
			m_JumpFlash = PrefabInstantiate.CreateByInit( "Template_Effect_JumpFlash" , 
				"Template_Effect_JumpFlash" + ConstName.GenerateIterateString() , 
				m_TargetObject.Obj.transform.position , 
				m_TargetObject.Obj.transform.rotation ) ;
			
			Animation anim = m_JumpFlash.GetComponentInChildren<Animation>() ;
			if( null != anim )
				anim.Play() ;
		}
	}

	protected override void DoKeepClose()
	{
		if( null != m_JumpFlash 
			)
		{
			Animation anim = m_JumpFlash.GetComponentInChildren<Animation>() ;
			if( null != anim &&
				false == anim.isPlaying )
			{
				GameObject.Destroy( m_JumpFlash ) ;
				m_JumpFlash = null ;
			}
			
		}
		// Debug.Log( "DoKeepClose()" ) ;
	}		
}
