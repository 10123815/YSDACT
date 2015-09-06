/*************************************************************

** Auth: ysd
** Date: 
** Desc: 
** Vers: v1.0

*************************************************************/

using UnityEngine;
using System.Collections;

public class Sundry : BagItem
{

    public Sundry (ConstantDefine.CollectionType type)
        :base(type)
    {
        bagType = ConstantDefine.BagItemType.Sundry;
    }

    public override void OnUsed ( )
    {
        //什么都不做
    }
}
