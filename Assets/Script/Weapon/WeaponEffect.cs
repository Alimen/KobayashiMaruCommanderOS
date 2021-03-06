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
@file WeaponEffect.cs
@brief 武器特效
@author NDark

# 基本的武器特效類別
# 被別人所繼承
# 掛載在各項武器的 Effect 3D Object 上
# 使用 m_WeaponDataShared 來存取各項需要物件
# 檢查物件是否被關閉，同時清除武器集合
# 必須以 Setup 設定之
# 透過此script來運作各項武器特效
# 更新位置方向
# 判定碰撞及傷害

@date 20121119 file started.
@date 20130107 by NDark . change class method ClearWeaponDataShared() to virtual.
@date 20130206 by NDark . comment.

*/
using UnityEngine;

public class WeaponEffect : MonoBehaviour 
{
	public WeaponDataSet m_WeaponDataShared = null ;
	
	public virtual void Setup( WeaponDataSet _WeaponData )
	{
		m_WeaponDataShared = _WeaponData ;
	}
	
	// Use this for initialization
	void Start () 
	{
		ClearWeaponDataShared() ;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	protected void CheckRendererDisable()
	{
		Renderer renderer = this.gameObject.GetComponentInChildren<Renderer>() ;
		if( null != renderer )
		{
			if( false == renderer.enabled )
				ClearWeaponDataShared() ;
		}	
	}
	
	protected virtual void ClearWeaponDataShared()
	{
		m_WeaponDataShared = null ;
	}
}
