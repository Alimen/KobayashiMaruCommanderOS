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
@file MainCharacterController.cs
@brief 主角控制
@author NDark 
 
# 檢查加減速，左右轉，武器發射
# 檢查滾輪切換，滑鼠自動移動
# 使用逼逼聲　Common/Audio/WeaponFailBeep　，作為武器發射失敗的警告。
# 控制控制面板
m_ControlPanelsUnActive
m_ControlPanelsActive
# SetClickOnNoMoveFuncThisFrame()
set m_ClickOnNoMoveFuncThisFrame, 設定為true時,有一段時間不會接受其他點選訊號
# 支援系統紀錄
m_LastUpDown
m_LastLeftRight

@date 20121115 by NDark . add class method TrySelectClosestUnit()
...
@date 20130113 by NDark . comment
@date 20130120 by NDark . modify click sec m_MouseMoveIsClick to 0.3

*/
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public enum SelectFunctionMode
{
	None = 0 ,				// 無功能
	WeaponPhaser ,			// 光炮
	WeaponTorpedo ,			// 光雷
	FunctionTrakorBeam ,	// 牽引光束
	
	SpecialModeMultipleAttack ,	// 廣域射擊模式,無法由滾輪開啟,僅能由控制面版的特殊按鈕開啟.
}

[System.Serializable]
public enum SpecialModePanel 
{
	None = 0 ,
	MultiAttack,
}

public class MainCharacterController : MonoBehaviour 
{
	public bool Weapon1IsDown = false ;			// 武器1按鈕是否按下,用來檢查click
	public bool Weapon2IsDown = false ;			// 武器2按鈕是否按下,用來檢查click
	public bool TrackorBeam3IsDown = false ;	// 功能3牽引光束按鈕是否按下,用來檢查click
	public float m_ScrollWheelSum = 0.0f ;			// 滑鼠滾輪累積量
	
	public bool m_IsPressedUpAndDown = false ;		// 是否曾經按下上下,用來檢查tutorial
	public bool m_IsPressedLeftAndRight = false ;	// 是否曾經按下左右,用來檢查tutorial
	public bool m_IsFirePhaser = false ;			// 是否曾經發射光炮,用來檢查tutorial
	public bool m_IsFireTorpedo = false ;			// 是否曾經發射牽引光束,用來檢查tutorial
	
	
	public bool m_IsActiveControlPanelFunction = false ;	// 是否曾經觸發控制面版的功能,用來檢查tutorial
	
	public bool m_ClickOnNoMoveFuncThisFrame = false ;	// 滑鼠按下某物件還沒放開,這段時間不檢查移動.
	
	// 滑鼠點選的檢查,會阻止不必要的點選發生
	public CountDownTrigger m_MouseMoveIsClick = new CountDownTrigger( 0.3f ) ;
	
	// 目前的功能面板觸發的功能
	public SelectFunctionMode m_SelectModeNow = SelectFunctionMode.None ;
	
	public SpecialModePanel m_SpecialModeNow = SpecialModePanel.None ;
	
	// 武器發射失敗的音效
	AudioClip m_Audio_WeaponFailBeep = null ;
	
	bool m_AutoPlay = false ;
	
	// control pannel
	public Dictionary<SelectFunctionMode , NamedObject> m_ControlPanelsUnActive = new Dictionary<SelectFunctionMode, NamedObject>() ;// 全部的control panel (grey)
	public Dictionary<SelectFunctionMode , NamedObject> m_ControlPanelsActive = new Dictionary<SelectFunctionMode , NamedObject>() ;// 全部的control panel (color)
	
	
	// set m_ClickOnNoMoveFuncThisFrame, 設定為true時,有一段時間不會接受其他點選訊號
	public void SetClickOnNoMoveFuncThisFrame( bool _set )
	{
		m_ClickOnNoMoveFuncThisFrame = _set ;
	}
	
	/* 
	 * 其他物件被點選,設定瞄準系統或發射武器/功能
	 * Click on unit , call by other unit's click plane.
	 * # Use UnitSelectionSystem to select target.
	 * # Fire weapon on select target.
	 * # 不能瞄準到自己
	 * # 如果功能面板的功能有啟動,則直接發射武器/功能
	 */
	public void ClickOnUnit( string _ClickObjectName )
	{
		// Debug.Log( "ClickOnUnit=" + _ClickObjectName ) ;
		// do not click on ourself
		if( this.gameObject.name == _ClickObjectName )
			return ;
		
		if( true == m_ClickOnNoMoveFuncThisFrame ||
			true == m_MouseMoveIsClick.IsCountDownToZero() )
			return ;
		
		UnitSelectionSystem selectSys = gameObject.GetComponent<UnitSelectionSystem>() ;
		if( null == selectSys )
			return ;
		if( m_SelectModeNow == SelectFunctionMode.None )
		{
			selectSys.ClickOnUnit( _ClickObjectName ) ;
		}
		else if( m_SelectModeNow == SelectFunctionMode.SpecialModeMultipleAttack )
		{
			FireWeaponInMultipleAttackMode() ;
		}
		else
		{
			if( _ClickObjectName != selectSys.GetPrimarySelectUnitName() )
			{
				selectSys.ClickOnUnit( _ClickObjectName ) ;
			}
				
			switch( m_SelectModeNow )
			{
			case SelectFunctionMode.WeaponPhaser :
				FireWeapon1() ;
				break ;
			case SelectFunctionMode.WeaponTorpedo:
				FireWeapon2() ;
				break ;			
			case SelectFunctionMode.FunctionTrakorBeam :
				ActiveTrackorBeam3() ;
				break ;	
			}
		}
		
		SetClickOnNoMoveFuncThisFrame( true ) ;		
	}
	

