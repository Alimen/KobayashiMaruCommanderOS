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
@file GUI_ControlPanelDetectZone.cs
@author NDark

控制面板的顯現

(目前沒有使用)

用一個偵測區域來做
不能用mouse over (因為會被蓋住)
注意進入與離開的範圍不同

@date 20121215 file started.
@date 20121219 by NDark . comment.
@date 20121220 by NDark . deactive code.

*/

using UnityEngine;

#if ENABLE

public class GUI_ControlPanelDetectZone : MonoBehaviour 
{
	[System.Serializable]
	public enum ControlPanelDetectZoneState
	{
		UnActive = 0 ,
		Active ,		
	}
	
	ControlPanelDetectZoneState m_State = ControlPanelDetectZoneState.UnActive ;
	
	public bool m_IsEverActiveControlPanel = false ;// 是用來檢查教學是否啟動過control panel 只會觸發一次
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		MainCharacterController controller = GlobalSingleton.GetMainCharacterControllerComponent() ;
		GUITexture guiTexture = this.gameObject.GetComponent<GUITexture>() ;
		if( null == controller ||
			null == guiTexture )
			return ;
		Rect deteczone = guiTexture.pixelInset ;
		
		switch( m_State )
		{
		case ControlPanelDetectZoneState.UnActive :
			
			if( Input.mousePosition.x > deteczone.x &&
				Input.mousePosition.y > deteczone.y &&
				Input.mousePosition.x < deteczone.x + deteczone.width &&
				Input.mousePosition.y < deteczone.y + deteczone.height / 2 )
			{
				CheckTutorialEverActiveControlPanel() ;
				
				// each time active control panel, canel function
				if( null != controller )
				{
					controller.CancelControlMode() ;
				}
						
				// controller.ShowControlPanel( true ) ;
				m_State = ControlPanelDetectZoneState.Active ;
			}

			break ;
		case ControlPanelDetectZoneState.Active :
			
			if( Input.mousePosition.x > deteczone.x &&
				Input.mousePosition.y > deteczone.y &&
				Input.mousePosition.x < deteczone.x + deteczone.width &&
				Input.mousePosition.y < deteczone.y + deteczone.height )
			{
			}
			else
			{
				// controller.ShowControlPanel( false ) ;
				m_State = ControlPanelDetectZoneState.UnActive ;
			}
			
			break ;			
		}
	}
	
	private void CheckTutorialEverActiveControlPanel()
	{
		// check tutorial event of active control panel
		if( false == m_IsEverActiveControlPanel )
		{
			GameObject globalSingletonObj = GlobalSingleton.GetGlobalSingletonObj() ;
			if( null != globalSingletonObj )
			{
				TutorialEvent tutorialEvent = globalSingletonObj.GetComponent<TutorialEvent>() ;
				if( null != tutorialEvent )
				{
					// tutorialEvent.m_IsActiveControlPanel = m_IsEverActiveControlPanel = true ;
				}
			}			
		}

	}
	
}

#endif