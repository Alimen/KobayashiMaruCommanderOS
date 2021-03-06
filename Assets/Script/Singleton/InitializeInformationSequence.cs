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
@file InitializeInformationSequence.cs
@author NDark

初始化資訊頁面群

# 戰後會有資訊頁面的流程，主要目的是給予玩家其他資訊（招募人員，資源路徑清單，或是工作人員，小提示等等）
# 原本的作法是戰後的按鈕直接導向各頁面，然後再由各頁面串接，最後連到回到起始（選擇關卡，或開頭）。
# 也就是把場景轉換的順序寫在場景（Scene）中。較為不彈性（如果要修改必須開Unity一個個場景做調整）
# 目前的作法是遊戲勝負已分就到資訊起始場景Scene_InformationHead，此場景是空的場景。
# 資訊起始場景用來讀入參數檔，並且設定場景參數到GlobalSingleton中。
# 之後（包含資訊起始場景）的所有資訊頁面都是去檢察此設定，然後切入下一個場景。
# 目前相關的運作
## ClickOnMessageCard_BackToSelectScene.cs 取得最終跳躍場景名稱
## LevelGenerator.cs 設定最終跳躍場景名稱
## VictoryEventManager.cs 設定最終跳躍場景名稱

@date 20130206 file stared.

*/
using UnityEngine;
using System.Collections;

public class InitializeInformationSequence : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		GlobalSingleton.InitializeInformationSequence() ;// 強制重置
		GlobalSingleton.TryLoadInformationNext() ;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
