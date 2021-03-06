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
@file GlobalSingleton.cs
@brief 全域物件及元件取得器 
@author NDark

## m_LevelString 用來記錄目前要讀取的關卡關鍵字串,由選關場景寫入,戰鬥場景讀出
## m_AcknowledgementGUIOBjectName 選關之後是否要開啟感謝的GUIObjectName
## m_CurrentModeSelectSceneString 用來記錄關卡結束後的要回到哪一個選擇關卡場景
## m_CustomActive 是否開啟客製化主角船
## m_CustomPrefabName 客製化主角船的prefab名稱
## m_CustomDefaultPrefabName 客製化主角船的prefab預設名稱
## m_CustomUnitDataName 客製化主角船的unitdata樣本名稱
## m_CustomDefaultUnitDataName 客製化主角船的unitdata樣本預設名稱
## ResetCustomData() 重置及關閉客製化設定
## m_InformationSequenceFilepath 資訊頁面的設定檔路徑
## m_InformationSceneEnd 資訊頁面預設的返回點
## m_InformationSequenceIsInitialized 資訊頁面是否已經初始化
## m_InformationSequenceIndex 資訊頁面的目前索引
## m_InformationSceneNames 資訊頁面的目前表格
## InitializeInformationSequence() 初始化資訊頁面表格
## TryLoadInformationNext() 嘗試讀入資訊頁面下一個場景

@date 20121108 by NDark . replace GameObject.Find() by GameObject.FindGameObjectWithTag() at GetMainCharacterObj()
...
@date 20130109 by NDark . add class method ActiveMainCharacterController()
@date 20130112 by NDark . commnet. 
@date 20130120 by NDark 
. add class method GetUnitDataGUIAnimationManager()
. add class method GetBattleEventCamera()
. add class method GetBattleEventCameraFollower()
. add class method GetBattleEventCameraManager()
@date 20130121 by NDark . remove argument of class method GetGUI_ConversationTextObject()
@date 20130126 by NDark . add class member m_CurrentModeSelectSceneString.
@date 20130203 by NDark . add class method GetEnergyManipulator()
@date 20130204 by NDark
. add class member m_CustomActive 
. add class member m_CustomPrefabName
. add class member m_CustomDefaultPrefabName
. add class member m_CustomUnitDataName
. add class member m_CustomDefaultUnitDataName
. add class method ResetCustomData
@date 20130205 by NDark . comment.
@date 20130206 by NDark 
. remove class member m_CurrentModeSelectSceneString
. add class member m_InformationSequenceFilepath
. add class member m_InformationSceneEnd
. add class member m_InformationSequenceIsInitialized
. add class member m_InformationSequenceIndex
. add class member m_InformationSceneNames
. add class method InitializeInformationSequence()
. add class method TryLoadInformationNext()


*/
// #define DEBUG
using UnityEngine;
using System.Collections.Generic;

public static class GlobalSingleton 
{
	public static string m_AcknowledgementGUIOBjectName = "" ; // 選關之後是否要開啟感謝的GUIObjectName
	public static string m_LevelString = "" ; // 用來記錄目前要讀取的關卡關鍵字串,由選關場景寫入,戰鬥場景讀出
	
	// custom ship
	public static bool m_CustomActive = false ; 
	public static string m_CustomPrefabName = "Template_MainCharacter_Enterprise01" ;
	public static string m_CustomDefaultPrefabName = "Template_MainCharacter_Enterprise01" ;
	public static string m_CustomUnitDataName = "UnitDataTemplate_MainCharacter_Enterprise09" ;
	public static string m_CustomDefaultUnitDataName = "UnitDataTemplate_MainCharacter_Enterprise09" ;
	
