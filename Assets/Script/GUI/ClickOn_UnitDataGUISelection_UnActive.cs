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
@file ClickOn_UnitDataGUISelection_UnActive.cs
@brief 點擊 GUI_UnitDataSelection_UnActive 的反應
@author NDark

# 點擊 GUI_UnitDataSelection_UnActive 的反應
# m_SelectName 紀錄目前點選到的部件名稱 並傳送給 主角的 UnitSelectionSystem
# m_ChildActive 取得Active的GUIObject
# 顯示m_ChildActive

@date 20121205 file started.
@date 20121205 
. add code of tell selection the m_SelectName
. add code of tell main character do not move by mouse click
@date 20121213 by NDark . 改成不鎖定 at OnMouseDown()
@date 20130111 by NDark . remove class member m_SelectLock

*/
using UnityEngine;

public class ClickOn_UnitDataGUISelection_UnActive : MonoBehaviour 
{
	public string m_SelectName = "" ;
	
	private GameObject m_ChildActive = null ;

	// Use this for initialization
	void Start () 
	{
		Transform trans = this.transform.FindChild( "GUI_UnitDataSelection_Active" ) ;
		if( null != trans )
			m_ChildActive = trans.gameObject ;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	void OnMouseDown()
	{
		
		// 顯示Active
		// set the component name
		GameObject mainChar = GlobalSingleton.GetMainCharacterObj() ;
		UnitSelectionSystem selectSys = mainChar.GetComponent<UnitSelectionSystem>() ;
		if( null != selectSys )
			selectSys.SpecifiedUnitComponent( m_SelectName ) ;
		
		MainCharacterController controller = mainChar.GetComponent<MainCharacterController>() ;
		if( null != controller )
			controller.SetClickOnNoMoveFuncThisFrame( true ) ;
		
		// 顯示Child
		if( null != m_ChildActive )
		{
			m_ChildActive.guiTexture.pixelInset = this.gameObject.guiTexture.pixelInset ;
			m_ChildActive.guiTexture.enabled = true ;
		}
		
	}
}
