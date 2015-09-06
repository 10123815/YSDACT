/*************************************************************

** Auth: ysd
** Date: 15.8.19
** Desc: 小型血药
** Vers: v1.0

*************************************************************/

using UnityEngine;
using System.Collections;

public class MinorHealthPotion : Potion
{

    public float healthingRate;

    public MinorHealthPotion(ConstantDefine.CollectionType type)
        :base(type)
    {
    
    }

    public override void OnUsed ( )
    {
        base.OnUsed();

        //玩家回血
        Debug.Log("回血");
    }
}