	// information sequece
	public static string m_InformationSequenceFilepath = "InformationSequence.txt" ;
	public static string m_InformationSceneEnd = "Scene_Warning" ;
	public static bool m_InformationSequenceIsInitialized = false ;
	public static int m_InformationSequenceIndex = 0 ;
	public static List<string> m_InformationSceneNames = new List<string>() ;
	public static void InitializeInformationSequence()
	{
		string content = LoadDataToXML.LoadToString( m_InformationSequenceFilepath , false ) ;
		string [] strVec = ConstName.GetSplitVec( content , '\n' ) ;
		m_InformationSequenceIndex = 0 ;
		m_InformationSceneNames.Clear() ;
		for( int i = 0 ; i < strVec.Length ; ++i )
		{
			
			string strAdd = strVec[ i ].Trim() ;
			if( strAdd.Length > 0 )
			{
				// Debug.Log( strAdd ) ;
				m_InformationSceneNames.Add( strAdd ) ;
			}
		}
		m_InformationSequenceIsInitialized = true ;
	}
	
	public static void TryLoadInformationNext()
	{
		if( false == m_InformationSequenceIsInitialized )
			InitializeInformationSequence() ;

		if( m_InformationSequenceIndex >= m_InformationSceneNames.Count )
		{
			if( 0 == m_InformationSceneEnd.Length )
				Application.LoadLevel( "Scene_Warning" ) ;	
			else
				Application.LoadLevel( GlobalSingleton.m_InformationSceneEnd ) ;
		}
		else
		{
			// "Scene_Recruitment"
			string sceneName = m_InformationSceneNames[ m_InformationSequenceIndex ] ;			
			// Debug.Log( "m_InformationSequenceIndex=" + m_InformationSequenceIndex ) ;
			Application.LoadLevel( sceneName ) ;
			++m_InformationSequenceIndex ;
		}
	}
	
	public static void ResetCustomData()
	{
		m_CustomActive = false ;
		m_CustomPrefabName = m_CustomDefaultPrefabName ;
		m_CustomUnitDataName = m_CustomDefaultUnitDataName ;
	}
	
	// singleton
	private static NamedObject m_GlobalSingletonObj = new NamedObject( ConstName.GlobalSingletonObjectName ) ;
	public static GameObject GetGlobalSingletonObj()
	{
		GameObject ret = m_GlobalSingletonObj.Obj ;
		if( null == ret )
		{
			Debug.Log( "GetGlobalSingletonObj() : ( null == ret )" ) ;
		}
		return ret ;
	}
	

	
	private static NamedObject m_BackgroundObject = new NamedObject( ConstName.BackgroundObjectName ) ;
	public static BackgroundObjInitialization GetBackgroundInit()
	{
		BackgroundObjInitialization ret = null ;
		GameObject bObj = m_BackgroundObject.Obj ;
		if( null == bObj )
		{
			Debug.Log( "GetBackgroundInit() : ( null == bObj )" ) ;
			return ret ;
		}
		ret = bObj.GetComponent<BackgroundObjInitialization>() ;
		if( null == ret )
		{
			Debug.Log( "GetBackgroundInit() : ( null == ret )" ) ;
		}
		return ret ;
	}
	
	public static GUIUpdate GetGUIUpdateComponent()
	{
		GameObject GlobalSingletonObj = GetGlobalSingletonObj() ;
		GUIUpdate guiUpdate = GlobalSingletonObj.GetComponent<GUIUpdate>() ;
		if( null == guiUpdate )
		{
			Debug.Log( "GetGUIUpdateComponent() : ( null == guiUpdate )" ) ;
		}
		return guiUpdate ;
	}
	
	public static ReloadAnimationManager GetReloadAnimationManager()
	{
		GameObject GlobalSingletonObj = GetGlobalSingletonObj() ;
		Transform trans = GlobalSingletonObj.transform.FindChild( "ReloadCompletenessAnimator" ) ;
		ReloadAnimationManager animator = null ;
		if( null == trans )
		{
			Debug.Log( "GetReloadAnimationManager() : ( null == ReloadCompletenessAnimator )" ) ;
			return animator ;
		}
		
		animator = trans.gameObject.GetComponent<ReloadAnimationManager>() ;
		if( null == animator )
		{
			Debug.Log( "GetReloadAnimationManager() : ( null == GUI_ReloadCompletenessAnimator )" ) ;
		}
		return animator ;
	}	
	
