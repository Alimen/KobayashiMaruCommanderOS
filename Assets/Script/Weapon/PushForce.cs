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
@file PushForce.cs
@brief 一定時間的推力
@author NDark

# 由 AI_CreateSelf 放在 新創造出來的物件上 
# 一定時間之後自我摧毀
# 參數 m_Force 推力
# 參數 m_ElapsedTimer 計時器要推多久

@date 20121125 file started.
@date 20121204 by NDark . comment.
@date 20130113 by NDark . comment.
@date 20130119 by NDark
. remove InverseTransformDirection() at Update()

*/
// #define DEBUG 

using UnityEngine;

public class PushForce : MonoBehaviour {
	
	private Vector3 m_Force = Vector3.zero ;
	private bool m_Active = false ;
	private CountDownTrigger m_ElapsedTimer = new CountDownTrigger() ;
	
	// Use this for initialization
	void Start () 
	{
	}
	
	public void SetupByTime( Vector3 _Force , float _ElapsedSec ) 
	{
		m_Force = _Force ;
#if DEBUG		
		Debug.Log( "SetupByTime() _Force=" + _Force ) ;
#endif		
		m_ElapsedTimer.Setup( _ElapsedSec ) ;
		m_ElapsedTimer.Rewind() ;
		m_Active = true ;
	}
	// Update is called once per frame
	void Update () 
	{
		if( false == m_Active )
			return ;
		
		if( true == m_ElapsedTimer.IsCountDownToZero() )
		{
			Component.Destroy( this ) ;
		}
		else
		{
			UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
			if( null != unitData )
			{
				Vector3 pushForceSelf = ( m_Force * Time.deltaTime ) ;
#if DEBUG		
				Debug.Log( "Update() pushForceSelf2=" + pushForceSelf.ToString() ) ;
#endif									
				unitData.m_ForceToMoveVec += ( pushForceSelf ) ;
			}
		}
	
	}
}
