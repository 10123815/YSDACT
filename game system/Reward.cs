/*************************************************************

** Auth: ysd
** Date: 15.8.24
** Desc: 各种奖励的基类，杀敌，任务奖励，成就奖励，隐藏物品....
         不一定需要UI界面
** Vers: v1.0

*************************************************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Reward
{

    /// <summary>
    /// 只有三种奖励，经验，钱，物品
    /// </summary>
    public int exp;
    public int gold;
    public BagItem[] items;
    public byte[] counts;

    public Reward ( )
    {
    }

    /// <summary>
    /// 得到某些奖励
    /// </summary>
    /// <param name="ui"></param>
    public void OnGot ( )
    {
        PlayerData.GetInstance().intValues["exp"] += exp;
        PlayerData.GetInstance().intValues["gold"] += gold;
        BagManager bm = BagManager.GetInstance();
        for (int i = 0; i < items.Length; i++)
        {
            bm.PutIntoBag(items[i], counts[i]);
        }
    }

    public void DisplayRewardUI ( )
    {
        GameObject rewardCanvas;
        CommonCanvasManager.GetInstance().OpenCommonCanvas("reward", out rewardCanvas, true);
        Sprite[] sprites = new Sprite[items.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i] = Resources.Load<Sprite>("ui/item/" + items[i].itemInfo.name);
        }
        RewardUI rewardUI = rewardCanvas.GetComponentInChildren<RewardUI>();
        rewardUI.Init(exp, gold, sprites, counts);
        Button button = rewardUI.GetComponentInChildren<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(delegate
        {
            OnGot();
            //领完就关闭
            CommonCanvasManager.GetInstance().Return("reward", true);
        });
    }
    
}