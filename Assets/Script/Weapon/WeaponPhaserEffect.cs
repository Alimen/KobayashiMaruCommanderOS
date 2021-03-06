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
@file WeaponPhaserEffect.cs
@brief 光炮特效
@author NDark

# 繼承自 WeaponEffect
# 掛載在光炮特效物件上
# m_WeaponDataShared 用來進行功能的武器集合
# Update()
## 更新特效物件 牽引光束
## 檢查是否需要關閉
# m_DamageCauseTimer 間斷觸發傷害 目前是1.0秒，請參考 BaseDefine.PHASER_CAUSE_DAMAGE_CYCLE_SEC

@date 20121112 file created.
@date 20121112 by NDark . add null assign at Start()
@date 20121219 by NDark . re-factor.
@date 20130110 by NDark
. add class member m_DamageCauseTimer
. add class member m_TriggerDamage
. add class method Setup()
. add class method TryCauseDamage()
. add class method CauseDamage()
@date 20130119 by NDark
. add class method CheckTargetExist()
. add the first strike at Setup()

*/
using UnityEngine;

public class WeaponPhaserEffect : WeaponEffect {
	
	float m_TriggerDamage = 0.0f ;
	CountDownTrigger m_DamageCauseTimer = new CountDownTrigger( BaseDefine.PHASER_CAUSE_DAMAGE_CYCLE_SEC ) ;
	
	public override void Setup( WeaponDataSet _WeaponData )
	{
		base.Setup( _WeaponData ) ;
		if( null != m_WeaponDataShared )
		{
			float triggertime = 
				Mathf.Floor( m_WeaponDataShared.m_FireTotalTime / m_DamageCauseTimer.m_CountDownTime ) 
					+ 1 ;
			// Debug.Log( triggertime ) ;
			m_TriggerDamage = m_WeaponDataShared.m_CauseDamage / triggertime ;
			// Debug.Log( m_TriggerDamage ) ;
		}
		// first time
		CauseDamage() ;
		m_DamageCauseTimer.Rewind() ;
	}
	
	// Use this for initialization
	void Start () 
	{
		ClearWeaponDataShared() ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( null != m_WeaponDataShared )
		{
			SetObjFromComponentToTargetComponent() ;
			TryCauseDamage() ;
			CheckTargetExist() ;
			this.CheckRendererDisable() ;	
		}
	}
	
	protected virtual void CheckTargetExist()
	{
		if( null == m_WeaponDataShared.TargetUnitObject )
		{
			// Debug.Log( "null == m_WeaponDataShared.TargetUnitObject" ) ;
			Renderer renderer = this.gameObject.GetComponentInChildren<Renderer>() ;
			if( null != renderer )
				renderer.enabled = false;
		}
	}
	
	protected virtual void SetObjFromComponentToTargetComponent()
	{
		GameObject targetComponentObj = m_WeaponDataShared.TargetComponentObject ;
		GameObject thisComponentObj = m_WeaponDataShared.Component3DObject ;
		if( null != targetComponentObj &&
			null != thisComponentObj )
		{
			Vector3 TargetPos = Vector3.zero ;
			Vector3 SrcPos = Vector3.zero ;
			/*
			Debug.Log( "WeaponPhaserEffect() " + " _TargetObject=" + m_WeaponDataShared.TargetComponentObject.name + 
							" _Displacement" + m_WeaponDataShared.m_Displacement ) ;
			//*/
			
			TargetPos = targetComponentObj.transform.position ;
			Vector3 RealTargetPosition = TargetPos + m_WeaponDataShared.m_Displacement ;
			
			SrcPos = thisComponentObj.transform.position ;
			
			Vector3 middlePos = SrcPos + RealTargetPosition ;
			middlePos *= 0.5f ;
			
			// set weapong effect object to correct position and rotation
			this.gameObject.transform.position = middlePos ;		
			
			Vector3 direction = RealTargetPosition - SrcPos ;
	
			this.gameObject.transform.rotation = Quaternion.LookRotation( direction , 
																	 new Vector3( 0 , 1 , 0 ) ) ;

			this.gameObject.transform.localScale = new Vector3( this.gameObject.transform.localScale.x , 
																this.gameObject.transform.localScale.y , 
												   				direction.magnitude / 10.0f ) ;
			
		}
	}
	
	private void TryCauseDamage()
	{
		if( true == m_DamageCauseTimer.IsCountDownToZero() )
		{
			
			CauseDamage() ;
			m_DamageCauseTimer.Rewind() ;
		}
	}
	
	private void CauseDamage()
	{
		if( null != m_WeaponDataShared )
		{
			GameObject unitObj = m_WeaponDataShared.TargetUnitObject ;
			if( null != unitObj )
			{
				UnitDamageSystem dmgSys = unitObj.GetComponent<UnitDamageSystem>() ;
				if( null != dmgSys )
				{
					// 檢查是否打到其他部件
					string targetComponentName = ConstName.SplitComponentObjectNameToComponentName( m_WeaponDataShared.TargetComponentObjectName ) ;
					if( 0 == targetComponentName.Length )
						targetComponentName = m_WeaponDataShared.UnitObjectName ;
					string attackerUnitName = m_WeaponDataShared.UnitObjectName ;
					
					string attackerDisplayName = attackerUnitName ;
					UnitData attackerUnitData = m_WeaponDataShared.ObjUnitData() ;
					if( null != attackerUnitData )
						attackerDisplayName = attackerUnitData.GetUnitDisplayName() ;
					
					// 取得被擊中的部件
					string newUnitName = "" ;
					string realTargetComponentObjectName = "" ;
					if( false == dmgSys.RetrieveRealTargetComponent( m_WeaponDataShared.UnitGameObject ,
																	 targetComponentName , 
																	 ref newUnitName , 
																	 ref realTargetComponentObjectName ) )
					{
						Debug.Log( "dmgSys.RetrieveRealTargetComponent() failed targetComponentName=" + realTargetComponentObjectName ) ;
						return ;
					}
					
					if( newUnitName != m_WeaponDataShared.TargetUnitName ) 
					{
						m_WeaponDataShared.SetupTarget( newUnitName , realTargetComponentObjectName ) ;
#if DEBUG						
						Debug.Log( "newUnitName != m_WeaponDataShared.GameObjectObj.name=" + newUnitName + " m_WeaponDataShared.TargetUnitName=" + m_WeaponDataShared.TargetUnitName ) ;
#endif						
						return ;// cause damage next time.
					}
					
					// Debug.Log( "newUnitName=" + newUnitName ) ;
					// Debug.Log( "orgtargetComponentName=" + targetComponentName ) ;
					// Debug.Log( "realTargetComponentObjectName=" + realTargetComponentObjectName ) ;
					dmgSys.CauseDamageValueOut( attackerUnitName , 
												attackerDisplayName ,
												m_TriggerDamage , 
												realTargetComponentObjectName ) ;
				}
			}
		}
	}
	
}
