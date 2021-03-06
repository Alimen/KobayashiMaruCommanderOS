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
@file GUIUpdate.cs
@brief GUI Update 主要GUI更新
@author NDark


Update()
# UpdateMainCharacterControlGUI() 更新選擇框
# UpdateSelectTargetUnitDataGUI() 更新選擇目標UnitDataGUI
# UpdateMainCharacterSelectionGUI() 更新主角UnitDataGUI
		
@date 20121103 by NDark . close select unit data gui , when the object is destroyed
@date 20121110 by NDark 
. add class member m_GUI_SelectUnitBackgroundTemplateName from UnitSelectionSystem.cs
. add class method CreateUnitDataGUI() from UnitSelectionSystem.cs
. add class method DestroyTargetUnitDataGUI() from UnitSelectionSystem.cs
. remove argument of class method DestroyTargetUnitDataGUI()
. add class method CreateSelectTargetUnitDataGUI()
. add class GUIObjNameSet from UnitSelectionSystem.cs
. refactor unit data GUI update/create
. change type of m_ObjMap to Dictionary<string , NamedObject>
. clear m_SelectTargetName at DestroyTargetUnitDataGUI()
@date 20121114 by NDark . add code for de-active unit data gui of select target at UpdateMainCharacterSelectionGUI()
@date 20121119 by NDark 
. add class member Debug_IfUpdateMouseGUI
. add class member m_MouseCursor 
. add class method UpdateMouseGUI()
@date 20121121 by NDark . add code for creation of trackor beam at CreateUnitDataGUI()
@date 20121124 by NDark . add sensor for UnitDataGUI at CreateUnitDataGUI()
@date 20121204 by NDark . comment.
@date 20121208 by NDark . add impulse engine for CreateUnitDataGUI()
@date 20121219 by NDark 
. remove class member m_GUI_SelectUnitBackgroundTemplateName
. remove class member m_MainCharacterGUIBackgroundName
. add class member m_MainCharacterGUIBackground
. replace by m_UnitDataGUITextureName at CreateUnitDataGUI()
@date 20121225 by NDark
. add class member m_UnitDataGUISwitcherObj
. add class method EnableUnitDataGUISwitcher()
@date 20121226 by NDark
. remove class member Debug_IfUpdateMouseGUI
. move class member m_MouseCursor to GUI_MouseCursor.cs
. move class member UpdateMouseGUI() to GUI_MouseCursor.cs
@date 20130119 by NDark . hide the unitdata gui selections at DestroyTargetUnitDataGUI()
@date 20130203 by NDark . add class method DestroyMainCharacterUnitDataGUI()

*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
GUI物件集合
每個單位之下的UnitData GUI我們用一個集合把她們收集起來.
 */
[System.Serializable]
public class GUIObjNameSet
{
	public string m_UnitName = "" ;
	// key is the shield component at UnitData, the value is GUI object name of that component.
	public Dictionary<string , NamedObject> m_ObjMap = new Dictionary<string, NamedObject>() ;
}

public class GUIUpdate : MonoBehaviour 
{
	/// <summary>
	/// The debug_flag if we need to update GUI for main character control, default is true.
	/// </summary>
	public bool Debug_IfUpdateMainCharacterControlGUI = true ;
	
	public bool Debug_IfUpdateSelectTargetControlGUI = true ;
	
	// The debug flag if we need to update GUI for main character selection
	public bool Debug_IfUpdateMainCharacterSelectionGUI = true ;
	
	// Debug flag of pulse engine ratio
	public float Debug_ImpulseEngineRatio = 0.0f ;
	
	// GUI for unit data 
	public string m_SelectTargetName = "" ;
	
	
	public NamedObject m_SelectTargetGUIBackground = new NamedObject() ;
	public GUIObjNameSet m_SelectTargetComponentGUIObjNames = new GUIObjNameSet() ;
	
	public NamedObject m_MainCharacterGUIBackground = new NamedObject() ;
	public GUIObjNameSet m_MainCharacterComponentGUIObjNames = new GUIObjNameSet() ;
	
