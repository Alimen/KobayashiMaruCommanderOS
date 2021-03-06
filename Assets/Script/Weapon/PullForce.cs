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
@file PullForce.cs
@brief 拉力
@author NDark

# 由牽引光束特效物件施放 掛載在受到牽引光束的單位上
# 參數 m_TargetUnit 被拉過去朝向的目標物件，施放者
# 參數 m_MinMaintainDistance 最小維持距離 小於此距離就不再拉
# 參數 m_MaxMaintainDistance 最大維持距離 用來計算拉力
# 參數 m_PullMaxSpeed 最大拉力速度
# Update()
## 計算出要被拉動的量 將其存到 m_ForceToMoveVec
# 呼叫 DestroyInSec() 後會有一定時間繼續拉動，然後自動消滅此拉力。


@date 20121122 . file started.
@date 20121122 by NDark . rename m_MaintainDistance to m_MinMaintainDistance
@date 20121123 by NDark 
. add class member m_MaxMaintainDistance
. add interpolate of speed at Update()
@date 20121204 by NDark . comment.
@date 20130109 by NDark . add class method DestroyInSec()
@date 20130112 by NDark 
. add class member m_State
. add class method KeepPull()
@date 20130119 by NDark
. remove InverseTransformDirection() at KeepPull()

*/
using UnityEngine;

[System.Serializable]
public class PullForce : MonoBehaviour 
{
	public enum PullState
	{
		UnActive ,
		Active ,
		WaitToDestroy ,
		Dead ,
	}
	
	private PullState m_State = PullState.UnActive ;
	
	private GameObject m_TargetUnit = null ;
	private float m_MinMaintainDistance = 0.0f ;
	private float m_MaxMaintainDistance = 0.0f ;
	private float m_PullMaxSpeed = 0.0f ;
	
	private CountDownTrigger m_DestroyTimer = new CountDownTrigger() ;
	
	public void Setup( GameObject _TargetUnit , 
					   float _MinMaintainDistance ,
					   float _MaxMaintainDistance ,
					   float _PullMaxSpeed )
	{
		m_TargetUnit = _TargetUnit ;
		m_MinMaintainDistance = _MinMaintainDistance ;
		m_MaxMaintainDistance = _MaxMaintainDistance ;
		m_PullMaxSpeed = _PullMaxSpeed ;
		m_State = PullState.Active ;
	}
	
	public void DestroyInSec( float _Sec )
	{
		m_DestroyTimer.Setup( _Sec ) ;
		m_DestroyTimer.Rewind() ;
		m_State = PullState.WaitToDestroy ;
	}
	
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch( m_State )
		{
		case PullState.UnActive :
			break ;
		case PullState.Active :
			KeepPull() ;
			break ;
		case PullState.WaitToDestroy :
			if( true == m_DestroyTimer.IsCountDownToZero() )
			{
				m_State = PullState.Dead ;
			}
			KeepPull() ;
			break ;
		case PullState.Dead :
			Component.Destroy( this ) ;
			break ;			
		}
	}
	
	private void KeepPull()
	{
		if( null == m_TargetUnit )
			return ;

		Vector3 ToTarget = m_TargetUnit.transform.position - this.gameObject.transform.position ;
		
		//Debug.Log( "ToTarget.magnitude" + ToTarget.magnitude ) ;
		if( ToTarget.magnitude > m_MinMaintainDistance )
		{
			float Speed = MathmaticFunc.Interpolate( 
				m_MinMaintainDistance , 0.0f ,
				m_MaxMaintainDistance , m_PullMaxSpeed ,
				ToTarget.magnitude ) ;
			// Debug.Log( "m_PullMaxSpeed" + m_PullMaxSpeed + " Speed" + Speed ) ;
			
			ToTarget.Normalize() ;
			ToTarget *= ( Speed * Time.deltaTime ) ;
			
			UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
			if( null != unitData )
			{
				unitData.m_ForceToMoveVec += ToTarget ;
			}
		}
	}
}
