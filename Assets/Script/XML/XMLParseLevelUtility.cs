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
@file XMLParseLevelUtility.cs
@brief XML關卡讀檔相關函式
@author NDark

# ParseAnchor() 分析位置錨點
# ParsePosition() 分析位置
包含座標及位置物件
# ParseQuaternion() 分析旋轉
# ParseSupplementalPair() 分析補充資料
# ParseUnit() 分析XML
call ParseUnitInitData()
# ParseEnemyGeneration() 分析動態產生單位資料
# ParseStandardParameter() 分析XML
# ParseAddScriptOnObject() 分析XML 新增script到指定物件
# ParseStaticObject() 分析靜態物件
# ParseBackgroundMusic() 分析XML 背景音樂設定
# ParseConversationSet() 分析對話集合
# ParseConversation() 分析對話
# ParseTalkStr() 分析字串索引
# ParsePotrit() 分析人頭像

@date 20121204 file created.
@date 20121209 by NDark . add class method ParsePosition()
@date 20121213 by NDark . modify to right hand coordinate at ParseQuaternion()
@date 20121224 by NDark . add class method ParseAnchor()
@date 20130102 by NDark . fix an error at ParseEnemyGeneration()
@date 20130103 by NDark
. add class method ParseConversationSet()
. add class method ParseConversation()
. add class method ParseTalkStr()
. add class method ParsePotrit()
@date 20130109 by NDark 
. add class member ParseUnitInitData()
. add class member ParseEnemyGeneration()
@date 20130115 by NDark . add class method ParseAnchor()
@date 20130121 by NDark . replace by Index at ParseTalkStr()
@date 20130203 by NDark 
. change argument type of ParsePosition()
. replace templateName by PrefabName

*/
using UnityEngine;
using System.Collections.Generic;
using System.Xml;

public static class XMLParseLevelUtility 
{
	public static bool ParseAnchor( XmlNode _Node , 
									ref PosAnchor _ResultAnchor )
	{
		if( null == _Node.Attributes[ "PosAnchor" ] )
			return false ;
		string PosAnchorStr = _Node.Attributes[ "PosAnchor" ].Value ;
			
		return ParseAnchor( PosAnchorStr , 
							ref _ResultAnchor ) ;
	}		
	public static bool ParseAnchor( string _AnchorStr , 
									ref PosAnchor _ResultAnchor )
	{
		char [] seperator = {','} ;
		string[] strVec = _AnchorStr.Split( seperator ) ;
		if( strVec.Length > 2 )
		{
			float x = 0 ;
			float y = 0 ;
			float z = 0 ;
			ParsePosition( _AnchorStr , ref x , ref y , ref z ) ;
			_ResultAnchor.Setup( new Vector3( x , y , z ) ) ;
			return true ;
		}
		else
		{
			_ResultAnchor.Setup( _AnchorStr ) ;
			return true ;
		}		
	}	
	public static bool ParsePosition( string _PositionStr , 
									  ref float _x ,
									  ref float _y , 
									  ref float _z )
	{
		char [] seperator = {','} ;
		string[] strVec = _PositionStr.Split( seperator ) ;
		if( strVec.Length > 2 )
		{
			float.TryParse( strVec[0] , out _x ) ;
			float.TryParse( strVec[1] , out _y ) ;
			float.TryParse( strVec[2] , out _z ) ;
			return true ;
		}
		return false ;
	}
	
	// 分析位置 包含座標及位置物件
	public static bool ParsePosition( XmlNode _PositionNode , 
									  ref PosAnchor _PosAnchor )
	{
		if( "Position3D" == _PositionNode.Name )
		{
			if( null != _PositionNode.Attributes[ "objectName" ] )
			{
				string objectName = _PositionNode.Attributes[ "objectName" ].Value ;
				_PosAnchor.Setup( objectName ) ;
			}
			else
			{
				
				// 絕對座標
				string strx = _PositionNode.Attributes[ "x" ].Value ;
				string stry = _PositionNode.Attributes[ "y" ].Value ;
				string strz = _PositionNode.Attributes[ "z" ].Value ;
				float x , y , z ;
				float.TryParse( strx , out x ) ;
				float.TryParse( stry , out y ) ;
				float.TryParse( strz , out z ) ;
				Vector3 setPosition = new Vector3( x , y , z ) ;
				// the y position of Unit is sure set to 1.0f
				setPosition.y = 1.0f ;
				
				_PosAnchor.Setup( setPosition ) ;
			}			

			return true ;
		}
		return false ;
	}
	

