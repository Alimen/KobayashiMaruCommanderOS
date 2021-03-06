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
@date UsualEvent.cs
@brief 基本事件
@author NDark
 
@date 20121126 file created.
...
@date 20130102 by NDark . add EnemyGenerationPipelineEvent to UsualEventFactory
@date 20130103 by NDark . add ActiveConversationCollisionEvent to UsualEventFactory
@date 20130104 by NDark 
. add PlayAnimationCollisionEvent to UsualEventFactory
. add JumpEffectTimeEvent to UsualEventFactory
@date 20130106 by NDark . add Level9ChangeTargetNameEvent to UsualEventFactory
@date 20130112 by NDark . move class UsualEventFactory to UsualEventFactory.cs
@date 20130116 by NDark 
. move class AudioPlayTimeEvent to AudioPlayTimeEvent.cs
. add class ConditionEvent
. add Condition parse at ParseXML of ConditionEvent
. move class UnitIsReachLocationShowGUITextEvent to ShowMessageConditionEvent.cs and rename.
. move class ReplaceAIWhenEnterEvent to ReplaceAIConditionEvent.cs and rename.
@date 20130118 by NDark . move class ConditionEvent to ConditionEvent.cs
*/
// #define DEBUG

using UnityEngine;
using System.Xml;
using System.Collections.Generic ;

/*
基本事件
# ParseXML()
# Update()
*/
[System.Serializable]
public class UsualEvent 
{
	
	public UsualEvent()
	{
		
	}
	
	public UsualEvent( UsualEvent _src )
	{
		
	}
	
	public virtual bool ParseXML( XmlNode _Node )
	{
		return false ;
	}
	
	public virtual void Update()
	{
		
	}
}



