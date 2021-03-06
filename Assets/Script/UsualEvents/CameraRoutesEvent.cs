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
@file CameraRoutesEvent.cs
@author NDark

攝影機運作事件

# 全部攝影機路線運作計時器
# 一次的路線經過計時器
# 啟動時是否要關閉Camera
# 啟動時是否要關閉MiniMap
# 啟動時間
# 路線截點
# 目前走到哪個路線節點

@date 20121224 file started.
@date 20121224 by NDark 
. remove class member m_JudgeDistance
. add code of DetectGUIObject at UpdateCamera()
@date 20121225 by NDark 
. add class method EnableMiniMap()
. add class member m_IsDeactiveMiniMap
. rename class method CheckSkip() to CheckMoveSkip()
. add class method CheckWaitSkip()
@date 20130119 by NDark
. add class member m_OriginalMiniMapEnable
. add class method RevertMiniMapEnable()
@date 20130121 by NDark . add class method CloseBattleEventCamera()

*/
using UnityEngine;
using System.Collections.Generic ;
using System.Xml;

public class CameraRoutesEvent : UsualEvent 
{
	TimeTrigger m_WholeRoutesTrigger = new TimeTrigger() ;
	TimeTrigger m_EachRouteTrigger = new TimeTrigger() ;
	
	bool m_IsDeactiveCameraFollower = false ;
	int m_DeactiveNameMax = 10 ;
	List<string> m_CameraFollowerNames = new List<string>() ;
	
	bool m_IsDeactiveMiniMap = false ;
	private bool m_OriginalMiniMapEnable = false ;
	
	float m_StartSec = 0.0f ;
	
	int m_RouteIndexNow = 0 ;
	List<PosRoute> m_Routes = new List<PosRoute>() ;
	
	
	/*
	<UsualEvent EventName="CameraRoutesEvent"
			StartSec="3.0" 

			IsDeactiveCameraFollower="true"
			CameraFollowerName0="CameraFollowMainCharacter" 
			CameraFollowerName1="CameraShakeEffect" 
			
			IsDeactiveMiniMap="true"
			
			JudgeDistance="10.0" >
		
		<PosRoute Destination="MainCharacter" 
				MoveTime="0.0" 
				WaitTime="0.1" 
				MoveDetectGUIObject="MessageCard_ObjectiveLevel06" 
				WaitDetectGUIObject="MessageCard_ObjectiveLevel06" 
				/>
		
	
	</UsualEvent>	
	
	*/	
	public override bool ParseXML( XmlNode _Node )
	{

		
		m_RouteIndexNow = 0 ;
		m_Routes.Clear() ;		
		if( true == _Node.HasChildNodes )
		{
			PosRoute addPosRoute = null ;
			for( int i = 0 ; i < _Node.ChildNodes.Count ; ++i )
			{
				if( "PosRoute" == _Node.ChildNodes[ i ].Name )
				{
					if( true == XMLParsePosRoute.Parse( _Node.ChildNodes[ i ] , out addPosRoute ) )
					{
						// Debug.Log( "m_Routes.Add" ) ;
						m_Routes.Add( addPosRoute ) ;
					}
				}
			}
		}

		
		if( null != _Node.Attributes["StartSec"] )
		{
			string StartSecStr = _Node.Attributes["StartSec"].Value ;
			float.TryParse( StartSecStr , out m_StartSec ) ;
		}
		m_WholeRoutesTrigger.Setup( m_StartSec , 0 ) ;
		
		
		
		if( null != _Node.Attributes["IsDeactiveCameraFollower"] )
		{
			string IsDeactiveCameraFollowerStr = _Node.Attributes["IsDeactiveCameraFollower"].Value ;
			m_IsDeactiveCameraFollower = ( "true" == IsDeactiveCameraFollowerStr ) ? true : false ;			
		}
		
		if( null != _Node.Attributes["IsDeactiveMiniMap"] )
		{
			string IsDeactiveMiniMapStr = _Node.Attributes["IsDeactiveMiniMap"].Value ;
			m_IsDeactiveMiniMap = ( "true" == IsDeactiveMiniMapStr ) ? true : false ;			
		}
		
		for( int i = 0 ; i < m_DeactiveNameMax ; ++i )
		{
			string format = string.Format( "CameraFollowerName{0}" , i ) ;
			if( null == _Node.Attributes[format] )
				break ;
			string Name = _Node.Attributes[format].Value ;
			
			m_CameraFollowerNames.Add( Name ) ;
		}
				

		return true ;
	}
	
	public override void Update()
	{
		if( m_WholeRoutesTrigger.IsAboutToStart( true ) )
		{
			ActiveCameraFollower( false ) ;
			EnableMiniMap( false ) ;
			CloseBattleEventCamera() ;
			RewindEachRouteTimer() ;
		}		
		else if( m_WholeRoutesTrigger.IsActive() )
		{
			UpdateCamera() ;
		}		
			
	}
	
	private void ActiveCameraFollower( bool _enable )
	{
		if( false == m_IsDeactiveCameraFollower )
			return ;
		
		foreach( string script in m_CameraFollowerNames )
		{
			MonoBehaviour follower = (MonoBehaviour) Camera.mainCamera.GetComponent( script ) ;
			if( null != follower )
				follower.enabled = _enable ;
		}
	}
	
