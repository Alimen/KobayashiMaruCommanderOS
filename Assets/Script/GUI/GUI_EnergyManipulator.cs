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
@file GUI_EnergyManipulator.cs
@author NDark

# EnergySliderType 能源控制器調棒的種類
# EnergySliderStruct 能源控制器調棒的資料結構
# GUI_EnergyManipulator 能源控制器

@date 20121227 file started
@date 20130101 by NDark
. remove class method TrySetNow() of EnergySliderStruct
. add class member m_Changed of GUI_EnergyManipulator
. add class member m_Changed of GUI_EnergyManipulator
. add class method SyncSliderValue()
. remove class method SyncData()
@date 20130113 by NDark . remove class member m_AuxiliaryEnergyNow of EnergySliderStruct
@date 20130119 by NDark . refactor and comment.

*/
using UnityEngine;
using System.Collections.Generic;

// 能源控制器調棒的種類 
[System.Serializable]
public enum EnergySliderType
{
	EnergyManipulatorSymbol_Auxiliary ,
	EnergyManipulatorSymbol_Weapon ,
	EnergyManipulatorSymbol_Shield ,
	EnergyManipulatorSymbol_Sensor ,
	EnergyManipulatorSymbol_Engine ,
	EnergyManipulatorSymbol_Max ,
}

/*
能源控制器調棒的資料結構
# 描述調棒上的符號大小
# 調棒上的符號物件
# 描述調棒的大小
# m_Value 目前調棒的數值
# 能否被改變，影響到GUI使用的種類
*/
[System.Serializable]
public class EnergySliderStruct
{
	public static float m_MaximumSymbolSize = 32.0f ;
	public static float m_DefaultSymbolSize = 32.0f ;
	public static float m_SliderWidth = 10.0f ;
	public static float m_SliderHeight = 200.0f ;
	public NamedObject m_SymbolGUIObject = new NamedObject() ;
	public Rect m_Rect = new Rect() ;
	public StandardParameter m_Value = new StandardParameter( 1.0f , 2.0f , 0.0f , 1.0f ) ;
	public bool m_IsAllowToAlter = true ;
	
	public EnergySliderStruct()
	{
	}

	public EnergySliderStruct( EnergySliderStruct _src )
	{
		m_SymbolGUIObject = new NamedObject( _src.m_SymbolGUIObject ) ;
		m_Rect = _src.m_Rect ;
		m_Value = _src.m_Value ;
	}
	
	public EnergySliderStruct( string _SymbolObjName )
	{
		m_SymbolGUIObject = new NamedObject( _SymbolObjName ) ;
	}
	
	public void InitializeSliderRect()
	{
		Camera mainCamera = Camera.mainCamera ;
		if( null != this.m_SymbolGUIObject.Obj )
		{
			GameObject SymbolObject = this.m_SymbolGUIObject.Obj ;
			GUITexture guiTexture = SymbolObject.GetComponent<GUITexture>() ;
			
			if( null != guiTexture )
			{
				float size = guiTexture.pixelInset.height ;
				Vector3 viewportPos = new Vector3( 
					SymbolObject.transform.position.x ,
					1.0f - SymbolObject.transform.position.y , 
					0 ) ;
				Vector3 screenPos = mainCamera.ViewportToScreenPoint( viewportPos ) ;
				screenPos.x -= m_SliderWidth / 2 ;
				screenPos.y += size / 2 ;
				
				this.m_Rect.Set( screenPos.x ,
								 screenPos.y ,
								 m_SliderWidth , 
								 m_SliderHeight ) ;
			}
		}
	}
	
	public bool IsAllowToAlter()
	{
		return m_IsAllowToAlter ;
	}
	
}


/*
能源控制器
# 掛在 物件 GUI_EnergyManipulator 上
# 各調棒的資料結構
# 初始化會取得一次主角船資料
# 每次改變能源後會重新取得一次資料
# 顯示時才會更新GUI
# 可以由外界設定顯示或關閉
# RetrieveData() 取得主角船資料並同步調棒
# SyncSliderValue() 依照UnitData同步調棒的now與max
# TrySetValue() 設定能源數值到UnitData內
# OnGUI() 繪出能源調棒
## 使用 GUI.VerticalSlider
## 使用 GUI.VerticalScrollbar
# m_Sliders[  ].m_Value 的值都是絕對能源(是該部位總和量)
*/
public class GUI_EnergyManipulator : MonoBehaviour 
{

	// 圖示的物件:用來標示位置
	Dictionary<EnergySliderType , EnergySliderStruct> m_Sliders = new Dictionary<EnergySliderType, EnergySliderStruct>() ;
	bool m_Active = false ;
	bool m_init =false ;
	bool m_Changed = false  ;
	
	public void Active( bool _Show )
	{
		if( true == _Show &&
			false == m_init )
		{
			m_init = true ;
			RetrieveData() ;
		}
		m_Active = _Show ;
	}	
	
	public void Show( bool _Show )
	{
		ShowGUITexture.Show( this.gameObject , _Show , true , true ) ;
	}
	public void Hide()
	{
		m_Active = false ;
		ShowGUITexture.Show( this.gameObject , false , true , true ) ;
	}
	