	// set m_SelectModeNow to SelectFunctionMode.None
	public void CancelControlMode()
	{
		SwitchControlMode( SelectFunctionMode.None ) ;
	}

	/**
	 * @brief Switch control mode to _Mode
	 * 
	 * -# Check tutorial for active control panel.
	 * -# If _Mode is not none , hide all control panel except _Mode
	 * -# Show only the control panel.
	 * -# Show weapon range object
	 * -# Do not move ship.
	 * -# Set m_SelectModeNow ( and mouse cursor will display correctly. )
	 */
	public void SwitchControlMode( SelectFunctionMode _Mode )
	{
		// Check tutorial for active control panel.
		this.ConfirmTutorialActiveControlPanel() ;		
		
		if( m_SelectModeNow == _Mode )
			return ;
		
		if( _Mode == SelectFunctionMode.SpecialModeMultipleAttack )
		{

		}
		else if( _Mode == SelectFunctionMode.None )
		{
			this.HideAllControlPanel() ;
		}
		else
		{
			this.HideAllControlPanel( _Mode ) ;
			this.ShowControlPanel( _Mode ) ;		
			
		}
		
		// show weapon range
		ShowWeaponRange( _Mode ) ;		
		
		m_ClickOnNoMoveFuncThisFrame = true ;
		// Debug.Log( _Mode ) ;
		m_SelectModeNow = _Mode ;
	}
	

		

	/* 
	船隻的加減速 
	
	# 使用上下控制，檢查加減速按鈕 CheckSpeed()
	# 函式 ConfirmTutorialSpeed() 中，呼叫教學事件 TutorialEvent 來關閉加減速教學
	# 取得 船隻資訊 脈衝速度比例 的標準資料
	# 依照FPS計算 目前的脈衝速度比例 
	 */
	public void CheckSpeed( UnitData _UnitData , float _UpDown )
	{
		if( m_LastUpDown != _UpDown )
			SystemLogManager.AddLog( SystemLogManager.SysLogType.Control , "UpDown:" + _UpDown.ToString() ) ;
		
		// check ever press up and down for tutorial event
		if( 0.0f != _UpDown )
		{
			this.ConfirmTutorialSpeed() ;
		}
		
		_UnitData.AdjustImpulseEngineRatio( _UpDown * Time.deltaTime ) ;
		m_LastUpDown = _UpDown ;
	}	
	
	/*
	h3. 船隻的轉向
	
	# 取得控制左右轉的按鈕 檢查轉向 CheckTurn()
	# 在函式 ConfirmTutorialTurn() 中，呼叫教學事件 TutorialEvent 來關閉轉向教學
	# 如果有啟動自動駕駛(滑鼠點選移動),則不因為沒按鈕而停止轉向,如有按鈕則停止自動駕駛
	# 取得 船隻資訊 轉向速度比例
	# 依照FPS計算 轉向速度比例的調整值
	 */
	public void CheckTurn( UnitData _UnitData , float _LeftRight )
	{
		if( _LeftRight != m_LastLeftRight )
			SystemLogManager.AddLog( SystemLogManager.SysLogType.Control , "LeftRight:" + _LeftRight ) ;
		
		// check ever press left and right
		if( 0.0f != _LeftRight )
		{
			this.ConfirmTutorialTurn() ;
		}
		
		string IMPULSE_ENGINE_ANGULAR_RATIO = ConstName.UnitDataComponentImpulseEngineAngularRatio ;
		if( true == _UnitData.standardParameters.ContainsKey( IMPULSE_ENGINE_ANGULAR_RATIO ) )
		{
			StandardParameter pulseEngineAngularRatio = _UnitData.standardParameters[ IMPULSE_ENGINE_ANGULAR_RATIO ] ;
		
			// check auto pilot
			bool IsInAutoPilot = false ;
			UnitGoToPoint gotoPoint = this.gameObject.GetComponent<UnitGoToPoint>() ;
			if( null != gotoPoint &&
				true == gotoPoint.IsActiveAutoPilot() )
			{
				// do not check clear of angular ratio
				IsInAutoPilot = true ;
			}
		
			if( 0.5f > _LeftRight &&
				-0.5f < _LeftRight )
			{
				if( false == IsInAutoPilot )
				{
					pulseEngineAngularRatio.now = 0 ;// sudden stop under 0.5
				}
				
			}
			else
			{
				
				// check auto pilot
				if( true == IsInAutoPilot )
				{
					// stop autopilot
					gotoPoint.Stop() ;
				}
				
				pulseEngineAngularRatio.now += ( _LeftRight * Time.deltaTime ) ;
				
			}			
		}
		m_LastLeftRight = _LeftRight ;
	}
	