	public static MainUpdate GetMainUpdateComponent()
	{
		GameObject GlobalSingletonObj = GetGlobalSingletonObj() ;
		if( null == GlobalSingletonObj )
			return null ;
		MainUpdate mainUpdate = GlobalSingletonObj.GetComponent<MainUpdate>() ;
		if( null == mainUpdate )
		{
			Debug.Log( "GetMainUpdateComponent() : ( null == mainUpdate )" ) ;
		}
		return mainUpdate ;		
	}
	
	public static EnemyGenerator GetEnemyGeneratorComponent()
	{
		GameObject GlobalSingletonObj = GetGlobalSingletonObj() ;
		EnemyGenerator enemyGenerator = GlobalSingletonObj.GetComponent<EnemyGenerator>() ;
		if( null == enemyGenerator )
		{
			Debug.Log( "GetEnemyGeneratorComponent() : ( null == enemyGenerator )" ) ;
		}
		return enemyGenerator ;		
	}	
	
	public static GameObject GetUsualEventManagerObject()
	{
		GameObject GlobalSingletonObj = GetGlobalSingletonObj() ;
		Transform trans = GlobalSingletonObj.transform.FindChild( "UsualEventManagerObject" ) ;
		GameObject usualEventManagerObject = null ;
		if( null != trans )
		{
			usualEventManagerObject = trans.gameObject ;
		}
		else
		{
			Debug.Log( "GetUsualEventManagerObject() : ( null != trans )" ) ;
		}
		return usualEventManagerObject ;		
	}	
	public static UsualEventManager GetUsualEventManagerComponent()
	{
		GameObject usualEventManagerObject = GetUsualEventManagerObject() ;
		if( null == usualEventManagerObject )
			return null ;
		
		UsualEventManager usualEventManager = usualEventManagerObject.GetComponent<UsualEventManager>() ;
		if( null == usualEventManager )
		{
			Debug.Log( "GetUsualEventManagerComponent() : ( null == usualEventManager )" ) ;
		}
		return usualEventManager ;		
	}

	public static TutorialEvent GetTutorialEvent()
	{
		GameObject globalSingletonObj = GlobalSingleton.GetGlobalSingletonObj() ;
		if( null == globalSingletonObj )
			return null ;
		
		TutorialEvent ret = globalSingletonObj.GetComponent<TutorialEvent>() ;
		if( null == ret )
		{
			Debug.Log( "GetTutorialEvent() : ( null == ret )" ) ;
		}
		return ret ;		
	}
	
	public static VictoryEventManager GetVictoryEventManager()
	{
		GameObject GlobalSingletonObj = GetGlobalSingletonObj() ;
		VictoryEventManager eventManager = GlobalSingletonObj.GetComponentInChildren<VictoryEventManager>() ;
		if( null == eventManager )
		{
			Debug.Log( "GetVictoryEventManager() : ( null == eventManager )" ) ;
		}
		return eventManager ;		
	}	
	
	
	public static LevelGenerator GetLevelGeneratorComponent()
	{
		GameObject GlobalSingletonObj = GetGlobalSingletonObj() ;
		LevelGenerator levelGenerator = GlobalSingletonObj.GetComponent<LevelGenerator>() ;
		if( null == levelGenerator )
		{
			Debug.Log( "GetLevelGeneratorComponent() : ( null == levelGenerator )" ) ;
		}
		return levelGenerator ;			
	}
	
	public static GameObject GetBackgroundMusicObject()
	{
		GameObject GlobalSingletonObj = GetGlobalSingletonObj() ;
		Transform trans = GlobalSingletonObj.transform.FindChild( "BackgroundMusicObject" ) ;
		GameObject backgroundMusicObject = null ;
		if( null != trans )
		{
			backgroundMusicObject = trans.gameObject ;
		}
		else
		{
			Debug.Log( "GetBackgroundMusicObject() : ( null == trans )" ) ;
		}
		return backgroundMusicObject ;		
	}	
	
