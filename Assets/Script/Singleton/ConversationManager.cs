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
@file ConversationManager.cs
@author NDark

# 掛在 ConversationManagerObject
# ActiveConversationSet() 被呼叫啟動各對話集合
# m_Audio 每次更新發出音效 
# m_IsPlayConversation 目前是否啟動中
# m_PlayConversation 啟動的狀態
# EnableAllDialog() 檢查顯示目前的對話配置
# PlayNext() 播放下一個對話
# CheckIfPress() 檢查是否需要播放下一文字或對話
# CheckIsContinue() 檢查是否還有後續
# 如果對話系統正在播放時被啟動其他的對話集合，則使用串列來堆積，避免遺失訊號。
# DeActiveOtherSystem() 啟動時其他系統要關閉
# ResumeOtherSystem() 結束時回復其他系統的運作



@date 20130103 file started.
@date 20130107 by NDark
. add class member m_EnergyManipulatorIsOpen
. add code of close script at ActiveConversationSet()
. add code of enable script at Update()
. add disable of messageManager ActiveConversationSet()
@date 20130113 by NDark . comment.
@date 20130115 by NDark 
. refactor and comment.
. rename m_PlayConversation to m_ConversationCheckState
. remove class member m_IsPlayConversation
. add class method CheckContinueAndPlayNext()
. add class method DeActiveOtherSystem()
. add class method ResumeOtherSystem()
@date 20130126 by NDark
. add class member m_BattleEventCameraManagerDefaultEnable
. add code of BattleEventCameraManager at DeActiveOtherSystem()
. add code of BattleEventCameraManager at ResumeOtherSystem()

*/
// #define DEBUG

using UnityEngine;
using System.Collections.Generic;

public class ConversationManager : MonoBehaviour 
{
	public Dictionary<string , ConversationSet> m_ConversationSets = new Dictionary<string, ConversationSet>() ;
	
	private BasicTrigger m_ConversationCheckState = new BasicTrigger() ;
	private ConversationSet m_CurrentConversation = null ;
	private Queue<ConversationSet> m_WaitingConversationSets = new Queue<ConversationSet>() ;
	private Dictionary<string ,GameObject> m_GUIList = new Dictionary<string ,GameObject>() ;
	private float m_TimeScale = BaseDefine.NORMAL_SCALE_IN_TIME ;
	private AudioClip m_Audio = null ;
	private bool m_EnergyManipulatorIsOpen = false ;
	private bool m_MainCharacterDefaultEnable = false ;
	private bool m_BattleEventCameraManagerDefaultEnable = false ;
	
	public void ActiveConversationSet( string _Key )
	{
#if DEBUG		
		Debug.Log( "ConversationManager::ActiveConversationSet() _Key=" + _Key ) ;
#endif 
		
		if( false == m_ConversationSets.ContainsKey( _Key ) )
		{
			Debug.Log( "ActiveConversationSet() false == m_ConversationSets.ContainsKey" ) ;
			return ;
		}
		
		DeActiveOtherSystem() ;
		
		m_WaitingConversationSets.Enqueue( m_ConversationSets[ _Key ] ) ;
		
		CheckContinueAndPlayNext() ;
		
		m_ConversationCheckState.Active() ;
	}
	
	// Use this for initialization
	void Start () 
	{
		m_Audio = ResourceLoad.LoadAudio( "computerbeep_17" ) ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateConversationSystem() ;
	}
	
	private void UpdateConversationSystem()
	{

		switch( m_ConversationCheckState.State )
		{
		case TriggerState.UnActive :
			// not active
			break ;
			
		case TriggerState.Active :
			if( true == CheckIfPress() )
			{
				m_ConversationCheckState.Close() ;// 需檢查下一步或關閉
			}
			Time.timeScale = m_TimeScale ;
			break ;	
			
		case TriggerState.Closed :
			
			if( true == CheckIsContinue() )
			{
				PlayNext() ;
				m_ConversationCheckState.Active() ;// 回到繼續檢查
			}
			else
			{
				// 關閉系統
				EnableAllDialog( false ) ;
				m_ConversationCheckState.Initialize() ;
#if DEBUG		
				Debug.Log( "ConversationManager::UpdateConversationSystem() conversation is end." ) ;
#endif 
				ResumeOtherSystem() ;
			}
			Time.timeScale = m_TimeScale ;
			break ;
		}
		
	}
	
	private void EnableAllDialog( bool _Enable )
	{
		// Debug.Log( "EnableAllDialog() _Enable" + _Enable ) ;
		if( null == m_CurrentConversation )
			return ;
		
		// 根據conversation的目前 layout 啟動 dialog
		if( true == _Enable )
		{
			m_CurrentConversation.EnableDialog( ref m_GUIList ) ;// hide useless inside
			
			m_TimeScale = BaseDefine.FREEZE_SCALE_IN_TIME ;
			foreach( GameObject obj in m_GUIList.Values )
			{
				ShowGUITexture.Show( obj , true , true , false ) ;
			}
		}
		else
		{
			m_TimeScale = BaseDefine.NORMAL_SCALE_IN_TIME ;
			foreach( GameObject obj in m_GUIList.Values )
			{
				ShowGUITexture.Show( obj , false , true , false ) ;
			}
		}
	}
	