	// 檢查目前船隻是否應該開啟功能面板
	public void CheckControlPanelsUnActive()
	{
		if( true == CheckControlPanelsUnActive( SelectFunctionMode.WeaponPhaser ) )
			ShowGUITexture.Show( m_ControlPanelsUnActive[ SelectFunctionMode.WeaponPhaser ].Obj , true , false , false ) ;
		
		if( true == CheckControlPanelsUnActive( SelectFunctionMode.WeaponTorpedo ) )
			ShowGUITexture.Show( m_ControlPanelsUnActive[ SelectFunctionMode.WeaponTorpedo ].Obj , true , false , false ) ;
		
		if( true == CheckControlPanelsUnActive( SelectFunctionMode.FunctionTrakorBeam ) )		
			ShowGUITexture.Show( m_ControlPanelsUnActive[ SelectFunctionMode.FunctionTrakorBeam ].Obj , true , false , false ) ;
	}

	

	/*
	# 檢查是否是自動播放
	# 讀取 武器發射失敗音訊
	# 初始化控制面板的物件
	# 呼叫 CheckControlPanelsUnActive() 檢查控制面板是否要啟動
	*/
	void Start () 
	{
		m_AutoPlay = GlobalSingleton.IsInAutoPlay() ;
		if( m_AutoPlay )
		{
			m_LastUpDown = 0.0f ;
			m_LastLeftRight = 0.0f ;
		}
		m_Audio_WeaponFailBeep = (AudioClip) Resources.Load( "Common/Audio/WeaponFailBeep" ) ;
		if( null == m_Audio_WeaponFailBeep )
		{
			Debug.Log( "if( null == m_Audio_WeaponFailBeep )" ) ;
		}
		
		m_ControlPanelsActive[ SelectFunctionMode.WeaponPhaser ] = new NamedObject( ConstName.GUIControlPanelPhaser_ActiveName ) ;
		m_ControlPanelsActive[ SelectFunctionMode.WeaponTorpedo ] = new NamedObject( ConstName.GUIControlPanelTorpedo_ActiveName ) ;
		m_ControlPanelsActive[ SelectFunctionMode.FunctionTrakorBeam ] = new NamedObject( ConstName.GUIControlPanelTrakorBeam_ActiveName ) ;
		
		m_ControlPanelsUnActive[ SelectFunctionMode.WeaponPhaser ] = new NamedObject( ConstName.GUIControlPanelPhaser_UnActiveName ) ;
		m_ControlPanelsUnActive[ SelectFunctionMode.WeaponTorpedo ] = new NamedObject( ConstName.GUIControlPanelTorpedo_UnActiveName ) ;
		m_ControlPanelsUnActive[ SelectFunctionMode.FunctionTrakorBeam ] = new NamedObject( ConstName.GUIControlPanelTrakorBeam_UnActiveName ) ;
		CheckControlPanelsUnActive() ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( false == unitData.IsAlive() )
		{
			// The unit is not alive, we should not control it.
			return ;
		}
		
		// speed
		float UpDown = ( false == m_AutoPlay ) ? Input.GetAxis( "Vertical" ) : m_LastUpDown ;
		CheckSpeed( unitData , UpDown ) ;
	
		// turn
		float LeftRight = ( false == m_AutoPlay ) ? Input.GetAxis( "Horizontal" ) : m_LastLeftRight ;
		CheckTurn( unitData , LeftRight ) ;
		
		// fire weapon 1
		CheckWeapon1() ;

		// fire weapon 2
		CheckWeapon2() ;

		// active trackor beam 3
		CheckTrackorBeam3() ;		
		
		CheckMouseScrollWheel() ;
		
		CheckClickToMove() ;
	}
	
	
	/* 依照指定的關鍵字 嘗試發射武器*/
	private bool FireWeaponByKeyword( string _WeaponKeyword , 
									  bool _trySelectClosestUnit )
	{
		UnitSelectionSystem selectSys = this.gameObject.GetComponent< UnitSelectionSystem >() ;
		UnitWeaponSystem weaponSys = this.gameObject.GetComponent<UnitWeaponSystem>() ;
		if( null == weaponSys ||
			null == selectSys )
			return false ;
		
		SelectInformation selectInfo = null ;
		if( true == RetrieveSelectionInfo( selectSys , 
											out selectInfo ) )
		{
			if( true == TryFireWeapon( weaponSys ,
										_WeaponKeyword , 
										selectInfo ) )
			{
				return true ;
			}
		}
		else
		{
			// 如果沒選擇到 就會自動選擇
			// Debug.Log( "false == RetrieveSelectionInfo" ) ;
			if( true == _trySelectClosestUnit )
			{
				UnitSensorSystem sensorSys = this.gameObject.GetComponent<UnitSensorSystem>() ;
				if( null != sensorSys )
					TrySelectClosestUnit( sensorSys , selectSys ) ;
			}	
		}	
		return false ;
	}
	
	/*
	 * 發射武器 1
	 */
	private void FireWeapon1()
	{
		if( true == CheckControlPanelsUnActive( SelectFunctionMode.WeaponPhaser ) )
			SwitchControlMode( SelectFunctionMode.WeaponPhaser ) ;
		FireWeaponByKeyword( "Weapon_Phaser" , true ) ;
	}
	
	/*
	 * 發射武器 2
	 */
	private void FireWeapon2()
	{
		if( true == CheckControlPanelsUnActive( SelectFunctionMode.WeaponTorpedo ) )
			SwitchControlMode( SelectFunctionMode.WeaponTorpedo ) ;		
		FireWeaponByKeyword( "Weapon_PhotonTorpedo" , true ) ;
	}
	
