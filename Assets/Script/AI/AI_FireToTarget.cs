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
@file AI_FireToTarget.cs
@author NDark
 
# 繼承 AI_FireToClosestEnemy
# 參數
## 需指定目標 TargetName
## 發射後等待時間 WaitForReloadSec
# 其中會檢查 AI單位與目標單位的 方向 來決定要移動還是開火.
# KeepRetrieveData() 一開始在未啟動時,會有一段時間檢查AI單位的生命狀態,才開始AI的行為.
## m_MaximumRetrieveDataTime
# 攻擊開火 FireWeapon()
## 無法開火回到移動到目標

@date 20121108 by NDark . refine code.
@date 20121114 by NDark 
. add class member m_WaitForReload
. remove some relation code at Update() 
@date 20121116 by NDark . add alive checking at Update()
@date 20121203 by NDark . comment.
@date 20121205 by NDark 
. add class member m_MaximumWeaponRange.檢查目前與敵人的距離.來決定是否要繼續前進
. add enum CheckUnitData of AI_Fire_State
. refactor 
@date 20121210 by NDark . refactor.
@date 20121218 by NDark . refactor. 
@date 20121219 by NDark . add code of AutoPlayMachine at FireWeapon() 未來必須將判斷寫在CheckUnitData內
@date 20130204 by NDark . refine code.

*/
using UnityEngine;

public class AI_FireToTarget : AI_FireToClosestEnemy 
{
	// 一開始在未啟動時,會有一段時間檢查AI單位的生命狀態,才開始AI的行為.
	protected CountDownTrigger m_MaximumRetrieveDataTime = new CountDownTrigger( 1.0f ) ;
	
	// param
	// "WaitForReloadSec"	
	// "TargetName"
	
	// Use this for initialization
	void Start () 
	{
		// 指定對象
		this.SetState( AI_Fire_State.UnActive ) ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// check if the AI object is still alive or not
		UnitData unitData = null ;
		UnitWeaponSystem weaponSys = null ;
		if( false == CheckAbleRunAI( out unitData , 
									 out weaponSys ) ) 
		{
			return ;
		}
		
		
		m_State.Update() ;
		switch( (AI_Fire_State) m_State.state )
		{
		case AI_Fire_State.UnActive :
			this.SetState( AI_Fire_State.CheckUnitData ) ;
			break ;
			
		case AI_Fire_State.CheckUnitData :
			
			if( m_State.IsFirstTime() )
				m_MaximumRetrieveDataTime.Rewind() ;
			
			KeepRetrieveData() ;
			
			break ;
			
		case AI_Fire_State.MoveToTarget :
			
			if( false == CheckTarget( m_TargetNow ) )
			{
				this.SetState( AI_Fire_State.End ) ;
				break ;
			}
			
			CheckWeaponAndMoveToTarget( unitData , 
										weaponSys , 
										m_TargetNow.Obj.transform.position ) ;
			
			break ;
			
		case AI_Fire_State.StopAndFireToTarget :
			
			if( false == CheckTarget( m_TargetNow ) )
			{
				this.SetState( AI_Fire_State.End );
				break ;
			}
			
			if( true == m_State.IsFirstTime() )
			{
				unitData.AllStop() ;
			}			
			
			FireWeapon( weaponSys , m_TargetNow ) ;

			break ;
			
		case AI_Fire_State.WaitForReload :
			this.CheckReloadAndBackTo( AI_Fire_State.MoveToTarget ) ;
			break ;
			
		case AI_Fire_State.End :
			break ;			
		}
	}

	protected virtual void KeepRetrieveData()
	{
		if( true == m_MaximumRetrieveDataTime.IsCountDownToZero() )
		{
			// 無法完成任務,結束
			this.SetState( AI_Fire_State.End ) ;
		}
		else
		{
			if( true == RetrieveData() )
			{
				this.SetState( AI_Fire_State.MoveToTarget );
			}
		}
	}
	
	protected override bool RetrieveData()
	{
		bool ret = false ;
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		UnitWeaponSystem weaponSys = this.gameObject.GetComponent<UnitWeaponSystem>() ;
		
		if( null != unitData && 
			null != weaponSys )
		{
			RetrieveDataWeaponKeyword( unitData ) ;
			RetrieveDataMaximumWeaponRange( weaponSys , ref m_WeaponKeywords ) ;
			
			RetrieveParam( unitData , "WaitForReloadSec" , ref m_WaitForReloadSec ) ;
			
			string TargetName = "" ;
			if( true == RetrieveParam( unitData , "TargetName" , ref TargetName ) )
			{
				m_TargetNow.Name = TargetName ;
				ret = true ;
			}
		}			
		return ret ;
	}		


	protected override void FireWeapon( UnitWeaponSystem _weaponSys ,
							  			NamedObject _TargetUnit )
	{
		if( true == GlobalSingleton.GetGlobalSingletonObj().GetComponent<AutoPlayMachine>().m_Active )
			return ;
		
		string weaponComponent = "" ;
		// check phaser
		weaponComponent = _weaponSys.FindAbleWeaponComponent( RandomAWeaponKeyword() , 
															 _TargetUnit.Name ) ;
		
		if( 0 == weaponComponent.Length )
		{
			// Debug.Log( "FireWeapon() no available weapon" ) ;
			this.SetState( AI_Fire_State.MoveToTarget ) ;
			return ;
		}
		
		// fire
		if( true == _weaponSys.ActiveWeapon( weaponComponent , 
											 _TargetUnit ,
											 ConstName.UnitDataComponentUnitIntagraty ) )
		{
			this.SetState( AI_Fire_State.WaitForReload ) ;
		}
		
	}
}
