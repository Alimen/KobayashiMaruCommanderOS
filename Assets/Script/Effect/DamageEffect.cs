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
@file DamageEffect.cs
@brief 在 UnitDamageSystem 更新的各種傷害特效
@author NDark 
 
# IsDestroyAtEnd 是否要在結束時摧毀(不可重複使用)
# m_State 目前狀態
# m_CountDownTrigger 計時器
# m_EffectObj 特效物件
# EffectName() 特效物件的名稱 用來識別這個傷害特效
# Start() 啟動(需要外部關閉)
# StartByTime() 一定時間啟動
# Stop() 關閉
# EnableRenderer() 啟動/關閉特效的顯示
	
@date 20121111 file created.
@date 20121115 by NDark . add virtual to class method Update()
@date 20121203 by NDark . comment
@date 20121218 by NDark . comment
@date 20130205 by NDark . comment.

*/
using UnityEngine;

/* 傷害更新狀態 */
[System.Serializable]
public enum DamageState
{
	NonActive ,			// 未啟動
	Active ,			// 啟動中
	ActiveByTime ,		// 一定時間啟動
}



[System.Serializable]
public class DamageEffect
{
	public bool IsDestroyAtEnd = false ;// 是否要在結束時摧毀(不可重複使用)
	public DamageState m_State = DamageState.NonActive ;// 目前狀態
	public CountDownTrigger m_CountDownTrigger = new CountDownTrigger() ;// 計時器
	public NamedObject m_EffectObj = new NamedObject() ;// 特效物件
	
	public virtual void Update()
	{
		switch( m_State )
		{
		case DamageState.NonActive :
			break ;
		case DamageState.Active :
			break ;
		case DamageState.ActiveByTime :
			if( true == m_CountDownTrigger.IsCountDownToZero() )
			{
				Stop() ;
			}
			break ;
		}
	}
	
	// return the name of effect object
	public string EffectName()
	{
		return m_EffectObj.Name ;
	}
	
	// 啟動(需要外部關閉)
	public void Start()
	{
		EnableRenderer( true ) ;
		m_State = DamageState.Active ;
	}
	
	// 一定時間啟動
	public void StartByTime( float _ElapsetedTime )
	{
		Start() ;
		m_State = DamageState.ActiveByTime ;
		m_CountDownTrigger.Setup( _ElapsetedTime ) ;
		m_CountDownTrigger.Rewind() ;
	}

	// 關閉
	public void Stop()
	{
		m_State = DamageState.NonActive ;
		if( true == IsDestroyAtEnd )
		{
			if( null != m_EffectObj.GetObj() )
				GameObject.Destroy( m_EffectObj.Obj ) ;
			
			m_EffectObj.Name = "" ;
		}
		else
		{
			// do not release just turn it off
			EnableRenderer( false ) ;
		}
	}
	
	// enable/disable renderer of all effect object
	private void EnableRenderer( bool _Enable )
	{
		if( null != m_EffectObj.GetObj() )
		{
			Renderer[] renderers = m_EffectObj.Obj.GetComponentsInChildren<Renderer>() ;
			foreach( Renderer renderer in renderers )
			{
				renderer.enabled = _Enable ;
			}
		}
	}	
}