	// Use this for initialization
	void Start () 
	{
		for( int i = 0 ; i < (int) EnergySliderType.EnergyManipulatorSymbol_Max ; ++i )
		{
			EnergySliderType index = (EnergySliderType) i ;
			m_Sliders.Add( index , 
				new EnergySliderStruct( index.ToString() ) ) ;
			m_Sliders[ index ].InitializeSliderRect() ;
		}
		m_Sliders[ EnergySliderType.EnergyManipulatorSymbol_Auxiliary ].m_IsAllowToAlter = false ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( false == m_Active )
			return ;
		if( true == m_Changed )
		{
			RetrieveData() ;
			m_Changed = false ;
		}
	}
	
	void OnMouseDown()
	{
		GlobalSingleton.TellMainCharacterNotToTriggerOtherClick() ;
	}	
	
	private void SyncSliderValue( UnitData unitData , EnergySliderType _Type )
	{
		float now = 0 ;
		float max = 0 ;		
		switch( _Type )
		{
		case EnergySliderType.EnergyManipulatorSymbol_Engine :
			unitData.RetrieveImpulseEngineEnergy( out now , out max ) ;
			break ;
		case EnergySliderType.EnergyManipulatorSymbol_Sensor :
			unitData.RetrieveSensorEnergy( out now , out max ) ;
			break ;
		case EnergySliderType.EnergyManipulatorSymbol_Shield :
			unitData.RetrieveShieldEnergy( out now , out max ) ;
			break ;
		case EnergySliderType.EnergyManipulatorSymbol_Weapon :
			// weapon
			unitData.RetrieveWeaponEnergy( out now , out max ) ;
			break ;
		case EnergySliderType.EnergyManipulatorSymbol_Auxiliary :
			unitData.GetAuxiliaryEnergyValue( out now , out max ) ;			
			// Debug.Log( "unitData.RetrieveAllComponentEnergy now " + now + " max " + max ) ;
			break ;				
		}
		m_Sliders[ _Type ].m_Value.max = max ;
		m_Sliders[ _Type ].m_Value.now = now ;		
		
	}
	
	private void TrySetValue( UnitData unitData , EnergySliderType _Type , float _Value )
	{
		// Debug.Log( "TrySetValue" + _Value ) ;
		switch( _Type )
		{
		case EnergySliderType.EnergyManipulatorSymbol_Engine :
			unitData.TrySetImpulseEngineEnergy( _Value ) ;
			break ;
		case EnergySliderType.EnergyManipulatorSymbol_Sensor :
			unitData.TrySetSensorEnergy( _Value ) ;
			break ;
		case EnergySliderType.EnergyManipulatorSymbol_Shield :
			unitData.TrySetShieldEnergy( _Value ) ;
			break ;
		case EnergySliderType.EnergyManipulatorSymbol_Weapon :
			unitData.TrySetWeaponEnergy( _Value ) ;
			break ;
		}
	}
	
	private void RetrieveData()
	{
		GameObject mainCharacter = GlobalSingleton.GetMainCharacterObj() ;
		if( null == mainCharacter )
			return ;
		
		UnitData unitData = mainCharacter.GetComponent<UnitData>() ;
		if( null == unitData )
			return ;
		
		for( int i = 0 ; i < (int) EnergySliderType.EnergyManipulatorSymbol_Max ; ++i )
		{
			EnergySliderType index = (EnergySliderType) i ;
			if( false == m_Sliders.ContainsKey( index ) )
				continue ;			
			SyncSliderValue( unitData , index ) ;
		}	
	}
	
	private void OnGUI()
	{
		if( false == m_Active )
			return ;
		
		GameObject mainCharacter = GlobalSingleton.GetMainCharacterObj() ;
		if( null == mainCharacter )
			return ;
		
		UnitData unitData = mainCharacter.GetComponent<UnitData>() ;
		if( null == unitData )
			return ;
		
		
		for( int i = 0 ; i < (int) EnergySliderType.EnergyManipulatorSymbol_Max ; ++i )
		{
			EnergySliderType index = (EnergySliderType) i ;
			if( false == m_Sliders.ContainsKey( index ) )
				return ;
			
			EnergySliderStruct slider = m_Sliders[ index ] ;
			float adjustValue = 0.0f ;
			GUI.enabled = slider.IsAllowToAlter() ;
			if( true == slider.IsAllowToAlter() )
			{
				// Debug.Log( "min " + slider.m_Value.min + " now " + slider.m_Value.now + " max " + slider.m_Value.max ) ;
				adjustValue = GUI.VerticalSlider( 
					slider.m_Rect , 
					slider.m_Value.now ,
					slider.m_Value.max , 
					slider.m_Value.min ) ;
				
				if( adjustValue != slider.m_Value.now )
				{
					m_Changed = true ;
					TrySetValue( unitData , index , adjustValue ) ;
				}
			}
			else
			{
				GUI.VerticalScrollbar(
					slider.m_Rect , 
					slider.m_Value.now , 0.05f ,
					slider.m_Value.max , 
					slider.m_Value.min ) ;				
			}
		}
	}
}
