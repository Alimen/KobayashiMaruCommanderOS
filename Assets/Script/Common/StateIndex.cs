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
@file StateIndex.cs
@brief 有序狀態.
A index class with ability to record time, 
and first time after previous assign state.
@author NDark

-# call Initialize() at start
-# call Update() in the front of each update to make sure the correctness of IsFirstTime().

@date 20121125 by NDark add copy constructor of StateIndex.
@date 20121218 by NDark . add class member m_ActiveDebug
@date 20121220 by NDark . add realtimeSinceStartup of StateIndex

*/
using UnityEngine;

[System.Serializable]
public class StateIndex
{
	public bool m_UseRealTime = false ;
	public bool m_ActiveDebug = false ;
	public int state
	{
		set
		{ 
			m_PreviousValue = m_ValueNow ;
			if( false == m_UseRealTime )
				m_ChangeTime = Time.time ;// reset time
			else 
				m_ChangeTime = Time.realtimeSinceStartup ;// reset time
			m_JustChangeStateFlag = true ;
			m_ValueNow = value; 
			
			if( true == m_ActiveDebug )
				Debug.Log( "StateIndex=" + value.ToString() ) ;
			
		}
		get{ return m_ValueNow ; } 
	}
	
	public float Previous()
	{
		return m_PreviousValue ;
	}	
	
	public void Initialize( int _InitState )
	{
		state = _InitState ;
	}
	
	public void Update()
	{
		m_FirstElapsedFlag = m_JustChangeStateFlag ;
		m_JustChangeStateFlag = false ;
	}
	
	public bool IsFirstTime()
	{
		return m_FirstElapsedFlag ;
	}
	
	public float ElapsedFromLast()
	{
		float ret = 0.0f ;
		if( false == m_UseRealTime )
			ret = Time.time - m_ChangeTime ;// reset time
		else 
			ret = Time.realtimeSinceStartup - m_ChangeTime ;// reset time		
		return ret;
	}
	
	public StateIndex()
	{
	}
	public StateIndex( StateIndex _src )
	{
		m_ValueNow = _src.m_ValueNow ;
		m_PreviousValue = _src.m_PreviousValue ;
		m_ChangeTime = _src.m_ChangeTime ;
		m_JustChangeStateFlag = _src.m_JustChangeStateFlag ;
		m_FirstElapsedFlag = _src.m_FirstElapsedFlag ;
	}
	
	private int m_ValueNow = 0 ;
	private int m_PreviousValue = 0 ;
	private float m_ChangeTime = 0.0f ;	
	private bool m_JustChangeStateFlag = false ;
	private bool m_FirstElapsedFlag = false ;
	
}