	public static AutoPlayMachine GetAutoPlayMachine()
	{
		AutoPlayMachine ret = null ;
		GameObject globalObj = GlobalSingleton.GetGlobalSingletonObj() ;
		if( null != globalObj )
		{
			ret = globalObj.GetComponent<AutoPlayMachine>() ;
			if( null == ret )
			{
				Debug.Log( "GetAutoPlayMachine() : ( null == ret )" ) ;
			}
		}
		return ret ;
	}
	public static bool IsInAutoPlay()
	{
		bool ret = false ;
		AutoPlayMachine autoPlayMachine = GetAutoPlayMachine() ;
		if( null != autoPlayMachine )
		{
			ret = autoPlayMachine.m_Active ;
		}
		return ret ;
	}	
	
	public static BattleScoreManager GetBattleScoreManager()
	{
		BattleScoreManager ret = null ;
		GameObject globalObj = GetGlobalSingletonObj() ;
		if( null != globalObj )
		{
			ret = globalObj.GetComponent<BattleScoreManager>() ;
		}
		return ret ;
	}
	
	public static ConversationManager GetConversationManager()
	{
		ConversationManager ret = null ;
		GameObject globalObj = GetGlobalSingletonObj() ;
		if( null != globalObj )
		{
			ret = globalObj.GetComponentInChildren<ConversationManager>() ;
		}
		return ret ;
	}
	
	public static UnitDataGUIAnimationManager GetUnitDataGUIAnimationManager()
	{
		UnitDataGUIAnimationManager ret = null ;
		GameObject globalObj = GetGlobalSingletonObj() ;
		if( null != globalObj )
		{
			ret = globalObj.GetComponentInChildren<UnitDataGUIAnimationManager>() ;
		}
		return ret ;
	}		
	
	
	// main character
	private static NamedObject m_MainCharacter = new NamedObject( ConstName.MainCharacterObjectName ) ;
	public static GameObject GetMainCharacterObj()
	{
		GameObject ret = null ;
		if( null == m_MainCharacter.GetObj() )
		{
			m_MainCharacter.Obj = GameObject.FindGameObjectWithTag( ConstName.MainCharacterObjectTag ) ;// more efficiency
		}
		ret = m_MainCharacter.Obj ;
		
		if( null == ret )
		{
			Debug.Log( "GetMainCharacterObj() null == ret" ) ;
		}
		return ret;
	}
		
	public static MainCharacterController GetMainCharacterControllerComponent()
	{
		GameObject mainCharacterObj = GetMainCharacterObj() ;
		if( null == mainCharacterObj )
		{
			Debug.Log( "GetMainCharacterControllerComponent() : ( null == mainCharacterObj )" ) ;
			return null ;
		}
		MainCharacterController mainCharacterController = mainCharacterObj.GetComponent<MainCharacterController>() ;
		if( null == mainCharacterController )
		{
			Debug.Log( "GetMainCharacterControllerComponent() : ( null == mainCharacterController )" ) ;
		}
		return mainCharacterController ;		
	}
	public static void ActiveMainCharacterController( bool _Active )
	{
		MainCharacterController controller = GetMainCharacterControllerComponent() ;
		if( null != controller )
		{
			controller.enabled = _Active ;
		}
	}
	public static bool GetMainCharacterControllerEnbale()
	{
		MainCharacterController controller = GetMainCharacterControllerComponent() ;
		if( null != controller )
		{
			return controller.enabled ;
		}
		return false ;
	}	
	
	public static void TellMainCharacterNotToTriggerOtherClick()
	{
		MainCharacterController control = GetMainCharacterControllerComponent() ;
		if( null != control )
		{
			control.SetClickOnNoMoveFuncThisFrame( true ) ;
		}
	}
	
