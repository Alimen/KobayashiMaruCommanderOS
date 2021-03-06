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
@file Conversation.cs
@author NDark
@date 20130103 file started.
@date 20130107 by NDark . add GUI_Conversation_TextBackground at EnableDialog()
@date 20130112 by NDark . re-facotr and comment.
@date 20130121 by NDark
. add class member m_Index of TalkStr
. remove class member m_EnglishStr of TalkStr
. remove class member m_ChineseStr of TalkStr
. remove class method TalksHasText()
@date 20130205 by NDark . comment

*/
// #define DEBUG

using UnityEngine;
using System.Collections.Generic;

/*
# 字串的指定編號.
*/
[System.Serializable]
public class TalkStr
{
	public int m_Index = 0 ;
	
	public TalkStr()
	{
	}
	
	public TalkStr( TalkStr _src )
	{
		m_Index = _src.m_Index ;
	}
}

/*
# 大頭像 指定大頭像的GUI物件名稱(左邊或右邊)以及要使用的貼圖名稱
*/
[System.Serializable]
public class Potrait
{
	public string m_PotraitName = "" ;
	public string m_TextureName = "" ;
	
	public Potrait()
	{
		
	}
	
	public Potrait( Potrait _src )
	{
		m_PotraitName = _src.m_PotraitName ;
		m_TextureName = _src.m_TextureName ;
	}
}

/*
# 對話包含
## 複數個大頭像
## 複數個字串
# EnableDialog() 指定大頭像及各顯示介面物件
# 呼叫 PlayNext() 依據對話的流程依序顯示各字串直到結束為止
# IsFinished() 檢查是否已經結束
*/
[System.Serializable]
public class Conversation 
{
	public List<Potrait> m_Potrits = new List<Potrait>() ;
	public List<TalkStr> m_Talks = new List<TalkStr>() ;
	public int m_CurrentIndex = 0 ;
	Dictionary<string,GameObject> m_GUIObjectListShare = null ;
	
	public bool IsFinished() 
	{
		bool ret = ( m_CurrentIndex >= m_Talks.Count ) ;
#if DEBUG
		Debug.Log( "Conversation::IsFinished() " + ret ) ;
#endif
		return ret ;
	}
	
	public void PlayNext() 
	{
		if( true == IsFinished() )
			return ;
		
		
		TalkStr talkStr = m_Talks[ m_CurrentIndex ] ;
		
		int Index = talkStr.m_Index ;
		string DisplayStr = StrsManager.Get( Index ) ;
#if DEBUG
		Debug.Log( "Conversation::PlayNext() DisplayStr=" + DisplayStr ) ;
#endif			
		
		string key = "GUI_Conversation_Text" ;
		GameObject textObj = m_GUIObjectListShare[ key ] ;
		if( null != textObj )
		{
			GUIText guiText = textObj.GetComponent<GUIText>() ;
			if( null != guiText )
			{
				guiText.text = DisplayStr ;
			}
		}
		
		
		++m_CurrentIndex ;
#if DEBUG
		Debug.Log( "Conversation::PlayNext() m_CurrentIndex=" + m_CurrentIndex ) ;
#endif
	}
	
	// 指定大頭像及各顯示介面物件
	public void EnableDialog( ref Dictionary<string,GameObject> _GUIObjectList )
	{
		// 先關閉原本的
		foreach( GameObject obj in _GUIObjectList.Values )
		{
			ShowGUITexture.Show( obj , false , true , false ) ;
		}
		_GUIObjectList.Clear() ;
		
		// 檢查english text
		GameObject texObj = GlobalSingleton.GetGUI_ConversationTextObject() ;
		ShowGUITexture.Show( texObj , true , true , false ) ;
		_GUIObjectList[ texObj.name ] = texObj ;
		
		// buttons
		_GUIObjectList[ "GUI_Conversation_Next" ] = GlobalSingleton.GetGUI_ConversationNextObject() ;
		_GUIObjectList[ "GUI_Conversation_NextButtonBackground" ] = GlobalSingleton.GetGUI_ConversationChildObject( "GUI_Conversation_NextButtonBackground" ) ;
		_GUIObjectList[ "GUI_Conversation_TextBackground" ] = GlobalSingleton.GetGUI_ConversationChildObject( "GUI_Conversation_TextBackground" ) ;
		
		// potraits
		foreach( Potrait potrait in m_Potrits )
		{
			string objName = "GUI_Conversation_" +  potrait.m_PotraitName ;
			_GUIObjectList[ objName ] = GlobalSingleton.GetGUI_ConversationChildObject( objName ) ;
			if( null != _GUIObjectList[ objName ] )
			{
				GUITexture guiTexture = _GUIObjectList[ objName ].GetComponent<GUITexture>() ;
				if( null != guiTexture )
				{
					guiTexture.texture = ResourceLoad.LoadTexture( potrait.m_TextureName ) ;
				}
			}
		}
		
		m_GUIObjectListShare = _GUIObjectList ;
	}
	
	public Conversation()
	{
	}

	public Conversation( Conversation _src )
	{
		m_Talks = _src.m_Talks ;
		m_Potrits = _src.m_Potrits ;
		m_CurrentIndex = _src.m_CurrentIndex ;
		m_GUIObjectListShare = _src.m_GUIObjectListShare ;
	}	
	
	
}
