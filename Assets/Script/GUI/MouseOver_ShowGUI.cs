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
@file MouseOver_ShowGUI.cs
@brief 當滑鼠滑過時顯示指定物件 並且設定一定時間隱藏
@author NDark

目前掛載在 關卡2的牌卡上
目前客製化
控制物件是 GUI_MessageCard_KlingonStyleLink

@date 20121203 by NDark . add code to keep HideInSec visible at OnMouseOver()
@date 20121204 by NDark . comment.

*/
using UnityEngine;

public class MouseOver_ShowGUI : MonoBehaviour {
	
	public string m_GUIObjectName = "GUI_MessageCard_KlingonStyleLink" ;
	
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	void OnMouseOver()
	{
		// Debug.Log( "void OnMouseOver()") ;
		ShowGUITexture.Show( m_GUIObjectName , true , false , false ) ;				
	}
}