	public static UnitSelectionSystem GetMainCharacterSelectionSystem()
	{
		GameObject mainCharacterObj = GetMainCharacterObj() ;
		if( null == mainCharacterObj )
		{
			Debug.Log( "GetMainCharacterSelectionSystem() : ( null == mainCharacterObj )" ) ;
			return null ;
		}
		UnitSelectionSystem selectSys = mainCharacterObj.GetComponent<UnitSelectionSystem>() ;
		if( null == selectSys )
		{
			Debug.Log( "GetMainCharacterSelectionSystem() : ( null == selectSys )" ) ;
		}
		return selectSys ;		
	}	
	
	public static void CloseAudioOfMainCharacter()
	{
		// 玩家身上的聲音
		GameObject mainChar = GetMainCharacterObj() ;
		if( null != mainChar )
		{
			AudioSource [] audios = mainChar.GetComponentsInChildren<AudioSource>() ;
			foreach( AudioSource audio in audios )
			{
				audio.Stop() ;
			}			
		}				
	}
	
	public static BattleEventCameraManager GetBattleEventCameraManager()
	{
		GameObject globalSingleton = GetGlobalSingletonObj() ;
		BattleEventCameraManager ret = null ;
		if( null != globalSingleton )
		{
			ret = globalSingleton.GetComponent<BattleEventCameraManager>() ;
		}
		return ret;
	}	
	
	private static NamedObject m_BattleEventCamera = new NamedObject( "BattleEventCamera" ) ;
	public static Camera GetBattleEventCamera()
	{
		GameObject cameraObj = m_BattleEventCamera.Obj ;
		Camera ret = null ;
		if( null != cameraObj )
		{
			ret = cameraObj.GetComponent<Camera>() ;			
		}
		return ret;
	}
	public static CameraFollowUnit GetBattleEventCameraFollower()
	{
		GameObject cameraObj = m_BattleEventCamera.Obj ;
		CameraFollowUnit ret = null ;
		if( null != cameraObj )
		{
			ret = cameraObj.GetComponent<CameraFollowUnit>() ;			
		}
		return ret;
	}	
	
	// GUI
	private static NamedObject m_GUICamera = new NamedObject( "GUICamera" ) ;
	public static GameObject GetGUICameraObj()
	{
		GameObject ret = m_GUICamera.Obj ;
		if( null == ret )
		{
			Debug.Log( "GetGUICameraObj() : null == ret" ) ;
		}
		return ret;
	}
	
	public static GameObject GetGUI_LevelObjectiveSwitcher()
	{
		GameObject guiCamera = GlobalSingleton.GetGUICameraObj() ;
		if( null == guiCamera )
			return null ;
		Transform trans = guiCamera.transform.FindChild( "GUI_LevelObjectiveSwitcher" ) ;
		if( null == trans )
			return null ;
		
		GameObject ret = trans.gameObject ;
		return ret ;
	}	
	public static ClickOnGUI_SwitchGUIObject GetSwitchLevelObjective()
	{
		GameObject guiCamera = GlobalSingleton.GetGUICameraObj() ;
		if( null == guiCamera )
			return null ;
		Transform trans = guiCamera.transform.FindChild( "GUI_LevelObjectiveSwitcher" ) ;
		if( null == trans )
			return null ;
		
		GameObject switcher = trans.gameObject ;
		ClickOnGUI_SwitchGUIObject ret = switcher.GetComponent<ClickOnGUI_SwitchGUIObject>() ;
		return ret ;
	}
	public static ClickOnGUI_SwitchMiniMap GetSwitchMiniMap()
	{
		GameObject guiCamera = GlobalSingleton.GetGUICameraObj() ;
		if( null == guiCamera )
			return null ;
		Transform trans = guiCamera.transform.FindChild( "GUI_MiniMapSwitcher" ) ;
		if( null == trans )
			return null ;
		
		GameObject switcher = trans.gameObject ;
		ClickOnGUI_SwitchMiniMap ret = switcher.GetComponent<ClickOnGUI_SwitchMiniMap>() ;
		return ret ;
	}
	