	private void FireWeaponInMultipleAttackMode()
	{
		// Debug.Log( "FireWeaponInMultipleAttackMode" ) ;
		
		
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		UnitSensorSystem sensorSys = this.gameObject.GetComponent<UnitSensorSystem>() ;
		UnitWeaponSystem weaponSys = this.gameObject.GetComponent<UnitWeaponSystem>() ;
		UnitSelectionSystem selectSys = this.gameObject.GetComponent<UnitSelectionSystem>() ;
		
		SelectInformation preSelectInfo = new SelectInformation(  selectSys.GetPrimarySelectInfo() ) ;
		// Debug.Log( "preSelectInfo" + preSelectInfo.TargetUnitName ) ;
		if( null == unitData || 
			null == sensorSys || 
			null == weaponSys || 
			null == selectSys )
			return; 
		Dictionary<string , SelectInformation> fireList = new Dictionary<string, SelectInformation>() ;
		if( true == TryFireWeaponInMultiAttackMode( unitData , sensorSys , weaponSys , selectSys , ref fireList ) )
		{
			// Debug.Log( "true == TryFireWeaponInMultiAttackMode" ) ;
			foreach( string weaponComponentName in fireList.Keys ) 
			{
				weaponSys.ActiveWeapon( weaponComponentName ,
					fireList[ weaponComponentName ].GetTargetUnit() ,
					ConstName.UnitDataComponentUnitIntagraty ) ;
			}
		}
		else
		{
			Debug.Log( "false == TryFireWeaponInMultiAttackMode" ) ;
			// no weapon can fight.
		}
		
		if( null != preSelectInfo )
		{
			selectSys.SetPrimarySelectInfo( preSelectInfo ) ;
			// Debug.Log( "preSelectInfo" + preSelectInfo.TargetUnitName ) ;
		}
	}
	
	private bool TryFireWeaponInMultiAttackMode( UnitData _unitData ,
												UnitSensorSystem _sensorSys ,
												UnitWeaponSystem _weaponSys , 
												UnitSelectionSystem _selectSys ,
												ref Dictionary<string , SelectInformation > _fireList )
	{
		bool ret = false ;
		// 取得最大可能的瞄準框
		int maxSelectionNum = _selectSys.m_MaxSelectionNum ;
		int selectionIndexNow = 0 ;
		string [] selectionKeyArray = new string[ _selectSys.m_Selections.Count ] ;
		_selectSys.m_Selections.Keys.CopyTo( selectionKeyArray , 0 ) ;
		
		
		
		// 取得最大的武器清單,並依照距離來排序
		List<string> possibleWeaponList = RetrievePossibleWeaponListInOrder( _unitData , _weaponSys ) ;
		
		// 取得所有的可能敵人,並依照距離來排序
		List<NamedObject> possibleUnits = RetrievePossibleUnitInOrder( _sensorSys );
		
		// 目標是讓所有的武器都分別對上不同的敵人.然後設定好所有的瞄準框.最後全部開火.
		foreach( NamedObject possibleUnit in possibleUnits )
		{
			// Debug.Log( "possibleUnit=" + possibleUnit.Name ) ;
			// 假如瞄準框已經用盡,離開
			if( selectionIndexNow == maxSelectionNum - 1  )
				break ;
			
			// 每一個單位,找到可以發射的武器,而且最接近中線的武器
			string weaponComponentName = FindMostCloestWeaponComponent( _unitData , 
																		_weaponSys , 
																		possibleUnit , 
																		possibleWeaponList ,
																		ref _fireList ) ;
			// Debug.Log( "weaponComponentName=" + weaponComponentName ) ;
			if( 0 == weaponComponentName.Length )
				continue ;

			// 指定瞄準框的資訊
			string key = selectionKeyArray[ selectionIndexNow ] ;
			_selectSys.m_Selections[ key ].TargetUnitName = possibleUnit.Name ;
			_selectSys.m_Selections[ key ].TargetUnitObject = possibleUnit.Obj ;
			// Debug.Log( "_selectSys.m_Selections " + key + " " + possibleUnit.Name ) ;
			_fireList[ weaponComponentName ] = _selectSys.m_Selections[ key ] ;
			++selectionIndexNow ;
			ret = true ;
		}
		
		// 針對每個瞄準框,發射指定的武器
		return ret ;
	}
	
	private string FindMostCloestWeaponComponent( UnitData _unitData ,
												  UnitWeaponSystem _weaponSys ,
												  NamedObject _possibleUnit ,
												  List<string> _possibleWeaponList ,													
												  ref Dictionary<string , SelectInformation> _fireList ) 
	{
		string ret = "" ;
		
		Dictionary<string,float> angleMap = new Dictionary<string, float>() ;
		foreach( string weaponComponentName in _possibleWeaponList )
		{
			// Debug.Log( "weaponComponentName=" + weaponComponentName ) ;
			if( false == _weaponSys.m_WeaponDataMap.ContainsKey( weaponComponentName ) )
			{
				continue ;
			}
			
			// no trackor beam
			if( -1 != weaponComponentName.IndexOf( ConstName.UnitDataComponentWeaponTrackorBeamPrefix ) )
			{
				continue ;
			}			
			if( true == _fireList.ContainsKey( weaponComponentName ) )
			{
				// 如果已經用掉了就跳過不作
				continue ;
			}

			if( 0 != 
				( _weaponSys.FindAbleWeaponComponent( weaponComponentName , _possibleUnit.Obj ) ).Length
				)
			{
				
				WeaponDataSet weapenDataSet = _weaponSys.m_WeaponDataMap[ weaponComponentName ] ;
				Vector3 toTargetVec = _possibleUnit.Obj.transform.position - this.gameObject.transform.position ;
				Vector3 toComponentVec = weapenDataSet.Component3DObject.transform.position - this.gameObject.transform.position ;
				float angle = Vector3.Angle( toComponentVec , toTargetVec ) ;
				angleMap[ weaponComponentName ] = angle ;
				// Debug.Log( "toComponentVec=" + toComponentVec ) ;
				// Debug.Log( "toTargetVec=" + toTargetVec ) ;
				// Debug.Log( "angleMap[ weaponComponentName ]=" + angle ) ;
			}
		}
		
		// 尋找angle值最小的
		float angleMin = 361.0f ;
		Dictionary<string,float>.Enumerator e = angleMap.GetEnumerator() ;
		while( e.MoveNext() )
		{
			if( e.Current.Value < angleMin )
			{
				angleMin = e.Current.Value ;
				ret = e.Current.Key ;
			}
		}
		// Debug.Log( "FindMostCloestWeaponComponent() ret=" + ret ) ;
		return ret ;
	}
	
