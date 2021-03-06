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
@file AI_PowerUpAndReplaceAI.cs
@author NDark 

能源上線 並取代AI

# 繼承 AI_Base
# 參數
## 增加的 AI 名稱 addAIName
# 能源上線 PowerUp()
## 將各部件的能源拉到最高
## 新增AI
# 結束後摧毀自己

@date 20121212 file started.
@date 20121218 by NDark . refactor.

*/
using UnityEngine;

public class AI_PowerUpAndReplaceAI : AI_Base 
{
	// param
	string m_AddAIName = "" ;	// addAIName
	
	// Use this for initialization
	void Start () 
	{
		RetrieveData() ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		UnitData unitData = null ;
		if( false == CheckUnitData( out unitData ) )
			return ;
		PowerUp( unitData ) ;
		Destroy( this ) ;
	}
		
	protected override bool RetrieveData()
	{
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( null != unitData )
		{
			RetrieveParam( unitData , "addAIName" , ref m_AddAIName ) ;		
		}
		return true ;
	}
	
	void PowerUp( UnitData _unitData )
	{
		// Debug.Log( "PowerUp" ) ;
		if( null == _unitData )
			return ;
		foreach( UnitComponentData componentData in _unitData.componentMap.Values )
		{
			componentData.m_Energy.ToMax() ;
		}
		if( 0 != m_AddAIName.Length )
		{
			this.gameObject.AddComponent( m_AddAIName ) ;
		}
	}
}
