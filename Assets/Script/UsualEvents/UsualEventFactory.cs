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
@file UsualEventFactory.cs
@author NDark
@date 20120112 file started and copy from UsualEvent.cs
@date 20130116 
. replace UnitIsReachLocationShowGUITextEvent by ShowMessageConditionEvent.
. replace ReplaceAIWhenEnterEvent by ReplaceAIConditionEvent.
. replace ActiveConversationTimeEvent by ActiveConversationConditionEvent.
. remove ActiveConversationCollisionEvent

*/
using UnityEngine;

[System.Serializable]
public static class UsualEventFactory
{
	public static UsualEvent GetByString( string _EventName )
	{
		if( _EventName == "GUITextureShowTimeEvent" )
			return new GUITextureShowTimeEvent() ;
		else if( _EventName == "AudioPlayTimeEvent" )
			return new AudioPlayTimeEvent() ;		
		else if( _EventName == "SetLevelObjectiveTimeEvent" )
			return new SetLevelObjectiveTimeEvent() ;
		
		else if( _EventName == "ReplaceAIConditionEvent" )
			return new ReplaceAIConditionEvent() ;
		
		else if( _EventName == "ShowMessageConditionEvent" )
			return new ShowMessageConditionEvent() ;
		
		else if( _EventName == "CameraRoutesEvent" )
			return new CameraRoutesEvent() ;		
		
		else if( _EventName == "ActiveCountDownConditionEvent" )
			return new ActiveCountDownConditionEvent() ;			
		else if( _EventName == "ActiveElapsedTimeConditionEvent" )
			return new ActiveElapsedTimeConditionEvent() ;
		else if( _EventName == "EnableMiniMapConditionEvent" )
			return new EnableMiniMapConditionEvent() ;
		else if( _EventName == "RandomizeObjectsConditionEvent" )
			return new RandomizeObjectsConditionEvent() ;
		else if( _EventName == "EnableEnergyManipulatorPanelConditionEvent" )
			return new EnableEnergyManipulatorPanelConditionEvent() ;		
		else if( _EventName == "EnableBattleEventCameraManagerConditionEvent" )
			return new EnableBattleEventCameraManagerConditionEvent() ;		
		
		else if( _EventName == "EnemyGenerationPipelineEvent" )
			return new EnemyGenerationPipelineEvent() ;		
		else if( _EventName == "PlayAnimationConditionEvent" )
			return new PlayAnimationConditionEvent() ;		
		else if( _EventName == "JumpEffectTimeEvent" )
			return new JumpEffectTimeEvent() ;
		else if( _EventName == "PlayBackgroundMusicConditionEvent" )
			return new PlayBackgroundMusicConditionEvent() ;		
		else if( _EventName == "MoveObjectPositionConditionEvent" )
			return new MoveObjectPositionConditionEvent() ;	
		
		else if( _EventName == "AlterScriptConditionEvent" )
			return new AlterScriptConditionEvent() ;			
		
		else if( _EventName == "ActiveConversationConditionEvent" )
			return new ActiveConversationConditionEvent() ;	
		
		else if( _EventName == "Level9ChangeTargetConditionEvent" )
			return new Level9ChangeTargetConditionEvent() ;			
		
		
		else
			return null ;
	}

}