	// 取得最大的武器清單,並依照排序
	private List<string> RetrievePossibleWeaponListInOrder( UnitData _unitData , 
															UnitWeaponSystem _weaponSys )
	{
		List<string> ret = new List<string>() ;
		List<string> allWeapens = _unitData.GetAllWeaponComponentNameVec() ;
		List<string> realWeapons = new List<string>() ;
		
		// 剔除trackor beam
		foreach( string weapon in allWeapens )
		{
			if( -1 == weapon.IndexOf( ConstName.UnitDataComponentWeaponTrackorBeamPrefix ) )
			{
				// no trackor beam
				realWeapons.Add( weapon ) ;
			}
		}
		
		while( realWeapons.Count > 0 )
		{
			// 找最遠的
			float maxRange = -1 ;
			string maxRangeString = "" ;
			foreach( string weapon in realWeapons )
			{
				
				float rangeThis = _unitData.componentMap[ weapon ].m_WeaponParam.m_Range ;
				if( rangeThis > maxRange )
				{
					maxRange = rangeThis ;
					maxRangeString = weapon ;
				}
			}
			realWeapons.Remove( maxRangeString ) ;
			ret.Add( maxRangeString ) ;
		}
		
		return ret ;
	}
	
	private List<NamedObject> RetrievePossibleUnitInOrder( UnitSensorSystem _SensorSys )
	{
		List<NamedObject> allEnemyUnits = new List<NamedObject>() ;
		List<NamedObject> ret = new List<NamedObject>() ;
		
		// 剔除友軍
		foreach( NamedObject unit in _SensorSys.m_SensorUnitList )
		{
			if( -1 != unit.Name.IndexOf( "Enemy_" ) )
			{
				allEnemyUnits.Add( unit ) ;
			}
		}
		
		while( allEnemyUnits.Count > 0 )
		{
			float maxRange = -1 ;
			NamedObject maxRangeUnit = null ;			
			
			foreach( NamedObject unit in allEnemyUnits )
			{
				Vector3 toUnit = unit.Obj.transform.position - this.gameObject.transform.position ;
				if( toUnit.magnitude > maxRange )
				{
					maxRange = toUnit.magnitude ;
					maxRangeUnit = unit ;
				}
			}
			
			if( null == maxRangeUnit )// error
				break ;
			
			ret.Add( maxRangeUnit ) ;
			allEnemyUnits.Remove( maxRangeUnit ) ;
			
		}
		
		return ret ;
	}
		
	/*
	 * 檢查武器1按紐
	 */
	private void CheckWeapon1()
	{
		if( true == Input.GetKeyDown( KeyCode.Alpha1 ) )
		{
			// check ever fire phaser
			this.ConfirmTutorialPhaser() ;
			
			if( false == Weapon1IsDown )
			{
				Weapon1IsDown = true ;
				
				// 判斷是否啟動廣域射擊模式,如果是,則另外呼叫發射武器函式
				if( m_SpecialModeNow == SpecialModePanel.MultiAttack )
					FireWeaponInMultipleAttackMode() ;
				else
					FireWeapon1() ;
			}
		}
		else if( false == Input.GetKeyUp( KeyCode.Alpha1 ) )
		{
			Weapon1IsDown = false ;
		}
	}
		
	/*
	 * 檢查武器2按紐
	 */	
	private void CheckWeapon2()
	{
		if( true == Input.GetKeyDown( KeyCode.Alpha2 ) )
		{
			// check ever fire phaser
			this.ConfirmTutorialTorpedo() ;
			
			if( false == Weapon2IsDown )
			{
				Weapon2IsDown = true ;
				
				// 判斷是否啟動廣域射擊模式,如果是,則另外呼叫發射武器函式
				if( m_SpecialModeNow == SpecialModePanel.MultiAttack )
					FireWeaponInMultipleAttackMode() ;
				else
					FireWeapon2() ;
			}
		}
		else if( false == Input.GetKeyUp( KeyCode.Alpha2 ) )
		{
			Weapon2IsDown = false ;
		}
	}
	
