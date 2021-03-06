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
@file GUI_FadeInOut.cs
@author NDark
 
淡入淡出

# 依照狀態進行淡入淡出
<pre>
	public enum FadeState
	{
		UnActive = 0 ,
		FadeIn , 
		Steady ,
		FadeOut ,
		End ,
	}
</pre>
# 同時對物件其下的GUITexture與GUIText作用
# 可以設定時間
# 可以設定是否要淡入淡出
# 如果要淡入，則開始時不顯示，淡入時才顯示
# 如果要循環 設定 m_IsLoop

@date 20121210 file started.
@date 20121219 by NDark . comment.
@date 20121225 by NDark . add code of ShowGUITexture before fadein and after fadeout
@date 20130103 by NDark 
. add class method GetAlpha()
. add class method ApplyAlpha()
@date 20130113 by NDark . comment.
@date 20130119 by NDark 
. add class member m_IsLoop
. fix an error of half the alpha at ApplyAlpha()

*/
using UnityEngine;

public class GUI_FadeInOut : MonoBehaviour 
{
	public enum FadeState
	{
		UnActive = 0 ,
		FadeIn , 
		Steady ,
		FadeOut ,
		End ,
	}
	public StateIndex m_State = new StateIndex() ;
	public float m_StartSec = 0.0f ;
	public float m_CurrentSec = 0.0f ;
	public bool m_FadeInValid = true ;
	public float m_FadeInSec = 3.0f ;
	
	public float m_SteadySec = 3.0f ;
	
	public bool m_FadeOutValid = true ;
	public float m_FadeOutSec = 3.0f ;
	
	public bool m_IsLoop = false ;
	
	private GUITexture []m_GUITextures = null ;
	private GUIText []m_GUITexts = null ;
	
	
	// Use this for initialization
	void Start () 
	{
		m_GUITextures = this.gameObject.GetComponentsInChildren<GUITexture>() ;
		m_GUITexts = this.gameObject.GetComponentsInChildren<GUIText>() ;
		if( true == m_FadeInValid )
		{
			foreach( GUITexture guiTexture in m_GUITextures )
			{
				guiTexture.color = new Color( guiTexture.color.r , 
					guiTexture.color.g , 
					guiTexture.color.b , 
					0 ) ;
			}
			
			foreach( GUIText guiText in m_GUITexts )
			{
				guiText.material.color = new Color( guiText.material.color.r , 
					guiText.material.color.g , 
					guiText.material.color.b , 
					0 ) ;
			}			
			
			ShowGUITexture.Show( this.gameObject , false , true , true ) ;
		}
		
		m_State.state = (int)FadeState.UnActive ;
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		float currentAlpha = 0 ;
		m_State.Update() ;
		switch( (FadeState)m_State.state )
		{
		case FadeState.UnActive :
			if( m_State.ElapsedFromLast() > m_StartSec )
			{
				m_State.state = (int)FadeState.FadeIn ;			
			}
			break ;
		case FadeState.FadeIn :
			if( true == m_State.IsFirstTime() )
				ShowGUITexture.Show( this.gameObject , true , true ,true ) ;
			
			if( false == m_FadeInValid )
				m_State.state = (int)FadeState.Steady ;				
			
			currentAlpha = GetAlpha() ;
			float timeRemain = m_FadeInSec - m_State.ElapsedFromLast() ;
			if( timeRemain < 0.0f )
			{
				currentAlpha = 1.0f ;
				m_State.state = (int) FadeState.Steady ;
			}
			else
			{
				currentAlpha = m_State.ElapsedFromLast() / m_FadeInSec ;
			}
			
			ApplyAlpha( currentAlpha ) ;	
			
			
			break ;
			
		case FadeState.Steady :
			if( m_State.ElapsedFromLast() > m_SteadySec )
				m_State.state = (int)FadeState.FadeOut ;
			break ;
		case FadeState.FadeOut :
			if( false == m_FadeOutValid )
				
				m_State.state = (int) FadeState.End ;
			
			currentAlpha = GetAlpha() ;
			timeRemain = m_FadeOutSec - m_State.ElapsedFromLast() ;
			if( timeRemain < 0.0f )
			{
				currentAlpha = 0.0f ;
				m_State.state = (int) FadeState.End ;
			}
			else
			{
				currentAlpha = timeRemain / m_FadeOutSec ;
			}

			ApplyAlpha( currentAlpha ) ;	
			
			break ;
		case FadeState.End :
			if( true == m_IsLoop )
			{
				m_State.state = (int) FadeState.FadeIn ;
			}
			else if( true == m_State.IsFirstTime() )
			{
				ShowGUITexture.Show( this.gameObject , false , true , true ) ;
			}
			break ;			
		}
	
	}
	
	private float GetAlpha()
	{
		float ret = 0 ;
		if( m_GUITextures.Length > 0 )
			ret = m_GUITextures[ 0 ].color.a ;
		return ret ;
	}
	
	private void ApplyAlpha( float _Alpha )
	{
		// Debug.Log( "ApplyAlpha() _Alpha=" + _Alpha ) ;
		
		foreach( GUITexture guiTexture in m_GUITextures )
		{
			guiTexture.color = new Color( guiTexture.color.r , 
				guiTexture.color.g , 
				guiTexture.color.b , 
				_Alpha ) ;
		}
		
		foreach( GUIText guiText in m_GUITexts )
		{
			guiText.material.color = new Color( guiText.material.color.r , 
				guiText.material.color.g , 
				guiText.material.color.b , 
				_Alpha ) ;			
		}		
	}
}
