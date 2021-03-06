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
@file ShockwaveEffect.cs
@author NDark

衝擊波特效

# 造成傷害及動畫播放的衝擊波特效
# 狀態計時器
## 啟動
## 動畫結束
# 等待聲音播完計時器
# 等待動畫播完計時器
# 紀錄被波及的單位與部件 unitComponentList
# m_DamageValue 傷害值
# m_ScaleNow 目前縮放值
# m_MaxScaleValue 最大縮放值
# m_DetectRange 偵測距離
# 開始時間，用來計算縮放值
# m_TimeDestinationTime 結束時間，用來計算縮放值
# m_Audio 發出的音效
# m_AttackerDisplayName 發出震波的攻擊者
# SetScale() 設定縮放值
# CalculateHit() 啟動時就計算碰撞及傷害


@date 20121228 by NDark
@date 20121229 by NDark 
. add class UnitComponentPair
. add class member m_DetectRange
. add class member m_AttackerUnit
. add damage string at CalculateHit()
@date 20130126 by NDark . add class member m_AttackerDisplayName
@date 20130205 by NDark . comment.

*/
using UnityEngine;
using System.Collections.Generic;

// 紀錄被波及的單位與部件
[System.Serializable]
public class UnitComponentPair
{
	public string UnitName = "" ;
	public string ComponentName = "" ;
}

public class ShockwaveEffect : MonoBehaviour 
{
	BasicTrigger m_AliveTrigger = new BasicTrigger() ;
	CountDownTrigger m_WaitSoundTimer = new CountDownTrigger() ;
	CountDownTrigger m_AnimationTimer = new CountDownTrigger() ;
	float m_DamageValue = 0.0f ;
	float m_ScaleNow = 0 ;
	float m_MaxScaleValue = 30 ;
	float m_DetectRange = 15 ;
	float m_TimeStart = 0.0f ;
	float m_TimeDestinationTime = 0.0f ;	
	AudioClip m_Audio = null ;
	string m_AttackerName = "" ;
	string m_AttackerDisplayName = "" ;
	
	public void Active( float _DamageValue ,
						float _AnimationTime ,
						float _MaxScaleValue , 
						float _DetectRange ,
						string _AudioName , 
						float _AudioPlayTime ,
						GameObject _AttackerObject )
	{
		m_DamageValue = _DamageValue ;
		m_MaxScaleValue = _MaxScaleValue ;
		m_DetectRange = _DetectRange ;
		
		m_AnimationTimer.Setup( _AnimationTime ) ;
		m_AnimationTimer.Rewind() ;
		
		if( 0 != _AudioName.Length )
		{
			m_Audio = ResourceLoad.LoadAudio( _AudioName ) ;
			this.gameObject.audio.PlayOneShot( m_Audio ) ;	
			m_WaitSoundTimer.Setup( _AudioPlayTime ) ;
			m_WaitSoundTimer.Rewind() ;
		}
		
		m_AliveTrigger.Active() ;
		
		m_TimeStart = Time.time ;
		m_TimeDestinationTime = m_TimeStart + m_AnimationTimer.m_CountDownTime ;
		m_AttackerDisplayName = m_AttackerName = _AttackerObject.name ;
		UnitData attackerUnitData = _AttackerObject.GetComponent<UnitData>() ;
		if( null != attackerUnitData )
			m_AttackerDisplayName = attackerUnitData.GetUnitDisplayName() ;
		
		
		CalculateHit() ;
	}
	
	// Use this for initialization
	void Start () 
	{
		SetScale( m_ScaleNow ) ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( m_AliveTrigger.IsActive() )
		{
			if( true == m_AnimationTimer.IsCountDownToZero() )
			{
				// end				
				Renderer [] renderers = this.gameObject.GetComponentsInChildren<Renderer>() ;
				foreach( Renderer renderer in renderers )
				{
					renderer.enabled = false ;
				}
				m_AliveTrigger.Close() ;
			}
			else
			{
				m_ScaleNow = MathmaticFunc.Interpolate(
					m_TimeStart , 0 ,
					m_TimeDestinationTime , m_MaxScaleValue ,
					Time.time ) ;
				// Debug.Log( m_ScaleNow ) ;
				SetScale( m_ScaleNow ) ;
			}
		}
		else if( m_AliveTrigger.IsClosed() &&
				 true == m_WaitSoundTimer.IsCountDownToZero() )
		{
			GameObject.Destroy( this.gameObject ) ;
		}
	
	}
	
	private void SetScale( float _ScaleValue )
	{
		Vector3 scaleNow = this.gameObject.transform.localScale ;
		scaleNow.x = _ScaleValue ;
		scaleNow.z = _ScaleValue ;
		this.gameObject.transform.localScale = scaleNow ;
	}
	
	private void CalculateHit()
	{
		MainUpdate mainUpdate = GlobalSingleton.GetMainUpdateComponent() ;
		if( null == mainUpdate )
			return ;

		Dictionary<string , UnitComponentPair> unitComponentList = new Dictionary<string , UnitComponentPair>() ;
		Collider[] colliders = Physics.OverlapSphere( this.gameObject.transform.position , m_DetectRange ) ;
		foreach( Collider collider in colliders )
		{
			if( collider.name == this.gameObject.name )
				continue ;
			
			string key = "" ;
			string UnitName = "" ;
			string ComponentName = "" ;
			// Debug.Log( " collider.name=" + collider.name ) ;
			string [] strVec = collider.name.Split( ':' ) ;
			if( strVec.Length == 2 )
			{
				UnitName = strVec[ 0 ] ;
				ComponentName = strVec[ 1 ] ;
				key = collider.name ;
			}
			else if( collider.name == "CollideCube" )
			{
				if( null != collider.transform.parent )
				{
					UnitName = collider.transform.parent.gameObject.name ;
					ComponentName = ConstName.UnitDataComponentUnitIntagraty ;
					key = UnitName + ":" + ComponentName ;
				}
			}

			if( 0 == UnitName.Length ||
				UnitName == this.gameObject.name ||
				UnitName == m_AttackerName )
				continue ;	
			
			if( false == unitComponentList.ContainsKey( key ) )
			{
				UnitComponentPair newpair = new UnitComponentPair() ;
				newpair.UnitName = UnitName ;
				newpair.ComponentName = ComponentName ;						
				unitComponentList.Add( key , newpair ) ;
			}			
		}
		
		foreach( string key in unitComponentList.Keys )
		{
			UnitComponentPair pair = unitComponentList[ key ] ;
			// Debug.Log( " UnitName=" + pair.UnitName + " ComponentName=" + pair.ComponentName ) ;
			GameObject unitObj = mainUpdate.GetUnit( pair.UnitName ) ;
			if( null == unitObj )
				continue ;
			UnitDamageSystem dmgSys = unitObj.GetComponent<UnitDamageSystem>() ;
			if( null == dmgSys )
				continue ;
			dmgSys.ActiveDamageNumberEffectNextTime( true , 5 ) ;
			dmgSys.CauseDamageValueOut( m_AttackerName , 
										m_AttackerDisplayName ,
										m_DamageValue , 
										key ) ;	
			
		}
	}
}
