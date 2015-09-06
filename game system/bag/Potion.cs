/*************************************************************

** Auth: ysd
** Date: 
** Desc: 
** Vers: v1.0

*************************************************************/

using UnityEngine;
using System.Collections;

public class Potion : BagItem
{

    public Potion (ConstantDefine.CollectionType type)
        : base(type)
    {
        bagType = ConstantDefine.BagItemType.Potion;
    }


    public override void OnUsed ( )
    {
        //消耗品
        BagManager.GetInstance().potionBag.Remove(itemType);
    }
}
