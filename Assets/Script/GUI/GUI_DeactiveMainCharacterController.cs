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
@file GUI_DeactiveMainCharacterController.cs
@author NDark

隨著GUI物件出現同時關閉主角的控制器

# 由GUI物件的顯示與否來決定
# 顯示時取得目前控制器設定，並關閉之
# 結束顯示時回復其設定。

@date 20130109 file started.
@date 20130113 . comment.
*/
using UnityEngine;

public class GUI_DeactiveMainCharacterController : MonoBehaviour 
{
	public enum ScaleInTimeState
	{
		UnActive = 0 ,
		Active ,
		DoingActive ,
		DeActive ,
	}
	private bool m_DefaultMainCharacterEnable = false ;
	private StateIndex m_State = new StateIndex() ;
	
	// Use this for initialization
	void Start () 
	{
		SetState( ScaleInTimeState.UnActive ) ;
	}
	
	void OnDestroy()
	{
		GlobalSingleton.ActiveMainCharacterController( m_DefaultMainCharacterEnable ) ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch( (ScaleInTimeState) m_State.state )
		{
		case ScaleInTimeState.UnActive :
			if( true == this.gameObject.guiTexture.enabled )
			{
				SetState( ScaleInTimeState.Active ) ;
			}			
			break ;
		case ScaleInTimeState.Active :
			// Debug.Log( "ScaleInTimeState.Active" + Time.realtimeSinceStartup ) ;
			m_DefaultMainCharacterEnable = GlobalSingleton.GetMainCharacterControllerEnbale() ;
			GlobalSingleton.ActiveMainCharacterController( false ) ;
			SetState( ScaleInTimeState.DoingActive ) ;
			break ;
		case ScaleInTimeState.DoingActive :
			if( false == this.gameObject.guiTexture.enabled )
			{
				SetState( ScaleInTimeState.DeActive ) ; 
			}
			break ;
		case ScaleInTimeState.DeActive :
			// Debug.Log( "ScaleInTimeState.DeActive" + Time.realtimeSinceStartup ) ;
			GlobalSingleton.ActiveMainCharacterController( m_DefaultMainCharacterEnable ) ;
			SetState( ScaleInTimeState.UnActive ) ;
			break ;			
		}
	}
	
	private void SetState( ScaleInTimeState _Set )
	{
		m_State.state = (int) _Set ;
	}
}