	NamedObject m_UnitDataGUISwitcherObj = new NamedObject( "GUI_SelectTargetUnitDataGUISwitcher" ) ;
	// Use this for initialization
	void Start () 
	{
	}
	
	/*
	 * Destroies the target unit data GUI background object ( and its relative child )
	 * clear target GUI background object name.
	 */
	public void DestroyTargetUnitDataGUI()
	{
		if( null != m_SelectTargetGUIBackground.Obj )
		{
			GameObject.Destroy( m_SelectTargetGUIBackground.Obj ) ;
		}
		m_SelectTargetName = "" ;
		
		// 同時強制關閉UnitDataGUI的選擇框
		GameObject selectionUnActive = GlobalSingleton.GetGUIUnitDataSelection_UnActive() ;
		if( null != selectionUnActive )
		{
			ShowGUITexture.Show( selectionUnActive , false ,false , true ) ;
		}
	}
	
	public void DestroyMainCharacterUnitDataGUI()
	{
		if( null != m_MainCharacterGUIBackground.Obj )
		{
			GameObject.Destroy( m_MainCharacterGUIBackground.Obj ) ;
			m_MainCharacterGUIBackground.Name = "" ;
		}	
	}	
	
	// 創造選擇物件的UnitDataGUI
	public void CreateSelectTargetUnitDataGUI( string _TargetName )
	{
		string SelectTargetGUIBackgroundName = "" ;
		
		CreateUnitDataGUI( _TargetName , 
						   ConstName.UnitDataGUIBackgroundPrefabName ,
						   ref SelectTargetGUIBackgroundName ,
						   ref this.m_SelectTargetComponentGUIObjNames ) ;
		

		this.m_SelectTargetGUIBackground.Name = SelectTargetGUIBackgroundName ;
		this.m_SelectTargetName = _TargetName ;
		
		EnableUnitDataGUISwitcher() ;
	}
	
	private void EnableUnitDataGUISwitcher()
	{
		if( null != m_UnitDataGUISwitcherObj.Obj )
		{
			ClickOnGUI_SwitchUnitDataGUI switchGUI = m_UnitDataGUISwitcherObj.Obj.GetComponent<ClickOnGUI_SwitchUnitDataGUI>() ;
			if( null != switchGUI )
			{
				switchGUI.EnableText( true ) ;
			}
		}
	}
	
	// 設定 GUITxture
	private void SetUnitDataGUITexture( GameObject _GUIComponentObj , 
										string _ChildName , 
										float _BackgroundGUIPosZ , 
										float _GUIPosShiftInY ,
										Rect _GUIPosRect )
	{
		const float ShiftInZ = 1.0f ;
		if( null == _GUIComponentObj )
			return ;
		
		Transform trans = _GUIComponentObj.transform.FindChild( _ChildName ) ;
		if( null != trans )
		{
			GameObject obj = trans.gameObject ;
			
			// shift in z
			Vector3 orgPos = obj.transform.position ;
			obj.transform.position = new Vector3( orgPos.x , 
												  orgPos.y , 
												  _BackgroundGUIPosZ + ShiftInZ ) ;
			
			GUITexture guiTexture = obj.GetComponent<GUITexture>() ;
			if( null != guiTexture )
			{
				guiTexture.pixelInset = new Rect( (int) _GUIPosRect.x , 
												  (int) (_GUIPosRect.y + _GUIPosShiftInY ) ,
												  guiTexture.pixelInset.width , 
												  guiTexture.pixelInset.height ) ;
			}
		}
	}
	