	private void EnableMiniMap( bool _enable )
	{
		if( false == m_IsDeactiveMiniMap )
			return ;
		
		ClickOnGUI_SwitchMiniMap switcher = GlobalSingleton.GetSwitchMiniMap() ;
		
		if( null != switcher )
		{
			m_OriginalMiniMapEnable = switcher.GetMiniMapEnable() ;
			switcher.EnableMiniMap( _enable ) ;
		}
	}
	
	private void CloseBattleEventCamera()
	{
		// 檢查CameraFollower是否政在運作中
		CameraFollowMainCharacter follower = GlobalSingleton.GetCameraFollowMainCharacter() ;
		if( null != follower &&
			false == follower.enabled )
		{
			BattleEventCameraManager manager = GlobalSingleton.GetBattleEventCameraManager() ;
			if( null != manager )
			{
				manager.Close() ;
			}
		}
	}
	
	private void RevertMiniMapEnable()
	{
		if( false == m_IsDeactiveMiniMap )
			return ;
		
		ClickOnGUI_SwitchMiniMap switcher = GlobalSingleton.GetSwitchMiniMap() ;
		if( null != switcher )
			switcher.EnableMiniMap( m_OriginalMiniMapEnable ) ;
	}		
	
	private void UpdateCamera()
	{
		if( m_RouteIndexNow >= m_Routes.Count )
		{
			// stop 
			// Debug.Log( "stop" ) ;
			ActiveCameraFollower( true ) ;
			RevertMiniMapEnable() ;
			m_WholeRoutesTrigger.Close() ;
		}
		else
		{
			
			Vector3 destination = m_Routes[ m_RouteIndexNow ].m_Destination.GetPosition() ;
			Vector3 disVec = destination - Camera.mainCamera.transform.position ;
			disVec.y = 0.0f ;
			// Debug.Log( "disVec.magnitude" + disVec.magnitude ) ;
			// Debug.Log( "m_EachRouteTrigger.m_State.ElapsedFromLast" + m_EachRouteTrigger.m_State.ElapsedFromLast() ) ;
			
			if( m_EachRouteTrigger.IsAboutToStart( true ) )
			{
				// start to wait
			}
			else if( m_EachRouteTrigger.IsAboutToClose( true ) )
			{
				// Debug.Log( "++m_RouteIndexNow" ) ;
				++m_RouteIndexNow ;
				RewindEachRouteTimer() ;

				
			}
			else if( m_EachRouteTrigger.IsReady() )
			{
				if( true == CheckMoveSkip() )
				{
					// Debug.Log( "CheckMoveSkip()" ) ;
					// detect shutdown
					++m_RouteIndexNow ;
					RewindEachRouteTimer() ;
					return ;
				}
				
				// update pos
				// 0 now estimation
				float remainTime = m_EachRouteTrigger.m_StartTime - m_EachRouteTrigger.m_State.ElapsedFromLast() ;
				float speed = MathmaticFunc.Interpolate( 
					0 , 0 ,
					remainTime , disVec.magnitude ,
					Time.deltaTime ) ;
				// float speed = 30.0f ;
				disVec.Normalize() ;
				disVec *= ( speed ) ;
				// Debug.Log( "disVec" + disVec ) ;
				Camera.mainCamera.transform.Translate( disVec , Space.World ) ;					
			}						
			else if( m_EachRouteTrigger.IsActive() )
			{
				// wait
				if( true == CheckWaitSkip() )
				{
					// Debug.Log( "CheckWaitSkip()" ) ;
					// detect shutdown
					++m_RouteIndexNow ;
					RewindEachRouteTimer() ;
					return ;
				}
				
				// keep follow the destination
				if( 0 != destination.magnitude )
				{
					Camera.mainCamera.transform.Translate( disVec , Space.World ) ;					
				}
			}				
	
		}
	}
	
	private void RewindEachRouteTimer()
	{
		if( m_RouteIndexNow < m_Routes.Count )
		{
			m_EachRouteTrigger.Setup( m_Routes[ m_RouteIndexNow ].m_MoveTime , 
									  m_Routes[ m_RouteIndexNow ].m_WaitTime ) ;
		}
	}
	
	private bool CheckMoveSkip()
	{
		NamedObject detectGUIObject = m_Routes[ m_RouteIndexNow ].m_MoveDetectGUIObject ;
		// Debug.Log( "CheckMoveSkip() " + detectGUIObject.Name ) ;
		return ( null != detectGUIObject &&
		 		 0 != detectGUIObject.Name.Length &&
				 null != detectGUIObject.Obj &&
				 null != detectGUIObject.Obj.GetComponent<GUITexture>() &&
				 false == detectGUIObject.Obj.guiTexture.enabled ) ;
				
	}
	private bool CheckWaitSkip()
	{
		NamedObject detectGUIObject = m_Routes[ m_RouteIndexNow ].m_WaitDetectGUIObject ;
		// Debug.Log( "CheckWaitSkip() " + detectGUIObject.Name ) ;
		return ( null != detectGUIObject &&
		 		 0 != detectGUIObject.Name.Length &&
				 null != detectGUIObject.Obj &&
				 null != detectGUIObject.Obj.GetComponent<GUITexture>() &&
				 false == detectGUIObject.Obj.guiTexture.enabled ) ;
				
	}	
}