	public static ClickOnGUI_SwitchEnergyManipulator GetEnergyManipulatorSwitcher()
	{
		GameObject guiCamera = GlobalSingleton.GetGUICameraObj() ;
		if( null == guiCamera )
			return null ;
		Transform trans = guiCamera.transform.FindChild( "GUI_EnergyManipulatorSwitcher" ) ;
		if( null == trans )
			return null ;
		
		GameObject switcher = trans.gameObject ;
		ClickOnGUI_SwitchEnergyManipulator ret = switcher.GetComponent<ClickOnGUI_SwitchEnergyManipulator>() ;
		return ret ;
	}		
	public static GameObject GetEnergyManipulatorParentObj()
	{
		GameObject ret = null ;
		GameObject guiCamera = GlobalSingleton.GetGUICameraObj() ;
		if( null != guiCamera )
		{
			Transform trans = guiCamera.transform.FindChild( "GUI_EnergyManipulator" ) ;
			if( null != trans )
				ret = trans.gameObject ;
		}
		return ret ;
	}
	public static GUI_EnergyManipulator GetEnergyManipulator()
	{
		GUI_EnergyManipulator ret = null ;
		GameObject guiCamera = GlobalSingleton.GetGUICameraObj() ;
		if( null != guiCamera )
		{
			Transform trans = guiCamera.transform.FindChild( "GUI_EnergyManipulator" ) ;
			if( null != trans )
			{
				ret = trans.gameObject.GetComponent<GUI_EnergyManipulator>() ;
			}
		}
		return ret ;
	}	
	
	public static CameraFollowMainCharacter GetCameraFollowMainCharacter()
	{
		CameraFollowMainCharacter ret = Camera.mainCamera.GetComponent<CameraFollowMainCharacter>() ;
		if( null == ret )
		{
			Debug.Log( "GetCameraFollowMainCharacter() : null == ret" ) ;
		}
		return ret;
	}
	
	public static GameObject GetMiniMapCameraObj()
	{
		Transform trans = Camera.mainCamera.transform.FindChild( "MiniMapCamera" ) ;
		if( null == trans )
			return null ;
		GameObject ret = trans.gameObject ;
		if( null == ret )
		{
			Debug.Log( "GetMiniMapCameraObj() : null == ret" ) ;
		}
		return ret;
	}	
	public static DrawMiniMap GetDrawMiniMap()
	{
		GameObject miniMapCamera = GetMiniMapCameraObj() ;
		if( null == miniMapCamera )
			return null ;
		DrawMiniMap ret = miniMapCamera.GetComponent<DrawMiniMap>() ;
		if( null == ret )
		{
			Debug.Log( "GetDrawMiniMap() : null == ret" ) ;
		}
		return ret;
	}
	
	private static NamedObject m_GUI_UnitDataSelection_UnActive = new NamedObject( ConstName.GUIUnitDataSelection_UnActive ) ;
	public static GameObject GetGUIUnitDataSelection_UnActive()
	{
		GameObject ret = m_GUI_UnitDataSelection_UnActive.Obj ;
		if( null == ret )
		{
			Debug.Log( "GetGUIUnitDataSelection_UnActive() : null == ret" ) ;
		}
		return ret;
	}

	
	private static NamedObject m_MessageQueueManagerObj = new NamedObject( "MessageQueueManagerObj" ) ;
	public static MessageQueueManager GetMessageQueueManager()
	{
		MessageQueueManager ret = null ;
		if( null == m_MessageQueueManagerObj.Obj )
			return ret ;
		ret = m_MessageQueueManagerObj.Obj.GetComponent<MessageQueueManager>() ;
		if( null == ret )
		{
			Debug.Log( "GetMessageQueueManager() : null == ret" ) ;
		}
		return ret;
	}	

	public static GameObject GetGUI_BattleMenuSwitcher()
	{
		GameObject guiCamera = GlobalSingleton.GetGUICameraObj() ;
		if( null == guiCamera )
			return null ;
		Transform trans = guiCamera.transform.FindChild( "GUI_BattleMenuSwitcher" ) ;
		if( null == trans )
			return null ;
		
		GameObject ret = trans.gameObject ;
		return ret ;
	}	
	