	// 設定 GUIText
	private void SetUnitDataGUIText( GameObject _GUIComponentObj , 
									 string _DisplayName ,
									 string _ChildName , 
									 float _BackgroundGUIPosZ , 
									 Rect _GUIPosRect )
	{
		const float ShiftInZ = 1.0f ;
		if( null == _GUIComponentObj )
			return ;
		
		Transform trans = _GUIComponentObj.transform.FindChild( _ChildName ) ;
		if( null != trans )
		{
			GameObject obj = trans.gameObject ;
			
			// shift in z
			Vector3 orgPos = obj.transform.position ;
			obj.transform.position = new Vector3( orgPos.x , 
												  orgPos.y , 
												  _BackgroundGUIPosZ + ShiftInZ ) ;
			
			GUIText guiText = obj.GetComponent<GUIText>() ;
			if( null != guiText )
			{
				guiText.text = _DisplayName ;
				guiText.pixelOffset = new Vector2( (int) _GUIPosRect.x , 
												  (int) _GUIPosRect.y ) ;
			}
		}
	}
	/*
	 * @brief Create GUI for unit data
	 * 依照傳入的單位物件及樣本物件來創造一系列GUI物件
	 * @param _TargetName the name of target unit
	 * @param _PrefabName the prefab name of background object,
	 * Template_GUI_SelectUnitDataBackground / Template_GUI_MainCharacterUnitDataBackground
	 * @param _GUIBackgroundObjName the created background object name.
	 * @param _GUIShieldSet the shield set of all GUI objects
	 * @param _GUIWeaponSet the weapon set of all GUI objects
	 */
	public void CreateUnitDataGUI( string _TargetName , 
								   string _PrefabName ,
								   ref string _GUIBackgroundObjName ,
								   ref GUIObjNameSet _GUIComponentObjSet )
	{
		// target infomation
		UnitData unitData = GlobalSingleton.GetUnitData( _TargetName ) ;
		if( null == unitData )
		{
			Debug.Log( "CreateUnitDataGUI() null == unitData _TargetName=" + _TargetName ) ;
			return ;
		}
		
		// create background object
		_GUIBackgroundObjName = ConstName.CreateGUIBackgroundObjName( _TargetName ) ;

		GameObject guiBackgroundObj = PrefabInstantiate.Create( _PrefabName , _GUIBackgroundObjName ) ;
		if( null == guiBackgroundObj )
			return ;
		
		// change texture for gui background object
		GUITexture guiTexture = guiBackgroundObj.GetComponentInChildren<GUITexture>() ;
		if( null == guiTexture )
			return ;			
		
		// for assign correct background texture		
		string ResourceName = unitData.m_UnitDataGUITextureName ;
		// Debug.Log( ResourceName ) ;
		
		Texture texture = ResourceLoad.LoadTexture( ResourceName ) ;
		if( null == texture )
		{
			Debug.Log( "CreateUnitDataGUI() : No such resource:" + ResourceName ) ;
			return ;
		}
		guiTexture.texture = texture ;
		
		// creating gui obj for each component
		string keyword ;
		_GUIComponentObjSet.m_ObjMap.Clear() ;
		string [] keys = new string[ unitData.componentMap.Count ];
		unitData.componentMap.Keys.CopyTo( keys , 0 ) ;
		

		// for all component
		// collect the name 
		foreach( string componentName in keys )
		{
			if( -1 != componentName.IndexOf( "Shield" ) )
				keyword = "Shield" ;
			else if( -1 != componentName.IndexOf( "Sensor" ) )
				keyword = "Shield" ;			
			else if( -1 != componentName.IndexOf( ConstName.UnitDataComponentImpulseEnginePrefix ) )
				keyword = "Shield" ;
			else if( -1 != componentName.IndexOf( "Weapon" ) )
				keyword = "Weapon" ;
			else
				continue ;

			UnitComponentData component = unitData.componentMap[ componentName ] ;
			
			string componentGUIObjName = ConstName.CreateGUIComponentObjectName( _TargetName , 
																		   	  componentName ) ;
			// Debug.Log( "componentObjName=" + componentObjName ) ;
			GameObject componentGUIObj = PrefabInstantiate.Create( "Template_GUI_Component_" + keyword , 
																   componentGUIObjName ) ;
			
			if( null == componentGUIObj )
				continue ;

			// gui position from component param
			Rect guiRect = component.m_ComponentParam.GUIRect ;
			
			SetUnitDataGUITexture( componentGUIObj , 
								   ConstName.UnitDataGUIComponentHP , 
								   guiBackgroundObj.transform.localPosition.z , 
								   0.0f ,
								   guiRect ) ;

			SetUnitDataGUITexture( componentGUIObj , 
								   ConstName.UnitDataGUIComponentReload , 
								   guiBackgroundObj.transform.localPosition.z , 
								   -5.0f ,
								   guiRect ) ;

			SetUnitDataGUIText( componentGUIObj , 
								component.m_ComponentParam.GUIDisplayName , 
								ConstName.UnitDataGUIComponentLabel , 
								guiBackgroundObj.transform.localPosition.z , 
								guiRect ) ;

			if( null != guiBackgroundObj )
			{
				componentGUIObj.transform.parent = guiBackgroundObj.transform ;
			}

			_GUIComponentObjSet.m_ObjMap.Add( componentName , new NamedObject( componentGUIObjName ) ) ;
		}
		_GUIComponentObjSet.m_UnitName = _TargetName ;
		// Debug.Log( "_GUIComponentObjSet.m_UnitName=" + _TargetName ) ;

	}
	
	
	// 更新部件的GUI的資料
	// 呼叫 UpdateGUITextureBar() 來更新
	private void UpdateGUIComponentObject( GameObject _GUIComponent , 
										   string _ChildName , 
										  float _Ratio )
	{
		if( null == _GUIComponent )
			return ;
		// Debug.Log( _GUIComponent.name + " " + _ChildName + " " + _Ratio.ToString() ) ;
		Transform trans = _GUIComponent.transform.FindChild( _ChildName ) ;
		if( null != trans )
		{
			GameObject obj = trans.gameObject ;
			GUITexture guiTexture = obj.GetComponent<GUITexture>() ;			
			UpdateGUITextureBar( guiTexture , _Ratio ) ;
		}
	}
	