	// 分析旋轉
	public static bool ParseQuaternion( XmlNode _OrientationNode , 
						  				ref Quaternion _Orientation )
	{
		if( "OrientationEuler" == _OrientationNode.Name )
		{
			string strx = _OrientationNode.Attributes[ "x" ].Value ;
			string stry = _OrientationNode.Attributes[ "y" ].Value ;
			string strz = _OrientationNode.Attributes[ "z" ].Value ;
			float x , y , z ;
			float.TryParse( strx , out x ) ;
			float.TryParse( stry , out y ) ;
			float.TryParse( strz , out z ) ;									
			_Orientation = Quaternion.Euler( -x , -y , -z ) ;
			return true ;
		}		
		return false ;
	}	
	
	
	// 分析補充資料
	public static bool ParseSupplementalPair( XmlNode _Node , 
											  out string _Label , 
											  out string _Value )
	{
		_Label = "" ;
		_Value = "" ;
		bool ret = false ;
		if( "SupplementalPair" == _Node.Name &&
			null != _Node.Attributes[ "label" ] && 
			null != _Node.Attributes[ "value" ] )
		{
			_Label = _Node.Attributes[ "label" ].Value ;
			_Value = _Node.Attributes[ "value" ].Value ;
			return true ;
		}		
		return ret ;
	}
	
	
	// 分析XML 取出每筆單位的資料
	public static bool ParseUnitInitData( XmlNode _UnitNode , 
										  ref string _UnitName , 
										  ref string _TemplateName ,
										  ref string _UnitDataTemplateName ,
										  ref string _RaceName ,
										  ref string _SideName ,		
										  ref PosAnchor _PosAnchor , 
										  ref Quaternion _Orientation ,
										  ref Dictionary<string , string> _SupplementalVec )
	{
		if( null != _UnitNode.Attributes[ "name" ] )
			_UnitName = _UnitNode.Attributes[ "name" ].Value ;
		
		if( null != _UnitNode.Attributes[ "PrefabName" ] )
			_TemplateName = _UnitNode.Attributes[ "PrefabName" ].Value ;

		if( null != _UnitNode.Attributes[ "unitDataTemplateName" ] )
			_UnitDataTemplateName = _UnitNode.Attributes[ "unitDataTemplateName" ].Value ;
		
		if( null != _UnitNode.Attributes[ "raceName" ] )
			_RaceName = _UnitNode.Attributes[ "raceName" ].Value ;
		
		if( null != _UnitNode.Attributes[ "sideName" ] )
			_SideName = _UnitNode.Attributes[ "sideName" ].Value ;		
		
		string Label ;
		string Value ;
		
		if( true == _UnitNode.HasChildNodes )
		{
			for( int j = 0 ; j < _UnitNode.ChildNodes.Count ; ++j )
			{
				if( true == ParsePosition( _UnitNode.ChildNodes[ j ] , 
										   ref _PosAnchor ) )
				{
					
				}
				else if( true == ParseQuaternion( _UnitNode.ChildNodes[ j ] , 
																	   ref _Orientation ) )
				{
					
				}
				else if( true == ParseSupplementalPair( _UnitNode.ChildNodes[ j ] , 
																			 out Label , 
																			 out Value ) )
				{
					// Debug.Log( "ParseSupplementalPair() " + Label + " " + Value ) ;
					_SupplementalVec.Add( Label , Value ) ;
				}				
			}
		}
		return true ;
	}


	/* Parse Unit Init Data */
	public static bool ParseUnit( XmlNode _UnitNode , 
								  out string _UnitName , 
								  out string _TemplateName ,
								  out string _UnitDataTemplateName ,
								  out string _RaceName ,
								  out string _SideName ,
							      out PosAnchor _PosAnchor , 
							      out Quaternion _Orientation ,
								  out Dictionary<string , string> _SupplementalVec )
	{
		_UnitName = "" ;
		_TemplateName = "" ;
		_UnitDataTemplateName = "" ;
		_RaceName = "" ;
		_SideName = "" ;
		_PosAnchor = new PosAnchor() ;
		_Orientation = Quaternion.identity ;
		_SupplementalVec = new Dictionary<string , string>() ;

		if( "Unit" != _UnitNode.Name )
		{
			// Debug.Log( "ParseUnitInitData() : Unit != _UnitNode.Name:" + _UnitNode.Name ) ;
			return false ;
		}
		
		return ParseUnitInitData( _UnitNode , 
								  ref _UnitName , 
								  ref _TemplateName ,
								  ref _UnitDataTemplateName ,
								  ref _RaceName ,
								  ref _SideName ,
								  ref _PosAnchor , 
								  ref _Orientation ,
								  ref _SupplementalVec ) ;
	}
	
	
	// 分析動態產生單位資料
	public static bool ParseEnemyGeneration( XmlNode _UnitNode , 
										   out string _UnitName , 
										   out string _TemplateName ,
										   out string _UnitDataTemplateName ,
										   out string _RaceName ,
										   out string _SideName ,
										   out PosAnchor _PosAnchor , 
										   out Quaternion _Orientation ,
										   out Dictionary<string , string> _SupplementalVec ,
										   out float _time )
	{
		_UnitName = "" ;
		_TemplateName = "" ;
		_UnitDataTemplateName = "" ;
		_RaceName = "" ;
		_SideName = "" ;
		_PosAnchor = new PosAnchor() ;
		_Orientation = Quaternion.identity ;
		_time = 0.0f ;
		_SupplementalVec = new Dictionary<string, string>() ;
		
		if( "EnemyGeneration" != _UnitNode.Name )
		{
			// Debug.Log( "ParseEnemyGeneration() : Unit != _UnitNode.Name:" + _UnitNode.Name ) ;
			return false ;
		}
		
		if( null != _UnitNode.Attributes["time"] )
		{
			string timestr = _UnitNode.Attributes["time"].Value ;
			float time = 0.0f ;
			float.TryParse( timestr , out time ) ;
			_time = time ;
		}
			
		return ParseUnitInitData( _UnitNode , 
								  ref _UnitName , 
								  ref _TemplateName ,
								  ref _UnitDataTemplateName ,
								  ref _RaceName ,
								  ref _SideName ,			
								  ref _PosAnchor , 
								  ref _Orientation , 
								  ref _SupplementalVec ) ;
	}

