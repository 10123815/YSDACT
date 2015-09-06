/*************************************************************

** Auth: ysd
** Date: 15.8.19
** Desc: 可放入背包中的物品，可能在背包、商店、奖励等处
** Vers: v1.0

*************************************************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 用于在背包UI中显示物品，其实是一个Button
/// </summary>
[RequireComponent(typeof(Button))]
public class BagItemUI : MonoBehaviour
{

    /// <summary>
    /// 放在那个背包里
    /// </summary>
    public ConstantDefine.BagItemType bagType;

    /// <summary>
    /// 可放入背包中的物品一定是可获取物
    /// </summary>
    public ConstantDefine.CollectionType type;

    [System.NonSerialized]
    public CollectableItemInfo itemInfo;

    /// <summary>
    /// 无论何时点击UI时，普通状态、交易、领取奖励....
    /// </summary>
    private Button m_onCheckedButton;

    /// <summary>
    /// 普通状态下打开物品详情，点击使用时；
    /// 点一个物品UI时更换使用按钮的行为
    /// </summary>
    public delegate void OnUsed ( );
    public OnUsed onUsedMethod;

    /// <summary>
    /// 使用面板
    /// </summary>
    private Button m_onUsedCanvas;
    private Button m_onBoughtCanvas;
    private Button m_onSoldCanvas;

    void Start ( )
    {
        //初始化为普通状态
        ChangeFunction(OpenToDisplayDetail);
    }

    /// <summary>
    /// 改变按键的功能，使用、出售等
    /// </summary>
    /// <param name="onChecked">被点击时的行为</param>
    public void ChangeFunction (DelegateDefine.OnBagItemChecked onChecked)
    {
        if (m_onCheckedButton == null)
        {
            m_onCheckedButton = GetComponent<Button>();
        }
        m_onCheckedButton.onClick.RemoveAllListeners();
        m_onCheckedButton.onClick.AddListener(delegate
        {
            onChecked();
        });
    }

    /// <summary>
    /// 普通状态打开背包，显示物品信息；
    /// 根据物品类型，装备/药可以使用，杂物（包括任务物品）不可使用
    /// </summary>
    public void OpenToDisplayDetail ( )
    {
        GameObject useCanvas;
        //该UI正被使用时，直接替换显示内容
        CommonCanvasManager.GetInstance().OpenCommonCanvas("use", out useCanvas);
        Transform panelTF = useCanvas.transform.Find("Panel");
        panelTF.Find("Image/pic").GetComponent<Image>().sprite = GetComponent<Image>().sprite;
        panelTF.Find("name").GetComponent<Text>().text = itemInfo.name;
        panelTF.Find("description").GetComponent<Text>().text = itemInfo.info;

        Button useButton = panelTF.Find("use").GetComponent<Button>();
        //使用
        useButton.enabled = true;
        // 把使用功能改成自己的
        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(delegate
        {
            onUsedMethod();
        });

        //丢弃
        Button abondonButton = panelTF.Find("abondon").GetComponent<Button>();
        abondonButton.onClick.RemoveAllListeners();
        abondonButton.onClick.AddListener(delegate
        {
            BagManager bm = BagManager.GetInstance();
            switch (bagType)
            {
                case ConstantDefine.BagItemType.Equipment:
                    bm.equipmentBag.Abondon(type);
                    break;
                case ConstantDefine.BagItemType.Potion:
                    bm.potionBag.Abondon(type);
                    break;
                case ConstantDefine.BagItemType.Sundry:
                    bm.sundryBag.Abondon(type);
                    break;
                case ConstantDefine.BagItemType.Money:
                    break;
                default:
                    break;
            }
        });
    }

    public void OpenToSell ( )
    {
    }

    public void OpenToBuy ( )
    {
    }

    /// <summary>
    /// 丢弃
    /// </summary>
    public void Abondon ( )
    {

    }

    /// <summary>
    /// 关闭UI
    /// </summary>
    public void Close ( )
    {
        CommonCanvasManager.GetInstance().Return(name);
    }

}