	// 更新 GUITexture 的長度
	private void UpdateGUITextureBar( GUITexture _GUITexture , float _Ratio )
	{
		const float BarWidth = 50.0f ;
		if( null == _GUITexture )
			return ;
		
		_GUITexture.pixelInset = new Rect( _GUITexture.pixelInset.x , 
										   _GUITexture.pixelInset.y , 
										   BarWidth * _Ratio , 
										   _GUITexture.pixelInset.height ) ;
	}
	
	// 更新 GUIText的顏色
	private void UpdateGUIText( GameObject _GUIComponent , 
								string _ChildName ,
								UnitComponentData _ComponentData )
	{
		if( null == _GUIComponent || 
			null == _ComponentData )
			return ;
		
		Color LabelColor = ComponentStatusColor.GetColor( _ComponentData.componentStatus ) ;
		
		Transform trans = _GUIComponent.transform.FindChild( _ChildName ) ;
		if( null != trans )
		{
			GameObject obj = trans.gameObject ;
			GUIText guiText = obj.GetComponent<GUIText>() ;			
			if( null != guiText )
			{
				guiText.material.color = LabelColor ;
			}
		}
	}
	
	/* 
	 * 更新一個部件的各個UnitDataGUI物件
	 * 呼叫 UpdateGUIComponentObject() 更新HP
	 * 呼叫 UpdateGUIComponentObject() 更新Reload
	 * 呼叫 UpdateGUIText() 更新 Label
	 */
	private void UpdateAllGUIObjsOfGUIComponent( UnitComponentData _ComponentData ,
												 GameObject _GUIComponent )
	{
		if( null == _GUIComponent || 
			null == _ComponentData )
			return ;

		// try find HP
		UpdateGUIComponentObject( _GUIComponent , ConstName.UnitDataGUIComponentHP , _ComponentData.m_HP.Ratio() ) ;
		
		// try find Reload
		UpdateGUIComponentObject( _GUIComponent , ConstName.UnitDataGUIComponentReload , _ComponentData.m_ReloadEnergy.Ratio() ) ;
		
		// try find Label
		UpdateGUIText( _GUIComponent , ConstName.UnitDataGUIComponentLabel , _ComponentData ) ;

	}
	
