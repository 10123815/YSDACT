/*************************************************************

** Auth: ysd
** Date: 15.8.24
** Desc: 奖励UI
** Vers: v1.0

*************************************************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public struct RewardItemData
{

    [SerializeField]
    public Image itemImage;
    [SerializeField]
    public Text count;
}

public class RewardUI : MonoBehaviour
{

    public Text expText;
    public Text goldText;
    [SerializeField]
    public RewardItemData[] rewardItemDatas;

    public void Init(int exp, int gold, Sprite[] sprites, byte[] counts)
    {
        expText.text = "GOLD:" + exp.ToString();
        goldText.text = "EXP:" + gold.ToString();
        for (int i = 0; i < rewardItemDatas.Length; i++)
        {
            if (i < sprites.Length)
            {
                rewardItemDatas[i].count.text = counts[i].ToString();
                rewardItemDatas[i].itemImage.sprite = sprites[i];
            }
            else
            {
                rewardItemDatas[i].itemImage.gameObject.SetActive(false);
            }
        }
    }

    
}