	private void PlayNext()
	{
		// 播放下一個 字串 對話 或是對話集
		// Debug.Log( "PlayNext() " ) ;
		
		if( null == m_CurrentConversation ||
			( null != m_CurrentConversation && 
			  true == m_CurrentConversation.IsFinished() ) 
			)
		{
			m_CurrentConversation = m_WaitingConversationSets.Dequeue() ;
			// Debug.Log( "PlayNext() m_CurrentConversation = m_WaitingConversationSets.Dequeue" ) ;
		}
		
		if( null == m_CurrentConversation )
		{
			Debug.Log( "PlayNext() null == m_CurrentConversation" ) ;
			return ;
		}
		
		if( false == m_CurrentConversation.IsFinished() )
		{
			EnableAllDialog( true ) ;
			m_CurrentConversation.PlayNext() ;
			if( null != m_Audio )
			{
				this.gameObject.audio.PlayOneShot( m_Audio ) ;
			}
		}
	}
	
	private bool CheckIfPress()
	{
		// 檢查按鈕是否有被按下
		ClickOnGUI_Record record = GlobalSingleton.GetGUI_ConversationNextClickRecord() ;
		if( null == record )
			return false ;
		bool ret = record.m_IsClick ;
		
		if( true == record.m_IsClick )
			record.m_IsClick = false ;// reset
		
		return ret ;
	}
	
	private bool CheckIsContinue()
	{
		// 檢查是否還有後續
		// Debug.Log( "CheckIsContinue() " ) ;
		bool ret = false ;
		
		if( null == m_CurrentConversation &&
			m_WaitingConversationSets.Count > 0 )
		{
			// Debug.Log( "CheckIsContinue() m_WaitingConversationSets.Count > 0" ) ;
			ret = true ;
		}
		else 
		{
			if( false == m_CurrentConversation.IsFinished() )
			{
				// Debug.Log( "CheckIsContinue() false == m_CurrentConversation.IsFinished()" ) ;
				ret = true ;
			}
			else if( m_WaitingConversationSets.Count > 0 )
			{
				// Debug.Log( "CheckIsContinue() m_WaitingConversationSets.Count > 0" ) ;
				ret = true ;
			}
		}
		
#if DEBUG		
		Debug.Log( "ConversationManager::CheckIsContinue() return " + ret ) ;
#endif 		
		return ret ;
	}
	
	private void CheckContinueAndPlayNext()
	{
		if( true == CheckIsContinue() )
		{
			PlayNext() ;
		}
	}
	
	private void DeActiveOtherSystem()
	{
		BattleEventCameraManager battleEventCameraManager = GlobalSingleton.GetBattleEventCameraManager() ;
		if( null != battleEventCameraManager )
		{
			battleEventCameraManager.Close() ;
			m_BattleEventCameraManagerDefaultEnable = battleEventCameraManager.enabled ;
			battleEventCameraManager.enabled = false ;
		}
		
		MessageQueueManager messageManager = GlobalSingleton.GetMessageQueueManager() ;
		if( null != messageManager )
		{
			messageManager.CloseMessageNow( null ) ;
			messageManager.enabled = false ;
		}
		
		ClickOnGUI_SwitchEnergyManipulator switcher = GlobalSingleton.GetEnergyManipulatorSwitcher() ;
		if( null != switcher )
		{
			GameObject energyManipulatorObject = GlobalSingleton.GetEnergyManipulatorParentObj() ;
			if( null != energyManipulatorObject )
			{
				GUITexture guiTexture = energyManipulatorObject.GetComponentInChildren<GUITexture>() ;
				if( null != guiTexture )
					m_EnergyManipulatorIsOpen = guiTexture.enabled ;
			}			
			switcher.EnableEnergyManipulator( false ) ;
		}
		
		GlobalSingleton.CloseAudioOfMainCharacter() ;
		
		m_MainCharacterDefaultEnable = GlobalSingleton.GetMainCharacterControllerEnbale() ;
		GlobalSingleton.ActiveMainCharacterController( false ) ;

	}
	
	private void ResumeOtherSystem()
	{
		BattleEventCameraManager battleEventCameraManager = GlobalSingleton.GetBattleEventCameraManager() ;
		if( null != battleEventCameraManager )
		{
			battleEventCameraManager.enabled = m_BattleEventCameraManagerDefaultEnable ;
		}
		
		MessageQueueManager messageManager = GlobalSingleton.GetMessageQueueManager() ;
		if( null != messageManager )
		{
			messageManager.enabled = true ;
		}					
		ClickOnGUI_SwitchEnergyManipulator switcher = GlobalSingleton.GetEnergyManipulatorSwitcher() ;
		if( null != switcher )
		{
			switcher.EnableEnergyManipulator( m_EnergyManipulatorIsOpen ) ;
		}	
		
		// 玩家控制
		GlobalSingleton.ActiveMainCharacterController( m_MainCharacterDefaultEnable ) ;		
	}
}
