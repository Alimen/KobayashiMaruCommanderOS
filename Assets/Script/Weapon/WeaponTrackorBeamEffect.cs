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
@file WeaponTrackorBeamEffect.cs
@brief 牽引光束的特效
@author NDark

# 繼承自 WeaponPhaserEffect
# 掛載在牽引光束特效物件上
# Setup() 創造時產生 PullForce 拉力 放在目標物件上
# Deactive() 關閉時摧毀目標物件的拉力
清除 m_WeaponDataShared 不再檢察
# Update()
## 更新特效物件 牽引光束
## m_DamageCountDown 更新對危險單位造成的傷害
## m_MaxMaintainDistanceSquare 假如超過最大維持距離就必須主動關閉牽引光束
# CauseEffectToUnit() 造成傷害及傷害字特效

@date 20121122 . file created.
@date 20121122 by NDark
. add class member m_MaxMaintainDistanceSquare 
. add class method OnDestroy()
@date 20121123 by NDark . add code of mass calculation at Setup()
@date 20121130 by NDark . add class method CauseEffectToUnit()
@date 20121203 by NDark . comment.
@date 20121219 by NDark . re-factor.

*/
using UnityEngine;

public class WeaponTrackorBeamEffect : WeaponPhaserEffect 
{
	
	public float m_MaxMaintainDistanceSquare = 0.0f ;// 最大維持距離平方
	public CountDownTrigger m_DamageCountDown = new CountDownTrigger( 1.0f ) ;// 傷害週期
	// Use this for initialization
	void Start () 
	{
		ClearWeaponDataShared() ;
		m_DamageCountDown.Rewind() ;
	}
	
	void OnDestroy()
	{
		Deactive() ;
	}
	

	// Update is called once per frame
	void Update () 
	{
		if( null != m_WeaponDataShared )
		{
			SetObjFromComponentToTargetComponent() ;			
			
			CheckDamage() ;
			
			CheckTargetExist() ;
			
			CheckDeactive() ;			
		}
	}
	
	// 創造時產生 PullForce 拉力 放在目標物件上
	public override void Setup( WeaponDataSet _WeaponData )
	{
		if( null == _WeaponData )
			return ;
		m_WeaponDataShared = _WeaponData ;


		if( null != m_WeaponDataShared.TargetUnitObject )
		{
			GameObject targetUnit = m_WeaponDataShared.TargetUnitObject ;
			if( null == targetUnit.GetComponent<PullForce>() )
			{
				targetUnit.AddComponent<PullForce>() ;
				Vector3 Distance = m_WeaponDataShared.Component3DObject.transform.position - targetUnit.transform.position ;
				float Mass = 1.0f ;
				Rigidbody rbody = targetUnit.GetComponentInChildren<Rigidbody>() ;
				if( null != rbody )
				{
					Mass = rbody.mass ;
				}
				float MaxSpeed = m_WeaponDataShared.m_CauseDamage / Mass ;
				PullForce force = targetUnit.GetComponent<PullForce>() ;					
				if( null != force )
				{
					force.Setup( m_WeaponDataShared.Component3DObject , 
								 Distance.magnitude , // init distance
								 m_WeaponDataShared.m_CauseDamage ,  // max distance
								 MaxSpeed ) ;
				}
			}
		}
		// Debug.Log( "m_WeaponDataShared.m_CauseDamage" + m_WeaponDataShared.m_CauseDamage ) ;
		m_MaxMaintainDistanceSquare = m_WeaponDataShared.m_CauseDamage * m_WeaponDataShared.m_CauseDamage ;
		
	}
	
	private void CheckDeactive()
	{

		Vector3 TargetPos = Vector3.zero ;
		Vector3 SrcPos = Vector3.zero ;		
		
		GameObject targetComponentObj = m_WeaponDataShared.TargetComponentObject ;
		GameObject thisComponentObj = m_WeaponDataShared.Component3DObject ;
		if( null != targetComponentObj &&
			null != thisComponentObj )
		{
			TargetPos = targetComponentObj.transform.position ;
			SrcPos = thisComponentObj.transform.position ;
		}
			
		Renderer renderer = this.gameObject.GetComponentInChildren<Renderer>() ;
		if( null != renderer )
		{

			Vector3 DistanceVec = TargetPos - SrcPos ;
			if( DistanceVec.sqrMagnitude > m_MaxMaintainDistanceSquare )
			{
				renderer.enabled = false ;
			}
			
			if( false == renderer.enabled )
			{
				Deactive() ;
			}
		}
	}
	
	/*
	 * 關閉時摧毀目標物件的拉力
	 * 清除 m_WeaponDataShared 不再檢察
	 */
	private void Deactive()
	{
		if( null != m_WeaponDataShared &&
			null != m_WeaponDataShared.TargetUnitObject )
		{
			GameObject targetUnit = m_WeaponDataShared.TargetUnitObject ;
			PullForce force = targetUnit.GetComponent<PullForce>() ;
			if( null != force )
			{
				// Debug.Log( "Destroy( force " );
				force.DestroyInSec( 3.0f ) ;
			}
		}
		
		ClearWeaponDataShared() ;
	}
	
	private void CheckDamage()
	{
		// 製造可能的傷害
		if( null != m_WeaponDataShared.TargetUnitObject &&
			true == m_DamageCountDown.IsCountDownToZero() )
		{
			UnitData unitData = m_WeaponDataShared.TargetUnitObject.GetComponent<UnitData>() ;
			UnitDamageSystem dmgSys = m_WeaponDataShared.TargetUnitObject.GetComponent<UnitDamageSystem>() ;
			
			if( null != unitData &&
				null != dmgSys &&
				true == unitData.componentMap.ContainsKey( ConstName.UnitDataComponentUnitIntagraty ) )
			{
				UnitComponentData intagraty = unitData.componentMap[ConstName.UnitDataComponentUnitIntagraty] ;
				if( intagraty.componentStatus == ComponentStatus.Danger )
				{
					CauseEffectToUnit( dmgSys ) ;
					m_DamageCountDown.Rewind() ;
				}
			}
		}
	}
	
	// 造成傷害及傷害字特效
	private void CauseEffectToUnit( UnitDamageSystem _dmgSys )
	{
		float damagevalue = 10.0f * Time.deltaTime ;

		string attackerDisplayName = m_WeaponDataShared.UnitObjectName ;
		UnitData attackerUnitData = m_WeaponDataShared.ObjUnitData() ;
		if( null != attackerUnitData )
			attackerDisplayName = attackerUnitData.GetUnitDisplayName() ;
		
	
		_dmgSys.ActiveDamageNumberEffectNextTime( true , 1 ) ;		
		_dmgSys.CauseDamageValueOut( m_WeaponDataShared.UnitObjectName ,
									 attackerDisplayName , 
									 damagevalue , 
								 	 ConstName.UnitDataComponentUnitIntagraty ) ;
	}
		
}
