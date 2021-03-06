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
@file PlayAnimationCollisionEvent.cs
@author NDark

第九關黑洞動畫的管理器

# 黑洞播放完畢進入真正的第九關

@date 20130104 file started.
*/
using UnityEngine;
using System.Collections;

public class CheckLevel9BlackHoleManager : MonoBehaviour 
{
	TimeTrigger m_Active = new TimeTrigger() ;
	public NamedObject m_AnimationObject = new NamedObject( "Unit_BlackHole001" ) ;
	public string m_ChildName = "Dummy" ;
	public string m_AnimationName = "BlackHole01" ;
	public string m_LevelName = "Level09_Singularity_1" ;
		
	// Use this for initialization
	void Start () 
	{
		m_Active.Initialize() ;
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_Active.m_State.Update() ;
		switch( m_Active.State )
		{
		case TriggerState.UnActive :
			if( null != m_AnimationObject.Obj )
			{
				Transform trans = m_AnimationObject.Obj.transform.FindChild( m_ChildName ) ;
				if( null != trans )
				{
					Animation anim = trans.gameObject.GetComponent<Animation>() ;
					if( null != anim )
					{
						if( true == anim.IsPlaying( m_AnimationName ) )
						{
							// Debug.Log( "true == anim.IsPlaying" ) ;
							MessageQueueManager messageManager = GlobalSingleton.GetMessageQueueManager() ;
							if( null != messageManager )
							{
								string message = StrsManager.Get( 1201 ) ;
								messageManager.AddMessage( message ) ;
							}
							MainCharacterController controller = GlobalSingleton.GetMainCharacterControllerComponent() ;
							if( null != controller )
								controller.enabled = false ;
							m_Active.Active() ;
						}
						
					}
				}
			}				
			break ;
		case TriggerState.Active :
			if( m_Active.m_State.ElapsedFromLast() > 3 )
			{
				// Debug.Log( "Application.LoadLevel" ) ;
				GlobalSingleton.m_LevelString = m_LevelName ;
				Application.LoadLevel( "Scene_BattleLevel" ) ;
				m_Active.Close() ;
			}
			break ;
		case TriggerState.Closed :

			break ;			
		}
	
	}
}
