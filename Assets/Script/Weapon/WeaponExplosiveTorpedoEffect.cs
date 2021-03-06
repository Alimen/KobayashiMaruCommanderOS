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
@file WeaponExplosiveTorpedoEffect.cs
@brief 爆炸型魚雷效果
@author NDark

爆炸型魚雷效果

# 繼承 WeaponPhotonTorpedoEffect
# m_DetectRange 偵測距離 
# 覆寫 CheckHit() 碰撞時不直接造成傷害，而是產生一個衝擊波 Template_Effect_Weapon_Shockwave01 計算傷害。

@date 20121112 file created.
*/
using UnityEngine;

public class WeaponExplosiveTorpedoEffect : WeaponPhotonTorpedoEffect
{
	public float m_DetectRange = 15.0f ;
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
	
	protected override void CheckHit( Collision _collision )
	{
		if( null == m_WeaponDataShared )
			return ;
		string collisionComponentName = _collision.collider.name ;
		if( -1 != collisionComponentName.IndexOf( m_WeaponDataShared.UnitObjectName ) )
		{
			// do not collide with self
			return ;
		}
		// Debug.Log( "collision=" + _collision.collider.name ) ;
		
		GameObject shockwaveObject = 
			PrefabInstantiate.CreateByInit( "Template_Effect_Weapon_Shockwave01" , 
			this.gameObject.name + ":" + "Template_Effect_Weapon_Shockwave01" , 
			this.gameObject.transform.position ,
			this.gameObject.transform.rotation ) ;
		
		if( null != shockwaveObject )
		{
			ShockwaveEffect shockwaveEffect = shockwaveObject.GetComponent<ShockwaveEffect>() ;			
			shockwaveEffect.Active( m_WeaponDataShared.m_CauseDamage ,
				0.7f , 30 , 
				m_DetectRange , 
				"Flashbang" , 
				2.5f , 
				m_WeaponDataShared.UnitGameObject ) ;
		}
		
		CloseFireAnimation() ;
	}
}
