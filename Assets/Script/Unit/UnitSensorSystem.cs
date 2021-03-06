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
@file UnitSensorSystem.cs
@brief 自動收集附近的單位清單
@author NDark

# 自動收集附近的單位清單
# 有一個 單位清單 m_SensorUnitList
# 收集距離為 m_SensorDistance 會不斷跟UnitData更新
# m_RefreshTimer 一定周期才更新，請參考BaseDefine.SENSOR_REFRESH_SEC
# GetClosestObj() 取得最近單位
# CheckSensorDistance() 取得感測部件的功效
# CheckUnitAround() 重新檢查附近的單位

@date 20121115 file started.
@date 20121115 by NDark . add class method GetClosestObj()
@date 20121124 by NDark 
. add checking of null object
. add sensor range from UnitData at Start()
. add class method CheckSensorDistance()
@date 20121207 by NDark . add code of checking alive at CheckUnitAround()

*/
using UnityEngine;
using System.Collections.Generic;

public class UnitSensorSystem : MonoBehaviour 
{
	public float m_SensorDistance = 0.0f ; // 收集距離
	public CountDownTrigger m_RefreshTimer = new CountDownTrigger( BaseDefine.SENSOR_REFRESH_SEC ) ; // 一定周期才更新
	public List<NamedObject> m_SensorUnitList = new List<NamedObject>() ; // 單位清單 
	
	// 取得最近單位
	public GameObject GetClosestObj()
	{
		GameObject ret = null ;
		float minDistanceSquare = 0.0f ;
		List<NamedObject>.Enumerator e = m_SensorUnitList.GetEnumerator () ;
		while( e.MoveNext() )
		{
			if( null == e.Current.GetObj() )
				continue ;
			
			Vector3 vec = e.Current.Obj.transform.position - this.gameObject.transform.position ;
			if( null == ret )
			{
				ret = e.Current.Obj ;
				minDistanceSquare = vec.sqrMagnitude ;
			}
			else if( vec.sqrMagnitude < minDistanceSquare )
			{
				ret = e.Current.Obj ;
				minDistanceSquare = vec.sqrMagnitude ;
			}
		}
		return ret ;
	}
	// Use this for initialization
	void Start () 
	{
		m_RefreshTimer.Rewind() ;
	}
	
	// 取得感測部件的功效
	private void CheckSensorDistance()
	{
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( null != unitData )
		{
			UnitComponentData sensor = unitData.GetSensorComponent() ;
			if( null != sensor )
			{
				m_SensorDistance = sensor.TotalEffect()  ;
				// Debug.Log( "m_SensorDistance=" + m_SensorDistance );
			}
		}
	}
	
	// 重新檢查附近的單位
	private void CheckUnitAround()
	{
		MainUpdate mainUpdate = GlobalSingleton.GetMainUpdateComponent() ;
		if( null == mainUpdate )
			return ;
		m_SensorUnitList.Clear() ;
		float diatanceSquare = m_SensorDistance * m_SensorDistance ;
		foreach( NamedObject unit in mainUpdate.m_UnitNamesList )
		{
			if( null != unit.Obj && 
				this.gameObject != unit.Obj )
			{
				UnitData unitData = unit.Obj.GetComponent<UnitData>() ;
				if( false == unitData.IsAlive() )
					continue ;
				
				Vector3 vec = unit.Obj.transform.position - this.gameObject.transform.position ;
				if( vec.sqrMagnitude < diatanceSquare )
				{
					m_SensorUnitList.Add( new NamedObject( unit ) ) ;
				}
			}
		}
	}
	// Update is called once per frame
	void Update () 
	{
		if( true == m_RefreshTimer.IsCountDownToZero() )
		{
			CheckSensorDistance() ;
			CheckUnitAround() ;
			m_RefreshTimer.Rewind() ;
		}
	}
}
