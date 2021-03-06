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
@file BattleScoreManager.cs
@author NDark
 
戰場結算管理器

# 掛在 GlobalSingleton 下
# 負責統計數種戰場結算的資訊 m_Scores
## AddScore() 增加統計數目
## 戰鬥勝負時被啟動
## 依照目前數值更新到文字上
## 各文字有依序的GUIObject m_ScoreGUIText

@date 20121225 by NDark
@date 20130113 by NDark . comment.
@date 20130123 by NDark . replace hardcoded string by StrsManager.

*/
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public enum ScoreType
{
	DestroyNum ,
	DamageSuffer ,
	ElapsedSec ,
}

public class BattleScoreManager : MonoBehaviour 
{

	private Dictionary<ScoreType,float> m_Scores = new Dictionary<ScoreType, float>() ;
	// List<InterpolateTable> m_ScoreTables = new List<InterpolateTable>() ;
	private BasicTrigger m_Trigger = new BasicTrigger() ;
	private Dictionary<ScoreType,NamedObject> m_ScoreGUIText = new Dictionary<ScoreType,NamedObject>() ;
	
	public void Active()
	{
		if( true == m_Trigger.IsReady() )
			m_Trigger.State = TriggerState.Active ;
	}
	
	public void AddScore( ScoreType _Type , float _Add )
	{
		m_Scores[ _Type ] += _Add ;
	}
	
	// Use this for initialization
	void Start () 
	{
		m_ScoreGUIText[ ScoreType.DestroyNum ] = new NamedObject( ConstName.CreateBattleScore_TextRowName( ScoreType.DestroyNum ) ) ;
		m_ScoreGUIText[ ScoreType.DamageSuffer ] = new NamedObject( ConstName.CreateBattleScore_TextRowName( ScoreType.DamageSuffer ) ) ;
		m_ScoreGUIText[ ScoreType.ElapsedSec ] = new NamedObject( ConstName.CreateBattleScore_TextRowName( ScoreType.ElapsedSec ) ) ;
		m_Scores[ ScoreType.DestroyNum ] = 0 ;
		m_Scores[ ScoreType.DamageSuffer ] = 0 ;
		m_Scores[ ScoreType.ElapsedSec ] = 0 ;
		
		m_Trigger.Initialize() ;
	}
	
	
	// Update is called once per frame
	void Update () 
	{
		switch( m_Trigger.State )
		{
		case TriggerState.UnActive :
			// 為啟動
			break ;
		case TriggerState.Active :
			// 將資料傳送到指定的文字上
			SendScoreData() ;
			m_Trigger.State = TriggerState.Closed ;
			break ;
		case TriggerState.Closed :
			// 結束
			break ;			
		}
	}
	
	void SendScoreData()
	{
		Dictionary<ScoreType,float>.Enumerator e = m_Scores.GetEnumerator() ;
		
		while( e.MoveNext() )
		{
			ScoreType type = e.Current.Key ;
			// Debug.Log( type ) ;
			float Value = e.Current.Value ;
			
			string Str = "" ;
			switch( type )
			{
			case ScoreType.DestroyNum :
				Str = StrsManager.Get( 1024 ) ;
				Str = Str.Replace( "%d" , Value.ToString() ) ;
				break ;
			case ScoreType.DamageSuffer :
				Str = StrsManager.Get( 1025 ) ;
				Str = Str.Replace( "%d" , Value.ToString() ) ;
				break ;
			case ScoreType.ElapsedSec :
				Str = StrsManager.Get( 1026 ) ;
				Str = Str.Replace( "%d" , Value.ToString() ) ;
				break ;				
			}
			GUIText guiText = m_ScoreGUIText[ type ].Obj.GetComponent<GUIText>() ;
			if( null != guiText )
			{
				guiText.text = Str ;
				guiText.material.color = Color.black ;
			}
			
			// 幾顆星
		}
	}
}
