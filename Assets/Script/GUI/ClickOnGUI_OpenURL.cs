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
@file ClickOnGUI_OpenURL.cs
@brief 點擊開啟指定URL
@author NDark

# 掛載在GUITexture物件上
# 會依據目前平台來決定開啟方式
# 如果是網頁平台就開啟新的視窗
# 如果是本機平台就直接開啟URL

@date 20121202 by NDark . add function to open a new web at web player mode
@date 20121204 by NDark . comment.

*/
using UnityEngine;

public class ClickOnGUI_OpenURL : MonoBehaviour {
	
	public string m_URL = "" ;// 需指定URL字串
	
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	void OnMouseDown()
	{
		// Debug.Log( Application.platform ) ;
		if( Application.platform == RuntimePlatform.WindowsWebPlayer )
		{
			string url = "window.open('" + m_URL + "','aNewWindow')" ;
			// Debug.Log( url ) ;
			Application.ExternalEval( url );					
		}
		else if( Application.platform == RuntimePlatform.WindowsPlayer ||
				 Application.platform == RuntimePlatform.WindowsEditor )
		{
			Application.OpenURL( m_URL ) ;		
		}
	}
}