	// GUI Conversation
	private static NamedObject m_GUI_ConversationObject = new NamedObject() ;
	public static GameObject GetGUI_ConversationObj()
	{
		if( null == m_GUI_ConversationObject.GetObj() )
		{
			GameObject GUICameraObj = GetGUICameraObj() ;
			if( null != GUICameraObj )
			{
				Transform trans = GUICameraObj.transform.FindChild( "GUI_Conversation" ) ;
				if( null != trans )
				{
					m_GUI_ConversationObject.Setup( trans.gameObject.name , trans.gameObject ) ;
				}
			}
		}
		return m_GUI_ConversationObject.Obj ;
	}
	
	private static NamedObject m_GUI_ConversationText = new NamedObject() ;
	public static GameObject GetGUI_ConversationTextObject()
	{
		GameObject ret = null ;
		if( null != m_GUI_ConversationText.GetObj() )
			ret = m_GUI_ConversationText.Obj ;
		else
		{
			GameObject guiConversationObj = GetGUI_ConversationObj() ;
			if( null != guiConversationObj )
			{
				Transform trans = guiConversationObj.transform.FindChild( "GUI_Conversation_Text" ) ;
				if( null != trans )
				{
					ret = trans.gameObject ;
					m_GUI_ConversationText.Setup( ret.name , ret ) ;
				}
				else
				{
					Debug.Log( "GetGUI_ConversationText() : ( null == trans )" ) ;
				}
			}
		}
		return ret ;		
	}	
	
	private static NamedObject m_GUI_ConversationNextObject = new NamedObject() ;
	public static GameObject GetGUI_ConversationNextObject()
	{
		if( null == m_GUI_ConversationNextObject.GetObj() )
		{
			GameObject guiConversationObj = GetGUI_ConversationObj() ;
			if( null != guiConversationObj )
			{
				Transform trans = guiConversationObj.transform.FindChild( "GUI_Conversation_Next" ) ;
				if( null != trans )
				{
					m_GUI_ConversationNextObject.Setup( trans.gameObject.name , trans.gameObject ) ;
				}
			}
		}		
		return m_GUI_ConversationNextObject.Obj ;
	}		
	public static ClickOnGUI_Record GetGUI_ConversationNextClickRecord()
	{
		GameObject guiConversationNextObj = GetGUI_ConversationNextObject() ;
		ClickOnGUI_Record ret = null ;
		if( null != guiConversationNextObj )
		{
			ret = guiConversationNextObj.GetComponent<ClickOnGUI_Record>() ;
		}
		return ret ;
	}	

	public static GameObject GetGUI_ConversationChildObject( string _Name )
	{
		GameObject ret = null ;
		GameObject guiConversationObj = GetGUI_ConversationObj() ;
		if( null != guiConversationObj )
		{
			Transform trans = guiConversationObj.transform.FindChild( _Name ) ;
			if( null != trans )
			{
				ret = trans.gameObject ;
			}
		}
		return ret ;
	}			
		
	// other object
	
	static public UnitData GetUnitData( string _ObjectName )
	{
		UnitData ret = null ;
		GameObject obj = GameObject.Find( _ObjectName ) ;
		if( null != obj )
			ret = obj.GetComponent<UnitData>() ;
		else
		{
			// Debug.Log( "GetUnitData() : ( null == obj )" + _ObjectName ) ;
		}
		return ret ;
	}
	
	public static GameObject GetParentObject( GameObject _Obj )
	{
		GameObject ret = null ;
		if( null != _Obj.transform.parent )
			ret = _Obj.transform.parent.gameObject ;
		return ret ;
	}	
	
	public static string GetParentName( GameObject _Obj )
	{
		string ret = "" ;
		if( null != _Obj.transform.parent )
			ret = _Obj.transform.parent.gameObject.name ;
		return ret ;
	}
	

}
