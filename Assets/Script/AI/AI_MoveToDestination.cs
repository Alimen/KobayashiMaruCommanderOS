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
@file AI_MoveToDestination.cs
@author NDark


# 繼承 AI_TargetTo 
# 移動到指定位置物件
# 參數 從 m_SupplementalVec 取得必要資訊
## 目標位置物件名稱 TargetName 
## 判斷是否已經抵達距離 judgeDistance 
# 前往目標 GoToDestination()

@date 20121127 file started.
@date 20121203 comment.
@date 20121210 by NDark . refactor.
@date 20121212 by NDark . add impulseRatio at GoToDestination()
@date 20121218 by NDark . refactor and comment.

*/
using UnityEngine;

public class AI_MoveToDestination : AI_TargetTo 
{
	// param
	// "TargetName"
	protected float m_JudgeDistance = 0.0f ; // "judgeDistance"
	
	// Use this for initialization
	void Start () 
	{
		SetState( AIBasicState.UnActive ) ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Debug.Log( m_State ) ;
		m_State.Update() ;
		switch( (AIBasicState)m_State.state )
		{
		case AIBasicState.UnActive :
			SetState( AIBasicState.Initialized ) ;			
			break ;
		case AIBasicState.Initialized :
			if( false == RetrieveData() )
			{
				Debug.Log( "false == RetrieveData()" + this.gameObject.name ) ;				
				SetState( AIBasicState.Closed ) ;
			}	
			else
				SetState( AIBasicState.Active ) ;				
			break ;
		case AIBasicState.Active :
			GoToDestination() ;
			break ;
		case AIBasicState.Closed :
			break ;			
		}
	}

	// 前往目的地
	private void GoToDestination()
	{
		Vector3 vecToTarget = Vector3.zero ;
		Vector3 norVecToTarget = Vector3.zero ;		
		float angleOfTarget = 0.0f ;
		float dotOfUp = 0.0f ;
		
		UnitData unitData = null ;
		if( false == CheckUnitData( out unitData ) )
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

			string IMPULSE_ENGINE_RATIO = ConstName.UnitDataComponentImpulseEngineRatio ;
			StandardParameter impulseRatio = null ;
			if( true == unitData.standardParameters.ContainsKey( IMPULSE_ENGINE_RATIO ) )
			{
				impulseRatio = unitData.standardParameters[ IMPULSE_ENGINE_RATIO ] ;
			}
			
			// Debug.Log( "vecToTarget.magnitude" + vecToTarget.magnitude ) ;
			if( vecToTarget.magnitude < m_JudgeDistance )
			{
				impulseRatio.now = 0 ;// stop
				SetState( AIBasicState.Closed ) ;				
			}
			else
			{
				impulseRatio.ToMax() ;
			}
		}
	}

	// 從 m_SupplementalVec 取得必要資訊
	protected override bool RetrieveData()
	{
		bool ret = false ;
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( null != unitData )
		{
			string TargetName = "" ;
			if( true == RetrieveParam( unitData , "TargetName" , ref TargetName ) )
			{
				m_Target.Name = TargetName ;
				
				ret = true ;
			}
			
			RetrieveParam( unitData , "judgeDistance" , ref m_JudgeDistance ) ;
		}
		return ret ;
	}
		
}
