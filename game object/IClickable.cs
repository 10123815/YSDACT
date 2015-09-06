/*************************************************************

** Auth: ysd
** Date: 15.7.25
** Desc: 可点击的游戏物体
** Vers: v1.0

*************************************************************/

using UnityEngine;
using System.Collections;

public interface IClickable
{

    /// <summary>
    /// 当该游戏物体被点击时
    /// </summary>
    /// <param name="distance">玩家和被点击物体的距离</param>
    void OnClick (float distance = Mathf.Infinity);

}