/*************************************************************

** Auth: ysd
** Date: 15.8.18
** Desc: 可收集物品，点击后放入背包
         物品有三层表示：GameObject，UI层，Model
         * GameObject层用于玩家拾取，如果是奖励等情况，则没有这一层
         * 获取后，将物品添加至Model层和UI层，一一对应
         * Model层用于数据持久、逻辑
         * UI层用于物品使用的玩家交互
         不是联网物体，各个客户端不同
         此类用于表示GameObject层
** Vers: v1.0

*************************************************************/

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Reflection;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class CollectableItem : MonoBehaviour, IClickable, IGainable
{

    /// <summary>
    /// 只有在这个距离内点击才有效
    /// </summary>
    public float availableDistance = Mathf.Infinity;

    /// <summary>
    /// 可采集次数
    /// </summary>
    public byte collectedTime = 1;

    /// <summary>
    /// 采集时间
    /// </summary>
    public byte collectingTime = 0;

    private MessageDefine.ItemCollectedEvent collectedEvent;

    public ConstantDefine.BagItemType bagType;
    public ConstantDefine.CollectionType itemType;
    public string itemName;

    void Awake ( )
    {
        collectedEvent = new MessageDefine.ItemCollectedEvent();
        //将事件发给各个监听器，任务、成就....
        collectedEvent.AddListener(QuestManager.GetInstance().ItemCollectedEventHandler);
    }

    #region IClickable 成员

    public void OnClick (float distance = Mathf.Infinity)
    {

        if (distance > availableDistance)
            return;

        //采集进度动画
        if (collectingTime != 0)
        {

            //从公用UI中得到
            GameObject canvas;
            if (CommonCanvasManager.GetInstance().OpenCommonCanvas("collecting", out canvas))
            {
                Image collectingSlider = canvas.GetComponentsInChildren<Image>()[1];

                //放在被采集物品的上方
                Vector3 pos = transform.GetPositionOffset(0, 1, 0);
                collectingSlider.transform.parent.position = Camera.main.WorldToScreenPoint(pos);

                //补间动画
                collectingSlider.DOFillAmount(1, collectingTime).OnComplete(delegate
                {
                    collectingSlider.fillAmount = 0;
                    //返回给公用
                    CommonCanvasManager.GetInstance().Return("collecting");
                    OnGot();
                });
            }
        }
        else
            OnGot();
    }

    #endregion

    #region IGainable 成员

    public void OnGot ( )
    {

        //反射，通过物品名（类名）创建这个物品的实例
        Type type = Type.GetType(itemName);
        if (type == null)
        {
            Debug.LogError("没有该物品");
        }
        BagItem item = Activator.CreateInstance(type, new object[1] { itemType }) as BagItem;

        //放入背包
        BagManager.GetInstance().PutIntoBag(item);

        collectedEvent.Invoke(itemType);

        collectedTime--;
        if (collectedTime == 0)
            Destroy(gameObject);
    }

    #endregion

}