	/*
	 * 更新單位的所有部件的資料
	 * 呼叫 UpdateAllGUIObjsOfGUIComponent()
	 */
	private void UpdateUnitDataGUI( UnitData _UnitData , 
									GUIObjNameSet _ComponentGUIObjNames )
	{

		foreach( KeyValuePair<string, NamedObject> pair in _ComponentGUIObjNames.m_ObjMap )
		{
			string componentName = pair.Key ;
			
			if( false == _UnitData.componentMap.ContainsKey( componentName ) )
				continue ;
			GameObject GUIObj = pair.Value.Obj ;
			if( null == GUIObj )
				continue ;
			// Debug.Log( "componentName " + componentName ) ;
			UnitComponentData component = _UnitData.componentMap[ componentName ] ;
			UpdateAllGUIObjsOfGUIComponent( component , GUIObj ) ;
			
		}
		
	}
	
	// 更新目標物件的UnitDataGUI
	private void UpdateSelectTargetUnitDataGUI() 
	{
		if( 0 == m_SelectTargetName.Length )
			return ;
		
		UnitData unitData = GlobalSingleton.GetUnitData( m_SelectTargetName ) ;
		if( null == unitData )
		{
			// unit is destroy , de-active
			GameObject mainChar = GlobalSingleton.GetMainCharacterObj() ;
			
			if( null != mainChar )
			{
				UnitSelectionSystem selectSys = mainChar.GetComponent<UnitSelectionSystem>() ;
				if( null != selectSys )
				{
					selectSys.ActiveSelectInformation( m_SelectTargetName , false ) ;
				}
			}
			return ;		
		}
		
		UpdateUnitDataGUI( unitData , 
						   this.m_SelectTargetComponentGUIObjNames ) ;
		
	}
	
	// 更新主角物件的UnitDataGUI
	private void UpdateMainCharacterControlGUI() 
	{
		// update pulse engine ratio
		GameObject mainCharacterObj = GlobalSingleton.GetMainCharacterObj() ;
		if( null == mainCharacterObj )
		{
			Debug.Log( "null == mainCharacterObj" ) ;
			DestroyMainCharacterUnitDataGUI() ;
			return ;
		}

		UnitData unitData = mainCharacterObj.GetComponent<UnitData>() ;
		if( null == unitData )
			return ;
		
		string pulseEngineRatioStr = ConstName.UnitDataComponentImpulseEngineRatio ;
		GameObject GUI_ImpulseEngineRatioObj = GameObject.Find( "GUI_ImpulseEngineRatio" ) ;
		if( null != GUI_ImpulseEngineRatioObj )
		{
			if( true == unitData.standardParameters.ContainsKey( pulseEngineRatioStr ) )
			{
				// the value or ImpulseEngineRatio of this ship
				float ratio = unitData.standardParameters[ pulseEngineRatioStr ].now ;
				Debug_ImpulseEngineRatio = ratio ;
				GUITexture guiTexture = GUI_ImpulseEngineRatioObj.GetComponent<GUITexture>() ;
				if( null != guiTexture )
					guiTexture.pixelInset = new Rect( 0 , 0 , 50 * ratio , guiTexture.pixelInset.height ) ;				
			}
		}
		
		UpdateUnitDataGUI( unitData , 
			m_MainCharacterComponentGUIObjNames  ) ;		
	}