	/* 
	 * @brief Parse standard parameter from xml
	 */
	public static bool ParseStandardParameter( XmlNode _StandardParameterNode ,
											out string _Name ,
											out StandardParameter _Param )
	{
		_Name = "" ;
		_Param = new StandardParameter() ;
		
		if( null == _StandardParameterNode )
			return false ;
		
		_Name = _StandardParameterNode.Name ;
		// Debug.Log( _Name ) ;
		/*
			public float m_Now = 0.0f ;
			public float m_Max = 0.0f ;
			public float m_Min = 0.0f ;
			public float m_Std = 0.0f ;
			public float m_Init = 0.0f ;
		 */
		
		if( null != _StandardParameterNode.Attributes[ "min" ] )
		{
			float min = 0.0f ;
			float.TryParse( _StandardParameterNode.Attributes[ "min" ].Value , 
				out min ) ;
		    _Param.min = min ;
		}
		
		if( null != _StandardParameterNode.Attributes[ "max" ] )
		{
			float max ;
			float.TryParse( _StandardParameterNode.Attributes[ "max" ].Value , 
				out max ) ;
		    _Param.max = max ;
			_Param.m_Std = max ;
			_Param.m_Init = max ;
			_Param.now = max ;
		}

		if( null != _StandardParameterNode.Attributes[ "std" ] )
		{
			float std ;
			float.TryParse( _StandardParameterNode.Attributes[ "std" ].Value , 
				out std ) ;
		    _Param.m_Std = std ;
			_Param.now = std ;
		}
		
		if( null != _StandardParameterNode.Attributes[ "init" ] )
		{
			float init ;
			float.TryParse( _StandardParameterNode.Attributes[ "init" ].Value , 
				out init ) ;
		    _Param.m_Init = init ;
			_Param.now = init ;
		}
		
		return ( 0 != _Name.Length ) ;
	}
	
	/*
	<AddScriptOnObject objectName="GlobalSingleton" scriptName="ChallangeLevel01Manager" />	
	 */
	// 分析XML 新增script到指定物件
	public static bool ParseAddScriptOnObject( XmlNode _UnitNode , 
											 ref string _objectName , 
											 ref string _scriptName )
	{
		_objectName = "" ;
		_scriptName = "" ;
		if( "AddScriptOnObject" != _UnitNode.Name )
		{
			// Debug.Log( "ParseLoseCondition() : LoseCondition != _UnitNode.Name=" + _UnitNode.Name ) ;
			return false ;
		}
		
		if( null == _UnitNode.Attributes["objectName"] ||
		    null == _UnitNode.Attributes["scriptName"] )
			return false ;	
		_objectName = _UnitNode.Attributes["objectName"].Value ;
		_scriptName = _UnitNode.Attributes["scriptName"].Value ;
		return true ;
	}
	
	
	// 分析靜態物件
	public static bool ParseStaticObject( XmlNode _UnitNode , 
										ref string _UnitName ,
										ref string _PrefabName ,
										ref Vector3 _InitPosition ,
										ref Quaternion _InitQuaternion )
	{
		if( "StaticObject" == _UnitNode.Name )
		{
			if( null != _UnitNode.Attributes[ "name" ] )
				_UnitName = _UnitNode.Attributes[ "name" ].Value ;
			if( null != _UnitNode.Attributes[ "PrefabName" ] )
				_PrefabName = _UnitNode.Attributes[ "PrefabName" ].Value ;
			
			if( true == _UnitNode.HasChildNodes )
			{
				for( int j = 0 ; j < _UnitNode.ChildNodes.Count ; ++j )
				{
					PosAnchor posAnchor = new PosAnchor() ;
					if( true == ParsePosition( _UnitNode.ChildNodes[ j ] , 
											  	ref posAnchor ) )
					{
						_InitPosition = posAnchor.GetPosition() ;
						// Debug.Log( "_InitPosition=" + _InitPosition ) ;
					}
					else if( true == ParseQuaternion( _UnitNode.ChildNodes[ j ] , 
													   ref _InitQuaternion ) )
					{
						
					}				
				}
			}
			return true ;
		}
		return false ;
	}
	
