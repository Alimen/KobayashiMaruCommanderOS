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
@file AI_TargetTo.cs
@author NDark

# 移動系的基礎AI
# 參數 
TargetName 目標名稱
# 主要分為兩個狀態 AI單位活著就是追擊 AI單位死亡就是停止
# 往目標移動 ChaseTarget()

@date 20121108 by NDark . refine code.
@date 20121203 by NDark . refind code.
@date 20121210 by NDark 
. refactor.
. remove enum AI_TargetTo_State
@date 20130106 by NDark . add class method ChangeTarget()
@date 20130119 by NDark . add SetState() at ChangeTarget().
@date 20130204 by NDark . comment.

*/
using UnityEngine;

public class AI_TargetTo : AI_Base 
{
	// parameter
	public NamedObject m_Target = new NamedObject() ; // TargetName
	
	public void ChangeTarget( NamedObject _Obj )
	{
		m_Target.Setup( _Obj ) ;
		this.SetState( AIBasicState.Active ) ;// 重新設定狀態
	}
	
	// Use this for initialization
	void Start () 
	{
		SetState( AIBasicState.UnActive ) ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		UnitData unitData = null ;
		if( false == CheckUnitData( out unitData ) )
			return ;
		
		m_State.Update() ;
		switch( (AIBasicState) m_State.state )
		{
		case AIBasicState.UnActive :
			SetState( AIBasicState.Initialized ) ;
			break ;
		case AIBasicState.Initialized :
			if( false == RetrieveData() )
			{
				Debug.Log( "false == RetrieveData()" + this.gameObject.name ) ;
			}			
			SetState( AIBasicState.Active ) ;			
			break ;			
		case AIBasicState.Active :
			ChaseTarget() ;
			break ;
		case AIBasicState.Closed :
			break ;			
		}
	}
	
	protected void ChaseTarget()
	{
		UnitData unitData = null ;
		Vector3 vecToTarget = Vector3.zero ;
		Vector3 norVecToTarget = Vector3.zero ;		
		float angleOfTarget = 0.0f ;
		float dotOfUp = 0.0f ;		
		
		if( false == CheckUnitData( out unitData ) ||
			null == m_Target.Obj )
		{
			SetState( AIBasicState.Closed ) ;			
			return ;
		}
		
		if( true == MathmaticFunc.FindUnitRelation( this.gameObject , 
													m_Target.Obj , 
													ref vecToTarget , 
													ref norVecToTarget , 
													ref angleOfTarget ,
													ref dotOfUp ) )
		{
			unitData.AngularRatioHeadTo( angleOfTarget ,
										 dotOfUp , 
										 0.1f ) ;
		}		
	}
	
	protected override bool RetrieveData()
	{
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( null != unitData )
		{
			string TargetName = "" ;
			if( true == RetrieveParam( unitData , "TargetName" , ref TargetName ) )
			{
				m_Target.Name = TargetName ;
				return true ;
			}
		}			
		return false ;
	}	
}
