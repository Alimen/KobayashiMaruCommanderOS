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
@file ControlPanelActive.cs
@brief 控制面版啟動功能
@author NDark 

# 掛載在控制面版(彩色)的牌卡上
# 當按下左鍵時啟動或關閉功能
# 如果沒有點選(啟動)就離開 滑鼠離開時也會自動關閉
# 沒點選時 檢查滑鼠不在 強制關閉(上步驟的保險措施)

@date 20121117 file started.
@date 20121119 by NDark . add class method ActiveMainCharacterFunc()
@date 20121203 by NDark 
. add ShowControlPanel() at ActiveMainCharacterFunc()
. modify timing we close the GUI
@date 20121220 by NDark
. modify code for control panel of next version.
. refactor
. add class member m_ParentUnActive
. add class method RetrieveParentUnActive()
. add class method IsMouseOutsideThePanel()
@date 20121230 by NDark . rename class member m_ParentUnActive to m_ParentUnActiveObject
@date 20130113 by NDark
. remove class member m_ParentUnActiveObject
. remove class method RetrieveParentUnActive()
@date 20130205 by NDark
. remove class member m_ActiveCalled
. comment.

*/
using UnityEngine;

public class ControlPanelActive : MonoBehaviour 
{
	/**
	 * 因應control panel有三個狀態
	 * Ready , 滑鼠未點擊 滑鼠離開時關閉 Control Panel Active
	 *         滑鼠點擊切換到Active狀態
	 * 	       假如滑鼠離開就切換到Close狀態
	 * Active , 滑鼠已點擊 功能啟動 不會因為滑鼠離開而關閉.
	 * Close , 不會檢查上述流程的兩種狀態
	 */
	public BasicTrigger m_CloseActive = new BasicTrigger() ;
	
	// 由於座標是儲存在parent上，所以要計算滑鼠是否離開面板需要此物件
	public NamedObject m_ParentUnActiveObject = new NamedObject() ;
	
	public void Setup()
	{
		m_CloseActive.Initialize() ;
	}
	
	
	// Use this for initialization
	void Start () 
	{
		m_CloseActive.Close() ;
	}

	/**
	 * 因應control panel有三個狀態
	 * Ready , 滑鼠未點擊 滑鼠離開時關閉 Control Panel Active
	 *         滑鼠點擊切換到Active狀態
	 * 	       假如滑鼠離開就切換到Close狀態
	 * Active , 滑鼠已點擊 功能啟動 不會因為滑鼠離開而關閉.
	 * Close , 不會檢查上述流程的兩種狀態
	 */	
	// Update is called once per frame
	void Update () 
	{
		if( m_CloseActive.IsReady() &&
			true == IsMouseOutsideThePanel() )
		{
			// Debug.Log( "true == IsMouseOutsideThePanel" ) ;
			this.gameObject.guiTexture.enabled = false ;
			m_CloseActive.Close() ;
		}
	}
	
	/**
	 * 因應control panel有三個狀態
	 * Ready , 滑鼠未點擊 滑鼠離開時關閉 Control Panel Active
	 *         滑鼠點擊切換到Active狀態
	 * 	       假如滑鼠離開就切換到Close狀態
	 * Active , 滑鼠已點擊 功能啟動 不會因為滑鼠離開而關閉.
	 * Close , 不會檢查上述流程的兩種狀態
	 */	
	void OnMouseDown()
	{
		if( true == m_CloseActive.IsActive() )
		{
			// 關閉
			// Debug.Log( "true == m_CloseActive.IsActive" ) ;
			DeactiveMainCharacterFunc() ;
			m_CloseActive.Initialize() ;
		}
		else if( true == m_CloseActive.IsReady() )
		{
			// 啟動
			ActiveMainCharacterFunc( this.gameObject.name ) ;
			m_CloseActive.Active() ;
		}
	}
	
	/**
	 * 因應control panel有三個狀態
	 * Ready , 滑鼠未點擊 滑鼠離開時關閉 Control Panel Active
	 *         滑鼠點擊切換到Active狀態
	 * 	       假如滑鼠離開就切換到Close狀態
	 * Active , 滑鼠已點擊 功能啟動 不會因為滑鼠離開而關閉.
	 * Close , 不會檢查上述流程的兩種狀態
	 */	
	void OnMouseExit()
	{
		// Debug.Log( "OnMouseExit" ) ;
		// 如果沒有點選(啟動)就離開 滑鼠離開時也會自動關閉
		if( true == m_CloseActive.IsReady() ) 
		{
			// Debug.Log( "true == m_CloseActive.IsReady" ) ;
			this.gameObject.guiTexture.enabled = false ;	
			m_CloseActive.Close() ;
		}
	}
	
	
	// 啟動功能
	private void ActiveMainCharacterFunc( string _ControlPanelObjectName )
	{
		MainCharacterController controller = GlobalSingleton.GetMainCharacterControllerComponent() ;
		if( null == controller )
			return ;
		
		SelectFunctionMode mode = ConstName.FindControlPanelFunction( _ControlPanelObjectName ) ;
		controller.SwitchControlMode( mode ) ;
	}
	
	private void DeactiveMainCharacterFunc()
	{
		MainCharacterController controller = GlobalSingleton.GetMainCharacterControllerComponent() ;
		if( null == controller )
			return ;
		controller.CancelControlMode() ;
	}
		
	private bool IsMouseOutsideThePanel()
	{
		// Debug.Log( "this.gameObject.transform.position=" + this.gameObject.transform.position ) ;
		
		Vector3 ScreenPos = Camera.mainCamera.ViewportToScreenPoint( this.gameObject.transform.position ) ;
		Rect panel = this.gameObject.guiTexture.pixelInset ;
		Rect deteczone = new Rect( ScreenPos.x + panel.x , 
								   ScreenPos.y + panel.y , 
								   panel.width ,
								   panel.width ) ;
		
		return ( Input.mousePosition.x < deteczone.x ||
				 Input.mousePosition.y < deteczone.y ||
				 Input.mousePosition.x > deteczone.x + deteczone.width ||
				 Input.mousePosition.y > deteczone.y + deteczone.height ) ;
		
	}
	
}
