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
@file BaseDefine.cs
@author NDark

## CLICK_SEC : The Click Sec
## WAIT_SEC_AFTER_SHOW : The wait sec after show
## COLLIDE_DAMAGE_IN_SEC : The collide damage in a sec.
## STANDARD_COLLIDE_PUSH_SPEED : The standard push speed when collide.
## STANDARD_SENSOR_TOTAL_EFFECT : Constant standard sensor toatl effect.
## MIN_SCALE_FROM_STANDARD_DISTANCE_CAMERA : Constant min and max scale from standard distance of camera
## MAX_SCALE_FROM_STANDARD_DISTANCE_CAMERA : Constant min and max scale from standard distance of camera
## CAMERA_SHAKE_DISTANCE : Constant camera shake distance
## OBJECT_SHAKE_DISTANCE : Constant object shake distance.
## STANDARD_PUSH_DISTANCE : Constant push distance when mass is 1.
## DRAW_MINIMAP_REFRESH_SEC : Constant second of refresh draw minimap.
## SENSOR_REFRESH_SEC
## SLOWMOTION_SCALE_IN_TIME
## FREEZE_SCALE_IN_TIME
## NORMAL_SCALE_IN_TIME
## MESSAGE_QUEUE_SHOW_SEC
## MESSAGE_QUEUE_FADEOUT_SEC
## PHASER_CAUSE_DAMAGE_CYCLE_SEC
## BATTLE_EVENT_CAMERA_ELAPSED_SEC

@date 20130111 file started.
@date 20130126 by NDark . change class member m_BATTLE_EVENT_CAMERA_ELAPSED_SEC to private, and add relative property.
@date 20130204 by NDark . refine code.

*/
using UnityEngine;

[System.Serializable]
public static class BaseDefine 
{
	/// <summary>
	/// The click sec
	/// </summary>
	public const float CLICK_SEC = 0.5f ;
	
	/// <summary>
	/// The wait sec after show
	/// </summary>
	public const float WAIT_SEC_AFTER_SHOW = 0.3f ;	
	
	/// <summary>
	/// The collide damage in a sec.
	/// </summary>
	public const float COLLIDE_DAMAGE_IN_SEC = 1.8f ;
	
	/// <summary>
	/// The standard push speed when collide.
	/// </summary>
	public const float STANDARD_COLLIDE_PUSH_SPEED = 1.0f ;
	
	/// <summary>
	/// Constant standard sensor toatl effect.
	/// </summary>
	public const float STANDARD_SENSOR_TOTAL_EFFECT = 120.0f ;
	
	/// <summary>
	/// Constant min and max scale from standard distance of camera
	/// </summary>
	public const float MIN_SCALE_FROM_STANDARD_DISTANCE_CAMERA = 0.3f ;
	public const float MAX_SCALE_FROM_STANDARD_DISTANCE_CAMERA = 1.3f ;
	
	/// <summary>
	/// Constant camera shake distance.
	/// </summary>
	public const float CAMERA_SHAKE_DISTANCE = 0.5f ;
	
	
	/// <summary>
	/// Constant object shake distance.
	/// </summary>
	public const float OBJECT_SHAKE_DISTANCE = 0.1f ;
	
	/// <summary>
	/// Constant push distance when mass is 1.
	/// </summary>
	public const float STANDARD_PUSH_DISTANCE = 16.0f ;	
	
	/// <summary>
	/// Constant second of refresh draw minimap.
	/// </summary>
	public const float DRAW_MINIMAP_REFRESH_SEC = 1.0f ;
	
	public const float SENSOR_REFRESH_SEC = 1.0f ;
	
	public const float SLOWMOTION_SCALE_IN_TIME = 0.1f ;
	public const float FREEZE_SCALE_IN_TIME = 0.01f ;
	public const float NORMAL_SCALE_IN_TIME = 1.0f ;
	
	public const float MESSAGE_QUEUE_SHOW_SEC = 3.0f ;
	public const float MESSAGE_QUEUE_FADEOUT_SEC = 3.0f ;
	
	public const float PHASER_CAUSE_DAMAGE_CYCLE_SEC = 1.0f ;
	
	private static float m_BATTLE_EVENT_CAMERA_ELAPSED_SEC = 6.0f ;
	public static float BATTLE_EVENT_CAMERA_ELAPSED_SEC
	{
		get
		{
			return m_BATTLE_EVENT_CAMERA_ELAPSED_SEC ;
		}
		set
		{
			m_BATTLE_EVENT_CAMERA_ELAPSED_SEC = value ;
		}
	}
}
