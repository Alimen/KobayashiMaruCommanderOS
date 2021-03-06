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
@file WeaponPhotonTorpedoEffect.cs
@brief 光雷特效
@author NDark
 

# 繼承自 WeaponCannonEffect
# m_MoveSpeed 移動速度
# m_RotateSpeed 自轉速度
# Update()
## 更新位置及自轉
## 檢查關閉
# Hit() 擊中單位造成特效及傷害
關閉特效物件
關閉武器發射狀態
# OnCollisionEnter()
檢查碰撞並且處發擊中單位
# UpdateTargetDirection() 持續修正目標方向（追蹤），
# 目前修正角度 m_RotateTargetDirectionAngle 一秒30度


@date 20121112 file created.
@date 20121112 by NDark 
. add null assign at Start()
. implement OnCollisionEnter()
. add class method CheckHitUnit()
. add class method Hit()
@date 20121114 by NDark . remove class method CheckHitUnit()
@date 20121204 by NDark . comment.
@date 20121219 by NDark . refactor.
@date 20130109 by NDark . add class member m_RotateTargetDirectionAngle
@date 20130130 by NDark 
. remove init of m_MoveSpeed at Start()
. remove init of m_RotateSpeed at Start()

*/
using UnityEngine;

public class WeaponPhotonTorpedoEffect : WeaponCannonEffect 
{
	public float m_RotateSpeed = 1.0f ;
	protected float m_RotateTargetDirectionAngle = 30.0f ;
	
	// Use this for initialization
	void Start () 
	{
		this.ClearWeaponDataShared() ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( null != m_WeaponDataShared )
		{
			UpdateTargetDirection() ;
			UpdatePosition() ;
			CheckIsOutOfScreen() ;
			this.CheckRendererDisable() ;
		}
	}
	
	void OnCollisionEnter( Collision collision )  
	{
		this.CheckHit( collision ) ;
	}
	
	protected virtual void UpdateTargetDirection()
	{
		
		if( null != m_WeaponDataShared &&
			null != m_WeaponDataShared.TargetUnitObject )
		{
			Vector3 toTarget = m_WeaponDataShared.TargetUnitObject.transform.position - this.gameObject.transform.position ;
			toTarget.Normalize() ;
			float Angle = Vector3.Angle( m_WeaponDataShared.m_TargetDirection , toTarget ) ;
			if( Angle > 1 )
			{
				Vector3 Up = Vector3.Cross( m_WeaponDataShared.m_TargetDirection , toTarget ) ;
				Quaternion rotate = Quaternion.identity ;
				rotate = Quaternion.AngleAxis( m_RotateTargetDirectionAngle * Time.deltaTime  , Up ) ;
				m_WeaponDataShared.m_TargetDirection = rotate * m_WeaponDataShared.m_TargetDirection ;					
			}
			// m_WeaponDataShared.m_TargetDirection
		}
	}
	
	protected override void UpdatePosition()
	{
		Vector3 ToTarget = m_WeaponDataShared.m_TargetDirection * ( m_MoveSpeed * Time.deltaTime ) ;	
		this.gameObject.transform.position = this.gameObject.transform.position + ToTarget ;
		this.gameObject.transform.RotateAroundLocal( this.gameObject.transform.up , 
													 m_RotateSpeed * Time.deltaTime ) ;

	}
	

}