	// Create the unit data GUI for main character
	public void CreateMainCharacterUnitDataGUI()
	{
		string PrefabName = ConstName.UnitDataGUIMainCharacterBackgroundPrefabName ;
		
		string BackgroundName = "" ;
		this.CreateUnitDataGUI( ConstName.MainCharacterObjectName , 
								PrefabName , 
								ref BackgroundName , 
								ref m_MainCharacterComponentGUIObjNames ) ;
		m_MainCharacterGUIBackground.Name = BackgroundName ;
		// Debug.Log( "m_MainCharacterGUIBackground.Name=" + m_MainCharacterGUIBackground.Name ) ;
		m_MainCharacterGUIBackground.ForceObj() ;
		if( null == m_MainCharacterGUIBackground.Obj )
			Debug.Log( "null == m_MainCharacterGUIBackground.Obj" ) ;
		
		
		
	}
	
	
	/*
	 * This function will update screen position and valid of this selection GUI object
	 * 更新選擇框的位置
	 */
	private bool UpdateScreenPositionOfThisGUISelectionObject( ref GameObject _GUISelectionObj , 
												  			   ref SelectInformation _Info )
	{
		// update the screen position of this GUI Object
		if( 0 == _Info.TargetUnitName.Length )
		{
			Debug.Log( "Target has no name" ) ;
			return false ;
		}
		
		string targetUnitName = _Info.TargetUnitName ;
		GameObject targetObj = GameObject.Find( targetUnitName ) ;
		if( null == targetObj )
		{
			// the object is destroyed. force this selection is close.
			_Info.isValid = false ;
			return false ;
		}
		
		UnitData unitData = targetObj.GetComponent<UnitData>() ;
		if( null != unitData &&
			false == unitData.IsAlive() )
		{
			// the object is not alive. force this selection is close.
			_Info.isValid = false ;
			return false ;
		}
		
		// find screen position
		Vector3 worldPos = targetObj.transform.position ;							
		Vector3 screenPos = Camera.mainCamera.WorldToViewportPoint( worldPos ) ;
		if( screenPos.x > 1 || screenPos.x < 0 ||
			screenPos.y > 1 || screenPos.y < 0 )
		{
			// out of viewport
			_Info.isValid = false ;
			return false ;
		}
		
		_Info.screenPosition.Set( screenPos.x , screenPos.y , 0.0f ) ;
		_GUISelectionObj.transform.position = _Info.screenPosition ;
		return true ;
	}
	
	/*
	 * Update Main Character Selection GUI
	 * 更新主角選擇框資訊
	 */
	private void UpdateMainCharacterSelectionGUI() 
	{
		
		GameObject mainCharacterObj = GlobalSingleton.GetMainCharacterObj() ;
		if( null == mainCharacterObj )
			return ;
		UnitSelectionSystem selectSys = mainCharacterObj.GetComponent<UnitSelectionSystem>() ;
		if( null == selectSys )
			return ;
		
		// update for all selection
		foreach( KeyValuePair<string , SelectInformation> iMap in selectSys.m_Selections )
		{
			string GUIObjectName = iMap.Key ;
			GameObject guiSelectionObj = GameObject.Find( GUIObjectName ) ;
			if( null == guiSelectionObj )
			{
				// Debug.Log( "There is no such GUI object:" + GUIObjectName ) ;
				return ;
			}

			GUITexture guiTexture = guiSelectionObj.GetComponent<GUITexture>() ;
			if( null == guiTexture )
				return ;
				
			if( true == iMap.Value.isValid )
			{
				SelectInformation info = iMap.Value ;
				if( false == UpdateScreenPositionOfThisGUISelectionObject( ref guiSelectionObj , 
															  			   ref info ) )
				{
					selectSys.ActiveSelectInformation( info.TargetUnitName , false ) ;
				}
			}
			guiTexture.enabled = iMap.Value.isValid ;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		// update GUI for main character
		if( true == Debug_IfUpdateMainCharacterControlGUI )
		{
			UpdateMainCharacterControlGUI() ;
		}
		
		if( true == Debug_IfUpdateSelectTargetControlGUI )
		{
			UpdateSelectTargetUnitDataGUI() ;
		}
		
		if( true == Debug_IfUpdateMainCharacterSelectionGUI )
		{
			UpdateMainCharacterSelectionGUI() ;
		}
	}
	
}// end of GUIUpdate
