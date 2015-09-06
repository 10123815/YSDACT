/*************************************************************

** Auth: ysd
** Date: 15.8.19
** Desc: 可获得的物品接口
** Vers: v1.0

*************************************************************/

using UnityEngine;
using System.Collections;

public interface IGainable
{

    /// <summary>
    /// 当玩家得到该物品时触发，例如当得到某个任务物品时，触发得到物品事件;
    /// </summary>
    void OnGot ( );

}
