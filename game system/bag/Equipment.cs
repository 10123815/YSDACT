/*************************************************************

** Auth: ysd
** Date: 15.8.19
** Desc: 装备
** Vers: v1.0

*************************************************************/

using UnityEngine;
using System.Collections;

public class Equipment : BagItem
{

    public Equipment (ConstantDefine.CollectionType type)
        : base(type)
    {
        bagType = ConstantDefine.BagItemType.Equipment;
    }

    public override void OnUsed ( )
    {
        //替换同样位置上的装备    
    }
}
