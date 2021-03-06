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
@file ConversationSet.cs
@author NDark

# 對話集合就是多個Conversation。
# 一個對話集合就是一段對話，以m_Key來標明索引。
# PlayNext() 依照索引依序播放各Conversation。
# EnableDialog() 顯示該對話的介面物件

@date 20130103 file started.
@date 20130112 by NDark . re-facotr and comment.

*/
// #define DEBUG

using UnityEngine;
using System.Collections.Generic;
	
[System.Serializable]
public class ConversationSet 
{
	public string m_Key = "" ;
	public List<Conversation> m_Conversations = new List<Conversation>() ;
	public int m_CurrentIndex = 0 ;
	
	public bool IsFinished() 
	{
		bool ret = ( m_CurrentIndex >= m_Conversations.Count ) ;
#if DEBUG		
		Debug.Log( "ConversationSet::IsFinished() ret=" + ret ) ;
#endif 
		return ret ;
	}
	
	public void PlayNext() 
	{
#if DEBUG		
		Debug.Log( "ConversationSet::PlayNext() start" ) ;
#endif 
		if( true == IsFinished() )
			return ;
		
		if( m_CurrentIndex < m_Conversations.Count ) 
		{
#if DEBUG		
			Debug.Log( "ConversationSet::PlayNext() m_Conversations[ m_CurrentIndex ].PlayNext" ) ;
#endif 			
			m_Conversations[ m_CurrentIndex ].PlayNext() ;
		}
		
		if( true == m_Conversations[ m_CurrentIndex ].IsFinished() )
		{
			++m_CurrentIndex ;
#if DEBUG		
			Debug.Log( "ConversationSet::PlayNext() m_CurrentIndex=" + m_CurrentIndex ) ;
#endif
		}
	}
	
	public void EnableDialog( ref Dictionary<string,GameObject> _GUIObjectList )
	{
#if DEBUG		
		Debug.Log( "ConversationSet::EnableDialog() " ) ;
#endif
		if( true == IsFinished() )
			return ;
		
		m_Conversations[ m_CurrentIndex ].EnableDialog( ref _GUIObjectList ) ;
	}
	
	public ConversationSet()
	{
	}
	
	public ConversationSet( ConversationSet _src )
	{
		m_Key = _src.m_Key ;
		m_Conversations = _src.m_Conversations ;
		m_CurrentIndex = _src.m_CurrentIndex ;
	}
}
