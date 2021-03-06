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
@file ObjectShakeEffect.cs
@brief 物件震動效果
@author NDark

# 目前是裝載在防護罩特效物件上
# 發動時會記錄原本座標,關閉時會設定回去
# 物件隱藏時自動關閉
# 目前移動量大小是0.1，請參考 BaseDefine.OBJECT_SHAKE_DISTANCE

@date 20121111 file created.
@date 20121204 by NDark . comment.
@date 20121218 by NDark 
. add class member m_ScaleInRandomMove
. replace code by MathmaticFunc.RandomVector() at Update()

*/
using UnityEngine;

public class ObjectShakeEffect : MonoBehaviour {
	
	Vector3 m_OrgPos = Vector3.zero ;
	bool m_Active = false ;
	float m_ScaleInRandomMove = BaseDefine.OBJECT_SHAKE_DISTANCE ;
	
	// Use this for initialization
	void Start () {
	
	}
	
	public void Active( bool _Active )
	{
		m_Active = _Active ;
		if( true == _Active )
		{
			m_OrgPos = this.gameObject.transform.localPosition ;
		}
		else
		{
			this.gameObject.transform.localPosition = m_OrgPos ;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( true == m_Active )
		{
			Vector3 position = MathmaticFunc.RandomVector( 2 ) ;
			this.transform.Translate( position * m_ScaleInRandomMove ) ;			
			
			Renderer renderer = this.gameObject.GetComponentInChildren<Renderer>() ;
			if( false == renderer.enabled )
			{
				Active( false ) ;
			}
		}
	
	}
}
