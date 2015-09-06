/*************************************************************

** Auth: ysd
** Date: 15.8.18
** Desc: 可调查的物品，点击后弹出UI显示信息
** Vers: v1.0

*************************************************************/

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public struct ViewableItemInfo
{
    public string name;
    public string info;
}

[RequireComponent(typeof(Collider))]
public class ViewableItem : MonoBehaviour, IClickable
{

    /// <summary>
    /// 有效的点击距离
    /// </summary>
    public float availableDistance = Mathf.Infinity;

    private ViewableItemInfo m_itemInfo;

    public ConstantDefine.UnCollectionType itemType;

    private GameObject m_itemInfoPanel;

    void Start ( )
    {
        //根据Type从Xml中获取物品描述
        m_itemInfo = CommonUtility.GetViewableItemInfoFromXmlByType(itemType);
    }

    #region IClickable 成员

    public void OnClick (float distance = Mathf.Infinity)
    {

        if (distance > availableDistance)
            return;

        m_itemInfoPanel = GameObject.FindGameObjectWithTag("viewable item info");
        if (m_itemInfoPanel == null)
        {
            _CreateItemInfoPanel();
        }

        m_itemInfoPanel.transform.position = Vector3.zero;
        CanvasGroup canvasGroup = m_itemInfoPanel.GetComponent<CanvasGroup>();
        Text[] texts = m_itemInfoPanel.GetComponentsInChildren<Text>();
        texts[0].text = m_itemInfo.name;
        texts[1].text = m_itemInfo.info;

        Sequence seq = DOTween.Sequence();
        seq.Append(m_itemInfoPanel.transform.GetChild(0).DOScale(1, 0.2f))
            .Join(canvasGroup.DOFade(1, 0.2f))
            .AppendCallback(delegate
            {
                canvasGroup.interactable = true;
            });

    }

    #endregion

    private void _CreateItemInfoPanel ( )
    {
        m_itemInfoPanel = Instantiate(Resources.Load("ui/info")) as GameObject;
        CanvasGroup canvasGroup = m_itemInfoPanel.GetComponent<CanvasGroup>();
        m_itemInfoPanel.GetComponentInChildren<Button>().onClick.AddListener(delegate
        {
            canvasGroup.interactable = false;
            Sequence seq = DOTween.Sequence();
            //补间动画缩放
            seq.Append(m_itemInfoPanel.transform.GetChild(0).DOScale(0.5f, 0.2f))
                //淡出
                .Join(canvasGroup.DOFade(0, 0.2f))
                //序列完成后回调
                .AppendCallback(delegate
                {
                    m_itemInfoPanel.transform.position = new Vector3(10000, 10000, 0);
                });
        });
    }

}
