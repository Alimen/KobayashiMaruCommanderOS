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
@file ControlPanelUnActive.cs
@brief 當滑鼠移入牌卡時開啟對應的功能面版(彩色)
@author NDark 

# 掛載在功能面版(灰)的牌卡上
# 當滑鼠移入牌卡時開啟對應的功能面版(彩色)
# 同時會呼叫 功能面版(彩色)上的 ControlPanelActive 重置狀態為初始化

@date 20121117 file started.
@date 20121204 by NDark 
. comment.
. add class method RetrieveChildActive()

*/
using UnityEngine;

public class ControlPanelUnActive : MonoBehaviour 
{
	public bool m_ActiveCalled = false ;// 控制只需要開啟一次即可
	public NamedObject m_ChildActive = new NamedObject() ;
	// Use this for initialization
	void Start () 
	{
		RetrieveChildActive() ;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	void OnMouseOver()
	{
		if( false == m_ActiveCalled )
		{
			// Debug.Log( "OnMouseOver" + this.gameObject.name ) ;
			if( null != m_ChildActive.Obj )
			{
				// Debug.Log( "ShowGUITexture.Show( m_ChildActive.Obj , true" ) ;
				ShowGUITexture.Show( m_ChildActive.Obj , true , false , false  ) ;
				
				ControlPanelActive active = m_ChildActive.Obj.GetComponent<ControlPanelActive>() ;
				if( null != active )
				{
					active.Setup() ;							
				}
			}

			m_ActiveCalled = true ;// 只觸發一次	
		}
	}
	
	void OnMouseExit()
	{
		// Debug.Log( "OnMouseExit" ) ;
		m_ActiveCalled = false ;
	}
	
	// initialize m_ChildActive at start
	private void RetrieveChildActive()
	{
		string thisName = this.gameObject.name ;
		string ActiveName = thisName.Replace( "_UnActive" , "_Active" ) ;
		Transform trans = this.gameObject.transform.FindChild( ActiveName ) ;
		if( null != trans )
		{
			m_ChildActive.Name = ActiveName ;
			m_ChildActive.Obj = trans.gameObject ;
			// Debug.Log( "m_ChildActive.Obj" + trans.gameObject ) ;
		}
	}
}