	/*
	 * 發射牽引光束
	 * 
	 * # 檢查牽引光束是否正開啟,將其關閉
	 * # 啟動牽引光束
	 */
	private void ActiveTrackorBeam3()
	{
		UnitWeaponSystem weaponSys = this.gameObject.GetComponent<UnitWeaponSystem>() ;
		if( null == weaponSys )
			return ;
		
		if( true == weaponSys.WeaponIsFiring( "Weapon_TrackorBeam" ) )
		{
			weaponSys.StopEffect( "Weapon_TrackorBeam" ) ;
		}
		else
		{
			if( true == CheckControlPanelsUnActive( SelectFunctionMode.FunctionTrakorBeam ) )
				SwitchControlMode( SelectFunctionMode.FunctionTrakorBeam ) ;
			FireWeaponByKeyword( "Weapon_TrackorBeam" , false ) ;
		}
	}
	
	/*
	 * 檢查發射牽引光束
	 */
	private void CheckTrackorBeam3()
	{
		if( true == Input.GetKeyDown( KeyCode.Alpha3 ) )
		{			
			if( false == TrackorBeam3IsDown )
			{
				TrackorBeam3IsDown = true ;
				ActiveTrackorBeam3() ;
			}
		}
		else if( false == Input.GetKeyUp( KeyCode.Alpha3 ) )
		{
			TrackorBeam3IsDown = false ;
		}
	}
	
	/*
	 * 檢查滾輪切換功能
	 */
	private void CheckMouseScrollWheel()
	{
		// you can't swich by wheel in this mode
		if( m_SelectModeNow == SelectFunctionMode.SpecialModeMultipleAttack )
			return ;
		
		float scroll = Input.GetAxis("Mouse ScrollWheel") ;
		
		if( ( m_ScrollWheelSum > 0.0f && scroll < 0.0f )
			||
			( m_ScrollWheelSum < 0.0f && scroll > 0.0f ) )
		{
			m_ScrollWheelSum = scroll ;// 反向
		}
		else
		{
			m_ScrollWheelSum += scroll ;
		}
		
		float switchControlChangeThreashold = 0.3f ;
		if( m_ScrollWheelSum > switchControlChangeThreashold ) 
		{
			this.DeactiveControlPanel( m_SelectModeNow ) ;
			this.TrySwitchControlModeByOneStep( false ) ;
			this.ShowControlPanel( m_SelectModeNow ) ;
			m_ScrollWheelSum = 0.0f ;
		}
		else if( m_ScrollWheelSum < -1 * switchControlChangeThreashold ) 
		{
			this.DeactiveControlPanel( m_SelectModeNow ) ;			
			this.TrySwitchControlModeByOneStep( true ) ;
			this.ShowControlPanel( m_SelectModeNow ) ;
			m_ScrollWheelSum = 0.0f ;
		}
		
		
		
	}
	
	/*
	 * @brief 自動選擇最近的單位 
	 * 
	 * 向感測系統取得最近的單位
	 * 通知選擇系統點選
	 */
	private void TrySelectClosestUnit( UnitSensorSystem _Sensor , 
							   UnitSelectionSystem _Select )
	{
		if( null == _Sensor ||
			null == _Select )
			return ;

		GameObject closest = _Sensor.GetClosestObj() ;
		if( null != closest )
		{
			_Select.ClickOnUnit( closest.name ) ;
		}
	}	

	/* 
	h2. 檢查是否要依照滑鼠按鈕移動 CheckClickToMove()
	
	Check click left mouse button to move
	# 必須在 m_ClickOnNoMoveFuncThisFrame 未啟動的情形下(目前沒有其他物件的點選正在觸發) 才會正確觸發
	# 呼叫 UnitGoToPoint 來設定目標
	# m_ClickOnNoMoveFuncThisFrame 會在 計時器 m_MouseMoveIsClick 結束時清除
	 */
	private void CheckClickToMove()
	{
		if( true == Input.GetMouseButtonDown( 0 ) )
		{
			// mouse left button down press
			// 如果每次按下都rewind()會一直到數不完,導致新指令一直沒反應
			if( true == m_MouseMoveIsClick.IsCountDownToZero() )
			{
				// Debug.Log( "m_MouseMoveIsClick.Rewind() ;" );
				m_MouseMoveIsClick.Rewind() ;
			}
		}
		else if( true == Input.GetMouseButtonUp( 0 ) )
		{
			if( false == m_ClickOnNoMoveFuncThisFrame &&
				false == m_MouseMoveIsClick.IsCountDownToZero() )
			{
				// Debug.Log( "click left to move" ) ;				
				
				// calculate click position
				Vector3 unitPos = this.gameObject.transform.position ;

				// always positive
				float depthOfUnit = Camera.mainCamera.transform.position.y - unitPos.y ;
				
				Vector3 mouse3D = Camera.mainCamera.ScreenToWorldPoint( new Vector3( 
																	Input.mousePosition.x , 
																	Input.mousePosition.y , 
																	depthOfUnit ) ) ;
				// Debug.Log( "Input.mousePosition=" + Input.mousePosition + " mouse3D=" + mouse3D ) ;
				UnitGoToPoint gotoPoint = this.gameObject.GetComponent<UnitGoToPoint>() ;
				if( null != gotoPoint )
				{
					gotoPoint.Setup( mouse3D ) ;
				}
			}
		}// end of else if( true == Input.GetMouseButtonUp( 0 ) )
		
		// clear block sign
		if( true == m_MouseMoveIsClick.IsCountDownToZero() )
		{
			if( true == m_ClickOnNoMoveFuncThisFrame )
			{
				// Debug.Log( "clear click on unit this frame" ) ;
				m_ClickOnNoMoveFuncThisFrame = false ;		
			}
		}
	}// end of CheckClickToMove()

	
	private void TrySwitchControlModeByOneStep( bool _positive )
	{
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( null == unitData )
			return ;
		SelectFunctionMode preMode = m_SelectModeNow ;
		SwitchControlModeByOneStep( _positive ) ;
		// check weapon exist
		string keyword = ConstName.FindWeaponKeyword( m_SelectModeNow ) ;
		
		while( 0 != keyword.Length && // now such keyword 
			   // m_SelectModeNow == SelectFunctionMode.SpecialModeMultipleAttack && // you can't switch to a special mode
			   0 == unitData.GetAllComponentNameVecWithKeyword( keyword ).Count && // no such weapon
			   m_SelectModeNow != preMode  // if there is one mode, just skip this loop.
			 )
		{
			SwitchControlModeByOneStep( _positive ) ;
			keyword = ConstName.FindWeaponKeyword( m_SelectModeNow ) ;
		}		
		
		// Debug.Log( "TrySwitchControlModeByOneStep() m_SelectModeNow=" + m_SelectModeNow ) ;
		if( m_SelectModeNow != SelectFunctionMode.None )
			ShowWeaponRange( m_SelectModeNow ) ;			

	}
	
