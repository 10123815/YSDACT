/*************************************************************

** Auth: ysd
** Date: 
** Desc: 
** Vers: v1.0

*************************************************************/

using UnityEngine;
using System.Collections;

public class Key : Sundry
{

    public Key (ConstantDefine.CollectionType type)
        : base(type)
    {
        bagType = ConstantDefine.BagItemType.Sundry;
    }

    public override void OnUsed ( )
    {

    }
}