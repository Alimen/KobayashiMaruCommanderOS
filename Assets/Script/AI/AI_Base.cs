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
@file AI_Base.cs
@author NDark

# AI的基本類別
# AI的狀態
# CheckUnitData() 檢查單位是否合理
# CheckWeaponSys() 檢查武器系統是否存在
# CheckTarget() 檢查目標是否合理
# RetrieveData() 取得本AI需要的資料
# RetrieveParam() 自 UnitData::m_SupplementalVec取得一筆參數

@date 20121210 file started.
@date 20121210 by NDark 
. add class method CheckUnitData()
. change return type of class methods 
. add enum AIBasicState
. implement class method RetrieveParam()
. add class method CheckTarget()
@date 20121213 by NDark . add checking of victory for AI at CheckUnitData()
@date 20121218 by NDark 
. comment.
. add class method SetState()
@date 20130131 by NDark . remove argument of class method CheckTarget()
@date 20130204 by NDark . refine code.

*/
// #define DEBUG
using UnityEngine;

public class AI_Base : MonoBehaviour 
{
	[System.Serializable]
	public enum AIBasicState
	{
		UnActive = 0 ,
		Initialized ,	// 初始化取得資訊
		Active ,  // 啟動中
		Closed , //已經結束
	}
	
	protected StateIndex m_State = new StateIndex() ;

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	protected void SetState( AIBasicState _Set )
	{
		m_State.state = (int) _Set ;
	}
	
	protected virtual bool CheckUnitData( out UnitData _unitData )
	{
		_unitData = this.gameObject.GetComponent<UnitData>() ;
		
		// 檢查是否勝利與失敗,否則不可執行AI		
		if( true == GlobalSingleton.GetVictoryEventManager().IsWinOrLose() )
			return false ;		
		
		return ( null != _unitData &&
				 true == _unitData.IsAlive() ) ;
	}
	
	protected virtual bool CheckWeaponSys( out UnitWeaponSystem _weapon )
	{
		_weapon = this.gameObject.GetComponent<UnitWeaponSystem>() ;
		return ( null != _weapon ) ;
	}
	
	protected virtual bool CheckTarget( UnitObject _TargetUnit )
	{
		return ( null != _TargetUnit &&
				 null != _TargetUnit.ObjUnitData &&
				 true == _TargetUnit.ObjUnitData.IsAlive()
				 ) ;
	}
		
	protected virtual bool RetrieveData()
	{
		return true ;
	}
	
	protected virtual bool RetrieveParam( UnitData _unitData , string _Label , ref float _Value )
	{
		if( true == _unitData.m_SupplementalVec.ContainsKey( _Label ) )
		{
			string RotateAngularSpeedStr = _unitData.m_SupplementalVec[ _Label ] ;
			float.TryParse( RotateAngularSpeedStr , out _Value ) ;
#if DEBUG			
			Debug.Log( "AI_Base::RetrieveParam() " + _Label + " " + _Value ) ;
#endif			
			return true ;
		}	
		return false ;
	}
	
	protected virtual bool RetrieveParam( UnitData _unitData , string _Label , ref string _Value )
	{
		if( true == _unitData.m_SupplementalVec.ContainsKey( _Label ) )
		{
			_Value = _unitData.m_SupplementalVec[ _Label ] ;
#if DEBUG			
			Debug.Log( "AI_Base::RetrieveParam() " + _Label + " " + _Value ) ;
#endif			
			return true ;
		}
		return false ;
	}
	
	protected virtual bool RetrieveParam( UnitData _unitData , string _Label , ref Vector3 _Value )
	{
		if( true == _unitData.m_SupplementalVec.ContainsKey( _Label ) )
		{
			string routeValueStr = _unitData.m_SupplementalVec[ _Label ] ;
			float x = 0.0f ;
			float y = 0.0f ;
			float z = 0.0f ;
			if( true == XMLParseLevelUtility.ParsePosition( routeValueStr , 
															ref x , 
															ref y , 
															ref z ) )
			{
				_Value = new Vector3( x , y , z ) ;
#if DEBUG			
				Debug.Log( "AI_Base::RetrieveParam() " + _Label + " " + _Value ) ;
#endif							
				return true ;
			}
		}
		return false ;
	}
	
}