	// 上下切換一格的功能 set to m_SelectModeNow
	private void SwitchControlModeByOneStep( bool _positive )
	{
		int step = ( true == _positive ) ? 1 : -1 ;
		
		if( false == _positive &&
			m_SelectModeNow == SelectFunctionMode.None )
			m_SelectModeNow = SelectFunctionMode.FunctionTrakorBeam ;
		else if( true == _positive &&
				 m_SelectModeNow == SelectFunctionMode.FunctionTrakorBeam )
			m_SelectModeNow = SelectFunctionMode.None ;
		else
			m_SelectModeNow = (SelectFunctionMode) ( (int) m_SelectModeNow + step ) ;
	}

	
	// 顯示武器範圍
	private void ShowWeaponRange( string _Keyword )
	{
		UnitWeaponSystem weaponSys = this.gameObject.GetComponent<UnitWeaponSystem>() ;
		if( null == weaponSys )
			return ;
		// Debug.Log( "ShowWeaponRange()" ) ;
		weaponSys.ActiveWeaponRangeObject( _Keyword ) ;		
	}	
	
	// 顯示武器範圍
	private void ShowWeaponRange( SelectFunctionMode _Mode )
	{
		// Debug.Log( "ShowWeaponRange()" ) ;
		UnitWeaponSystem weaponSys = this.gameObject.GetComponent<UnitWeaponSystem>() ;
		if( null == weaponSys )
			return ;
		
		switch( _Mode )
		{
		case SelectFunctionMode.WeaponPhaser :
			weaponSys.ActiveWeaponRangeObject( "Weapon_Phaser" ) ;		
			break ;
		case SelectFunctionMode.WeaponTorpedo :
			weaponSys.ActiveWeaponRangeObject( "Weapon_PhotonTorpedo" ) ;
			break ;
		case SelectFunctionMode.FunctionTrakorBeam :
			weaponSys.ActiveWeaponRangeObject( "Weapon_TrackorBeam" ) ;		
			break ;
		case SelectFunctionMode.SpecialModeMultipleAttack :
			// Debug.Log( "SelectFunctionMode.SpecialModeMultipleAttack()" ) ;
			weaponSys.ActiveWeaponRangeObject( "Weapon_Phaser" ) ;
			weaponSys.ActiveWeaponRangeObject( "Weapon_PhotonTorpedo" ) ;
			break ;
		}
	}

	

	/*
	# 在函式 TryFireWeapon() 中，呼叫武器系統 依照關鍵字 取得 可以發射的武器部件 
	# 在函式 TryFireWeapon() 中，呼叫武器系統 傳入 武器部件名稱 目標船艦名稱 發射武器
	# 失敗時發出警告聲
	# 失敗時顯示武器範圍
	*/
	private bool TryFireWeapon( UnitWeaponSystem _WeaponSys , 
								string _WeaponKeyword ,
								SelectInformation _SelectionInfo )
	{
		// find correct weapon 
		string TargetComponentObjectName = ConstName.CreateComponent3DObjectName( _SelectionInfo.TargetUnitName , 
																				  _SelectionInfo.m_TargetComponentName ) ;
		string mainCause = "" ;
		string weaponComponentName = _WeaponSys.FindAbleWeaponComponent( _WeaponKeyword , 
																		 TargetComponentObjectName ,
																		 ref mainCause ) ;
		bool ret = false ;
		// Debug.Log( "ActiveTrackorBeam3() weaponComponentName=" + weaponComponentName ) ;
		if( false == ( ret = _WeaponSys.ActiveWeapon( weaponComponentName , 
									   		  		  _SelectionInfo.GetTargetUnit() ,
													  _SelectionInfo.m_TargetComponentName ) ) )
		{
			// fire weapon fail : beep
			if( null != m_Audio_WeaponFailBeep )
			{
				// show weapon range
				ShowWeaponRange( _WeaponKeyword ) ;				
				
				this.gameObject.audio.PlayOneShot( m_Audio_WeaponFailBeep ) ;
				
				MessageQueueManager manager = GlobalSingleton.GetMessageQueueManager() ;
				if( null != manager )
					manager.AddMessage( mainCause ) ;				
			}
		}
		return ret ;
		
	}
	
