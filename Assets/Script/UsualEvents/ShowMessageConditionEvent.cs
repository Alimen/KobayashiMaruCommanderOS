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
@file ShowMessageConditionEvent.cs
@author NDark

顯示訊息的事件

# MessageIndex 訊息內容的索引


@date 20130116 file started and copy from UnitIsReachLocationShowGUITextEvent
@date 20130126 by NDark . add class member m_Index
@date 20130205 by NDark
. rename class member m_Index to m_MessageIndex
. remove class member m_Message

*/
// #define DEBUG
using UnityEngine;
using System.Xml;

/*
碰撞後顯示訊息的事件
*/
[System.Serializable]
public class ShowMessageConditionEvent : ConditionEvent
{
	private int m_MessageIndex = -1 ;
	
	public ShowMessageConditionEvent()
	{
	}
	
	public ShowMessageConditionEvent( ShowMessageConditionEvent _src ) : base( _src )
	{
		m_MessageIndex = _src.m_MessageIndex ;
	}
	
	public override bool ParseXML( XmlNode _Node )
	{
#if DEBUG				
		Debug.Log( "ShowMessageConditionEvent::ParseXML()" ) ;
#endif
		ParseForChildren( _Node ) ;

		if( null != _Node.Attributes["MessageIndex"] )
		{
			string IndexStr = _Node.Attributes["MessageIndex"].Value ;
			int.TryParse( IndexStr , out m_MessageIndex ) ;
		}		
		
		return true ;		
	}
	
	public override void DoEvent()
	{
#if DEBUG				
		Debug.Log( "ShowMessageConditionEvent::DoEvent()" ) ;
#endif		
		ShowMessage() ;
	}	
	
	private void ShowMessage()
	{
		MessageQueueManager messaeQueueManager = GlobalSingleton.GetMessageQueueManager() ;
		
		string message = "" ;
		
		if( -1 != m_MessageIndex )
			message = StrsManager.Get( m_MessageIndex ) ;
		
		if( null != messaeQueueManager )
			messaeQueueManager.AddMessage( message ) ;
	}

}