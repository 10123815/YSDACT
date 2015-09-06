/*************************************************************

** Auth: ysd
** Date: 8.19
** Desc: 背包操作
** Vers: v1.0

*************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BagManager
{

    public Bag<Equipment> equipmentBag;

    public Bag<Potion> potionBag;

    public Bag<Sundry> sundryBag;

    public static BagManager GetInstance()
    {
        return Singleton<BagManager>.GetInstance();
    }

    private BagManager ( )
    {

        //各种背包的初始化，在使用之前
        equipmentBag = new Bag<Equipment>(ConstantDefine.BagItemType.Equipment);
        potionBag = new Bag<Potion>(ConstantDefine.BagItemType.Potion);
        sundryBag = new Bag<Sundry>(ConstantDefine.BagItemType.Sundry);

    }

    public void PutIntoBag (BagItem item, byte count = 1)
    {
        switch (item.bagType)
        {
            case ConstantDefine.BagItemType.Equipment:
                equipmentBag.Add((Equipment)item, item.bagType, count);
                break;
            case ConstantDefine.BagItemType.Potion:
                potionBag.Add((Potion)item, item.bagType, count);
                break;
            case ConstantDefine.BagItemType.Sundry:
                sundryBag.Add((Sundry)item, item.bagType, count);
                break;
            case ConstantDefine.BagItemType.Money:
                break;
            default:
                break;
        }
    }    

}
