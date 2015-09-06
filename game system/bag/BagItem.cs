
/*************************************************************

** Auth: ysd
** Date: 15.8.19
** Desc: 背包物品基类，可收集物品的Model层
** Vers: v1.0

*************************************************************/

using UnityEngine;
using System.Collections;

public struct CollectableItemInfo
{
    public string name;
    public string info;
}

abstract public class BagItem
{

    public ConstantDefine.BagItemType bagType;

    public ConstantDefine.CollectionType itemType;

    public CollectableItemInfo itemInfo;

    public BagItem(ConstantDefine.CollectionType type)
    {
        itemType = type;
        itemInfo = CommonUtility.GetCollectableItemInfoFromXmlByType(type);
    }

    /// <summary>
    /// 当使用这个物品时
    /// </summary>
    abstract public void OnUsed ( );

}