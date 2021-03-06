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
@file BattleEventCameraManager.cs
@author NDark

# 戰場特寫管理器
# 當敵人產生時 EnemyGenerator::TryActiveBattleEventCamera() 會呼叫此管理器
# 控制攝影機 m_BattleEventCamera
# 設定其跟隨器 m_CameraFollower
# Setup() 啟動展場特寫，需要傳入跟隨物件，以及索引
# 索引是用來決定戰場特寫的位置在上下左右四個座標
# SetupByTime() 啟動後一定時間關閉
# CheckTargetIsValid() 檢察目標是否存活，目標消失就馬上關閉
# 除了上下左右四個座標之外，戰場特寫還有上下左右四個箭頭
# Close() 
## 關閉箭頭
## 關閉Follower
## 關閉攝影機的顯示
## 設定狀態為未啟動

@date 20130120 file started.
@date 20130121 by NDark . add checking of CameraFollowMainCharacter at Setup()
@date 20130126 by NDark . add enable checking at Setup()
@date 20130205 by NDark . comment.

*/
using UnityEngine;
using System.Collections.Generic;

public class BattleEventCameraManager : MonoBehaviour 
{
	public enum ActiveState
	{
		UnActive , 
		Active ,
		ActiveByTime ,
	}
	
	public Rect [] m_ReferenceRects = { 
		new Rect( 0.43f , 0.7f , 0.15f , 0.195f ) ,
		new Rect( 0.43f , 0.18f , 0.15f , 0.195f ) ,
		new Rect( 0.04f , 0.4f , 0.15f , 0.195f ) ,
		new Rect( 0.8f , 0.35f , 0.15f , 0.195f )
	} ;
	
	public NamedObject m_Arrows = new NamedObject( "GUI_BattleEventArrows" ) ;
	public NamedObject [] m_ArrowObjs = new NamedObject[4] ;
	
	private NamedObject m_FollowUnit = new NamedObject() ;
	private ActiveState m_State = ActiveState.UnActive ;
	private Camera m_BattleEventCamera = null ;
	private CameraFollowUnit m_CameraFollower = null ;
	private CountDownTrigger m_Timer = new CountDownTrigger() ;
	
	public bool IsActive()
	{
		return m_State != ActiveState.UnActive ;
	}
	
	public void SetupByTime( NamedObject _FollowUnit , 
					   		 int _ReferenceRectIndex , 
							 float _ElapsedSec )
	{
		if( false == Setup( _FollowUnit , _ReferenceRectIndex ) )
			return ;
		m_Timer.Setup( _ElapsedSec ) ;
		m_Timer.Rewind() ;
		m_State = ActiveState.ActiveByTime;
	}
	
	public bool Setup( NamedObject _FollowUnit , 
					   int _ReferenceRectIndex )
	{
		if( false == this.enabled )
			return false ;
		
		// 檢查CameraFollower是否政在運作中
		CameraFollowMainCharacter follower = GlobalSingleton.GetCameraFollowMainCharacter() ;
		if( null != follower &&
			false == follower.enabled )
			return false ;
		
		if( null == m_BattleEventCamera || 
			null == m_CameraFollower ||
			_ReferenceRectIndex < 0 || 
			_ReferenceRectIndex >= 4 )
			return false ;
		
		// Debug.Log( "_ReferenceRectIndex=" + _ReferenceRectIndex ) ;
		
		m_BattleEventCamera.rect = m_ReferenceRects[ _ReferenceRectIndex ] ;
		ShowGUITexture.Show( m_Arrows.Obj , false , false , true ) ;// hide all arrows 
		ShowGUITexture.Show( m_ArrowObjs[ _ReferenceRectIndex ].Obj , true , false , false ) ;// show only arrow
		
		m_CameraFollower.Setup( _FollowUnit ) ;
		m_FollowUnit.Setup( _FollowUnit ) ;
		
		m_BattleEventCamera.enabled = true ;
		m_State = ActiveState.Active ;
		return true ;
	}
	
	public void Close()
	{
		if( null == m_BattleEventCamera ||
			null == m_CameraFollower )
			return ;
		
		ShowGUITexture.Show( m_Arrows.Obj , false , false , true ) ;// hide all arrows 
		
		m_CameraFollower.Close() ;		
		
		m_BattleEventCamera.enabled = false ;
		m_State = ActiveState.UnActive ;
	}	
	
	// Use this for initialization
	void Start () 
	{
		m_BattleEventCamera = GlobalSingleton.GetBattleEventCamera() ;
		m_CameraFollower = GlobalSingleton.GetBattleEventCameraFollower() ;
		
		if( null != m_Arrows.Obj )
		{
			for( int i = 0 ; i < 4 ; ++i )
			{
				string key = string.Format( "GUI_BattleEventArrow{0}" , i ) ;
				Transform trans = m_Arrows.Obj.transform.FindChild( key ) ;
				if( null != trans )
				{
					m_ArrowObjs[ i ].Setup( trans.gameObject ) ;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch( m_State )
		{
		case ActiveState.UnActive :
			break ;
		case ActiveState.Active :
			if( false == CheckTargetIsValid() )
			{
				Close() ;
			}
			break ;
		case ActiveState.ActiveByTime :
			if( false == CheckTargetIsValid() ||
				true == m_Timer.IsCountDownToZero() )
			{
				Close() ;
			}
			break ;			
		}
	}
	
	private bool CheckTargetIsValid()
	{
		return ( null != m_FollowUnit.Obj ) ;			
	}
}