	// retrieve selection information from selection system.
	private bool RetrieveSelectionInfo( UnitSelectionSystem _SelectSys , 
										out SelectInformation _SelectionInfo )
	{
		_SelectionInfo = _SelectSys.GetPrimarySelectInfo() ;
		
		if( null == _SelectionInfo ||
			false == _SelectionInfo.isValid )
			return false ;
		
		return true ;
	}
	

	
	private bool CheckControlPanelsUnActive( SelectFunctionMode _Mode )
	{
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		string keyword = ConstName.FindWeaponKeyword( _Mode ) ;
		List<string> componentVec = unitData.GetAllComponentNameVecWithKeyword( keyword ) ;
		return ( componentVec.Count > 0 ) ;			
	}	
	
	private bool CheckControlPanelsUnActive( WeaponType _Type )
	{
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		string keyword = ConstName.FindWeaponKeyword( _Type ) ;
		List<string> componentVec = unitData.GetAllComponentNameVecWithKeyword( keyword ) ;
		return ( componentVec.Count > 0 ) ;			
	}
	

	// tutorial
	private void ConfirmTutorialSpeed()
	{
		// check ever press up and down for tutorial event
		if( false == m_IsPressedUpAndDown )
		{
			m_IsPressedUpAndDown = true ;
			TutorialEvent tutorialEvent = GlobalSingleton.GetTutorialEvent() ;
			if( null != tutorialEvent )
				tutorialEvent.m_IsPressedUpAndDown = m_IsPressedUpAndDown ;
		}
	}
	
	private void ConfirmTutorialTurn()
	{
		// check ever press up and down for tutorial event
		if( false == m_IsPressedLeftAndRight )
		{
			m_IsPressedLeftAndRight = true ;
			TutorialEvent tutorialEvent = GlobalSingleton.GetTutorialEvent() ;
			if( null != tutorialEvent )
				tutorialEvent.m_IsPressedLeftAndRight = m_IsPressedLeftAndRight ;
		}
	}
	
	private void ConfirmTutorialPhaser()
	{
		// check ever fire phaser
		if( false == m_IsFirePhaser )
		{
			m_IsFirePhaser = true ;
			TutorialEvent tutorialEvent = GlobalSingleton.GetTutorialEvent() ;
			if( null != tutorialEvent )
				tutorialEvent.m_IsPressedFirePhaser = m_IsFirePhaser ;
		}
	}
	
	private void ConfirmTutorialTorpedo()
	{
		// check ever fire phaser
		if( false == m_IsFireTorpedo )
		{
			m_IsFireTorpedo = true ;
			TutorialEvent tutorialEvent = GlobalSingleton.GetTutorialEvent() ;
			if( null != tutorialEvent )
				tutorialEvent.m_IsPressedFireTorpedo = m_IsFireTorpedo ;
		}
	}
	
	private void ConfirmTutorialActiveControlPanel()
	{
		if( false == m_IsActiveControlPanelFunction )
		{
			m_IsActiveControlPanelFunction = true ;
			TutorialEvent tutorialEvent = GlobalSingleton.GetTutorialEvent() ;
			if( null != tutorialEvent )
				tutorialEvent.m_IsActiveControlPanelFunction = m_IsActiveControlPanelFunction ;
		}		
	}	
	
	// 關閉顯示指定控制面版
	private void DeactiveControlPanel( SelectFunctionMode _mode )
	{
		int index = ((int)_mode - (int)SelectFunctionMode.WeaponPhaser) ;
		
		if( index >= 0 && index < 3 )
		{
			ShowGUITexture.Show( m_ControlPanelsActive[ _mode ].Obj , false , false , false ) ;
			ControlPanelActive controlPanel = m_ControlPanelsActive[ _mode ].Obj.GetComponent<ControlPanelActive>() ;
			if( null != controlPanel )
				controlPanel.m_CloseActive.Initialize() ;
		}
	}
	
	// 顯示指定控制面版
	private void ShowControlPanel( SelectFunctionMode _mode )
	{
		int index = ((int)_mode - (int)SelectFunctionMode.WeaponPhaser) ;

		if( index >= 0 && index < 3 )
		{
			ShowGUITexture.Show( m_ControlPanelsActive[ _mode ].Obj , true , false , false ) ;
			ControlPanelActive controlPanel = m_ControlPanelsActive[ _mode ].Obj.GetComponent<ControlPanelActive>() ;
			if( null != controlPanel )
				controlPanel.m_CloseActive.Active() ;
			
		}
	}
	
	// 隱藏 所有 control panel active 除了指定的模式之外
	private void HideAllControlPanel( SelectFunctionMode _Exculde = SelectFunctionMode.None )
	{
		int ExculdeIndex = ((int)_Exculde - (int)SelectFunctionMode.WeaponPhaser) ;
		for( int i = 0 ; i < 3 ; ++i )
		{
			SelectFunctionMode index = (SelectFunctionMode)( i + (int)(SelectFunctionMode.WeaponPhaser) ) ;
			if( _Exculde != SelectFunctionMode.None &&
				i != ExculdeIndex )
			{
				ShowGUITexture.Show( m_ControlPanelsActive[ index ].Obj , false , false , false ) ;
			}
		}
	}
	
	private float m_LastUpDown = 99.0f ;
	private float m_LastLeftRight = 99.0f ;
}// end of MainCharacterController
