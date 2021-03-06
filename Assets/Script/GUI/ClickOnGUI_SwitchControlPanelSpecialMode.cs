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
@file ClickOnGUI_SwitchControlPanelSpecialMode.cs
@author NDark

# 因為特殊的面板只有蓋到 Phaser跟Torpedo所以不需要記錄 牽引光束面版
# 必須紀錄 幾種物件
## 一般 Control Panel Active 
## 一般 Control Panel Unactive
## 特殊 Control Panel Active
## 特殊 Control Panel Unactive
# SwitchControlPanelToSpecialMode() 指定模式
## 切換一般模式
關閉特殊模式的面版
## 切換特殊模式
關閉一般模式
只開啟特殊的UnActive
# SwitchControlPanelSpecialMode() 切換
切換主角的 controller.m_SpecialModeNow 為+1

@date 20121230 file started.
@date 20121231 by NDark 
. rename class member m_MultiAttackControlPanels to m_MultiAttackControlPanels_Active
. add class member m_MultiAttackControlPanels_UnActive
. move enum SpecialModePanel to MainCharacterController.cs
. move class member m_Mode to MainCharacterController.cs
@date 20130112 by NDark . comment.

*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClickOnGUI_SwitchControlPanelSpecialMode : MonoBehaviour 
{
	private List< NamedObject > m_UsualControlPanels_Active = new List<NamedObject>() ;
	private List< NamedObject > m_UsualControlPanels_UnActive = new List<NamedObject>() ;
	private List< NamedObject > m_MultiAttackControlPanels_Active = new List<NamedObject>() ;
	private List< NamedObject > m_MultiAttackControlPanels_UnActive = new List<NamedObject>() ;
	
	public void SwitchControlPanelToSpecialMode( SpecialModePanel _Mode ) 
	{
		MainCharacterController controller = GlobalSingleton.GetMainCharacterControllerComponent() ;
		if( null == controller )
			return ;
		// Debug.Log( _Mode ) ;
		switch( _Mode )
		{
		case SpecialModePanel.None :
	
			foreach( NamedObject obj in m_MultiAttackControlPanels_Active )
			{
				ShowGUITexture.Show( obj.Obj , false , false , false ) ;
			}		
			foreach( NamedObject obj in m_MultiAttackControlPanels_UnActive )
			{
				ShowGUITexture.Show( obj.Obj , false , false , false ) ;
			}		
			controller.CheckControlPanelsUnActive() ;
			controller.CancelControlMode() ;

			break ;
			
		case SpecialModePanel.MultiAttack :
			
			
			foreach( NamedObject obj in m_UsualControlPanels_Active )
			{
				ShowGUITexture.Show( obj.Obj , false , false , false ) ;
			}
			foreach( NamedObject obj in m_UsualControlPanels_UnActive )
			{
				ShowGUITexture.Show( obj.Obj , false , false , false ) ;
			}
			foreach( NamedObject obj in m_MultiAttackControlPanels_UnActive )
			{
				ShowGUITexture.Show( obj.Obj , true , false , false ) ;
			}	
			controller.CancelControlMode() ;
			
			break ;
		}
		controller.m_SpecialModeNow = _Mode ;
	}
	
	// Use this for initialization
	void Start () 
	{
		m_UsualControlPanels_Active.Add( new NamedObject( ConstName.GUIControlPanelPhaser_ActiveName ) ) ;
		m_UsualControlPanels_Active.Add( new NamedObject( ConstName.GUIControlPanelTorpedo_ActiveName ) ) ;
		// m_UsualControlPanels_Active.Add( new NamedObject( ConstName.GUIControlPanelTrakorBeam_ActiveName ) ) ;
		
		m_UsualControlPanels_UnActive.Add( new NamedObject( ConstName.GUIControlPanelPhaser_UnActiveName ) ) ;
		m_UsualControlPanels_UnActive.Add( new NamedObject( ConstName.GUIControlPanelTorpedo_UnActiveName ) ) ;
		// m_UsualControlPanels_UnActive.Add( new NamedObject( ConstName.GUIControlPanelTrakorBeam_UnActiveName ) ) ;
		
		m_MultiAttackControlPanels_Active.Add( new NamedObject( ConstName.GUIControlPanelMultiAttack_ActiveName ) ) ;
		m_MultiAttackControlPanels_UnActive.Add( new NamedObject( ConstName.GUIControlPanelMultiAttack_UnActiveName ) ) ;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	void OnMouseDown()
	{
		// Debug.Log( "OnMouseDown" ) ;
		SwitchControlPanelSpecialMode() ;

		GlobalSingleton.TellMainCharacterNotToTriggerOtherClick() ;
	}
		
	private void SwitchControlPanelSpecialMode()
	{
		MainCharacterController controller = GlobalSingleton.GetMainCharacterControllerComponent() ;
		if( null == controller )
			return ;		
		SpecialModePanel newMode = controller.m_SpecialModeNow ;
		int modeInt = (int)controller.m_SpecialModeNow ;
		
		if( controller.m_SpecialModeNow == SpecialModePanel.MultiAttack )
		{
			newMode = SpecialModePanel.None ;
		}
		else
		{
			newMode = (SpecialModePanel) (modeInt+1) ;
		}
		SwitchControlPanelToSpecialMode( (SpecialModePanel) newMode ) ;
	}
}
