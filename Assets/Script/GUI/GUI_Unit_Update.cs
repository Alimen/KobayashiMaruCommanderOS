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
@file GUI_Unit_Update.cs
@brief 單位血條的GUI更新
@author NDark

# 血量條是每個單位有一份
# 使用樣板名稱 Template_GUI_Ship_ShipIntagraty 
# 在創造單位時 建立 與船隻名稱雷同的物件 
# 物件名稱為 [UnitObjectName]:GUI_Ship_UnitIntagraty
# 依照單位的血量更新GUI的長度
# 此腳本掛在單位上。讓血調隨著單位創造建立。
# 此腳本隨著單位摧毀時，同時摧毀血條物件。
# 血條的部件名稱是"UnitIntagraty"

@date 20121109 by NDark . 因為防護罩有多條,所以移除防護罩的產生與更新部份.
@date 20121114 by NDark . add checking at OnDestroy()
@date 20121203 by NDark . comment.
@date 20121219 by NDark . format.

*/
using UnityEngine;

public class GUI_Unit_Update : MonoBehaviour 
{
	public enum GUI_Unit_UpdateState
	{
		UnActive ,			// 未啟動
		Initialization ,	// 初始化 創造血條
		Active ,			// 啟動中
		Closed ,			// 已關閉
	}

	GUI_Unit_UpdateState m_State ;
	NamedObject m_GUI_UnitIntagraty = new NamedObject() ;	
	private string m_GUI_UnitIntagratyTeamplateName = "" ;
	private string m_UnitIntagratyComponentName = "" ;
	
	// Use this for initialization
	void Start () 
	{
		m_GUI_UnitIntagratyTeamplateName = ConstName.GUI_UnitIntagratyTemplateName ;
		m_UnitIntagratyComponentName = ConstName.UnitDataComponentUnitIntagraty ;
	}
	
	void OnDestroy()
	{
		if( null != m_GUI_UnitIntagraty.GetObj() )
			GameObject.Destroy( m_GUI_UnitIntagraty.Obj ) ;
		m_GUI_UnitIntagraty.Obj = null ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch( m_State )
		{
		case GUI_Unit_UpdateState.UnActive :
			break ;
		case GUI_Unit_UpdateState.Initialization :
			CreateGUI_Unit_UnitIntagratyObject() ;// create gui ship ship intagraty object 
			UpdateGUI_UnitIntagraty() ;
			
			m_State = GUI_Unit_UpdateState.Active ;
			break ;
		case GUI_Unit_UpdateState.Active :
			UpdateGUI_UnitIntagraty() ;
			break ;
		case GUI_Unit_UpdateState.Closed :
			break ;			
		}
	}
	

	public void Active_GUI_Unit( bool _Active )
	{
		if( true == _Active )
		{
			if( m_State == GUI_Unit_UpdateState.UnActive )
			{
				m_State = GUI_Unit_UpdateState.Initialization ;
			}
		}
		else
		{
			
		}
	}
	
	void CreateGUI_Unit_UnitIntagratyObject()
	{
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( null == unitData ||
			false == unitData.componentMap.ContainsKey( m_UnitIntagratyComponentName ) )
		{
			return ;
		}
		
		m_GUI_UnitIntagraty.Name = this.gameObject.name + ":" + "GUI_Unit_UnitIntagraty" ;
		m_GUI_UnitIntagraty.Obj = PrefabInstantiate.CreateByInit( m_GUI_UnitIntagratyTeamplateName , 
										m_GUI_UnitIntagraty.Name , 
										Vector3.zero , 
										Quaternion.identity ) ;
		if( null == m_GUI_UnitIntagraty.Obj )
		{
			Debug.Log( "CreateGUI_Unit_UnitIntagratyObject() : ( null == m_GUI_UnitIntagraty.Obj )" ) ;
			return ;
		}
	}
	
	
	void UpdateGUI_UnitIntagraty()
	{
		// find the correct screen position of this ship
		// find gui ship ship intagraty object
		if( null == m_GUI_UnitIntagraty.Obj )
		{
			Debug.Log( " null == m_GUI_UnitIntagraty.Obj " ) ;
			return ;
		}
		
		// update position follow unit object
		Vector3 screenPosition = Camera.mainCamera.WorldToViewportPoint( this.gameObject.transform.position ) ;
		m_GUI_UnitIntagraty.Obj.transform.position = new Vector3( screenPosition.x , 
																  screenPosition.y , 
																  1.0f ) ;
		
		GUITexture guiTexture = m_GUI_UnitIntagraty.Obj.GetComponent<GUITexture>() ;
		
		// find ratio UnitIntagraty of UnitData
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( null != guiTexture &&
			null != unitData &&
			true == unitData.componentMap.ContainsKey( m_UnitIntagratyComponentName ) )
		{
			UnitComponentData unitIntagraty = unitData.componentMap[ m_UnitIntagratyComponentName ] ;
			float ratio = unitIntagraty.m_HP.Ratio() ;
			float textureMaxLength = 50.0f ;
			guiTexture.pixelInset = new Rect( guiTexture.pixelInset.x , 
											  guiTexture.pixelInset.y , 
											  textureMaxLength * ratio , 
											  guiTexture.pixelInset.height ) ;
		}
	}
		
}