	// 分析XML 背景音樂設定
	public static bool ParseBackgroundMusic( XmlNode _UnitNode , 
											ref string _AudioPath )
	{
		if( "BackgroundMusicAudioPath" == _UnitNode.Name )
		{
			if( null != _UnitNode.Attributes["name"] )
			{
				_AudioPath = _UnitNode.Attributes["name"].Value ;
				return true ;
			}
		}
		return false ;
	}
	
	/*
	<ConversationSet Key="ConversationLevel9_1_ColonyAdama">
		<Conversation>
			<Potrait Name="PotraitRight" Texture="Conversation/Conversation_WilliamAdama" />
			<TalkStr Index="10001" />
			<TalkStr Index="10002" />
			<TalkStr Index="10003" />
			<TalkStr Index="10004" />
			<TalkStr Index="10005" />
		</Conversation>
	</ConversationSet>
	 */
	public static bool ParseConversationSet( XmlNode _ConversationSetNode , 
										  out ConversationSet _ConversationSet )
	{
		
		_ConversationSet = null ;
		if( "ConversationSet" == _ConversationSetNode.Name )
		{
			
			
			_ConversationSet = new ConversationSet() ;
			_ConversationSet.m_Key = _ConversationSetNode.Attributes[ "Key" ].Value ;
			for( int i = 0 ; i < _ConversationSetNode.ChildNodes.Count ; ++i )
			{
				Conversation conversation = null ;
				if( true == ParseConversation( _ConversationSetNode.ChildNodes[ i ] , out conversation ) )
				{
					_ConversationSet.m_Conversations.Add( conversation ) ;
				}
			}
		}
		return ( _ConversationSet != null &&
			     _ConversationSet.m_Conversations.Count > 0 ) ;
	}
	
	public static bool ParseConversation( XmlNode _ConversationNode , 
										  out Conversation _Conversation )
	{
		_Conversation = null ;
		if( "Conversation" == _ConversationNode.Name )
		{
			_Conversation = new Conversation() ;
			for( int i = 0 ; i < _ConversationNode.ChildNodes.Count ; ++i )
			{
				TalkStr talkStr = null ;
				if( true == ParseTalkStr( _ConversationNode.ChildNodes[ i ] , out talkStr ) )
				{
					_Conversation.m_Talks.Add( talkStr ) ;
				}
				Potrait potrait = null ;
				if( true == ParsePotrit( _ConversationNode.ChildNodes[ i ] , out potrait ) )
				{
					_Conversation.m_Potrits.Add( potrait ) ;
				}				
			}
		}
		return ( null != _Conversation &&
			_Conversation.m_Talks.Count > 0 ) ;
	}
	
	public static bool ParseTalkStr( XmlNode _TalkStrNode , 
									 out TalkStr _TalkStr )
	{
		_TalkStr = null ;
		if( "TalkStr" == _TalkStrNode.Name )
		{
			_TalkStr = new TalkStr() ;
			if( null != _TalkStrNode.Attributes[ "Index" ] )
			{
				string IndexStr = _TalkStrNode.Attributes[ "Index" ].Value ;
				int.TryParse( IndexStr , out _TalkStr.m_Index ) ;
			}			
		}
		return ( null != _TalkStr ) ;
	}
	
	// <Potrait Name="PotraitLeft" Texture="Conversation/GUI_Destination_Forbidden" />
	public static bool ParsePotrit( XmlNode _PotraitNode , 
									 out Potrait _Potrait )
	{
		_Potrait = null ;
		if( "Potrait" == _PotraitNode.Name )
		{
			_Potrait = new Potrait() ;
			if( null != _PotraitNode.Attributes[ "Name" ] )
			{
				_Potrait.m_PotraitName = _PotraitNode.Attributes[ "Name" ].Value ;
			}

			if( null != _PotraitNode.Attributes[ "Texture" ] )
			{
				_Potrait.m_TextureName = _PotraitNode.Attributes[ "Texture" ].Value ;
			}			
		}
		return ( null != _Potrait ) ;
	}			
}
