/*************************************************************

** Auth: ysd
** Date: 15.8.26
** Desc: 商人，与Quser类似
** Vers: v1.0

*************************************************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Businessman : NPC, IClickable
{

    static public Businessman instance;

    public float availableDistance = 3;

    private GameObject m_shopCanvas;

    private Transform[] m_toggleTFs;
    private byte m_selectedIndex;

    void Start ( )
    {
        instance = this;
        _InitShop();
    }

    private void _InitShop ( )
    {
        //商店里的所有货物，从Xml，添加的/原背包中的BagItemUI的功能是买卖
    }

    #region IClickable 成员

    public void OnClick (float distance = Mathf.Infinity)
    {

        if (distance > availableDistance)
            return;

        //打开商店面板
        CommonCanvasManager.GetInstance().OpenCommonCanvas("bag", out m_shopCanvas, true);
        if (m_toggleTFs == null)
        {
            ToggleController toggleController = m_shopCanvas.GetComponentInChildren<ToggleController>();
            m_toggleTFs = new Transform[toggleController.tabs.Length];
            for (byte i = 0; i < toggleController.tabs.Length; i++)
            {
                Toggle toggle = toggleController.tabs[i].toggle;
                m_toggleTFs[i] = toggle.transform;
                toggle.gameObject.SetActive(true);
                byte index = i;
                toggle.onValueChanged.AddListener(delegate
                {
                    _OnSelect(index);
                });
            }
            Button closeButton = m_shopCanvas.transform.GetChild(0).Find("close").GetComponent<Button>();
            closeButton.onClick.AddListener(delegate
            {
                _Close();
            });
        }
        else
        {
            m_toggleTFs[0].GetComponent<Toggle>().isOn = true;
            m_toggleTFs[0].SetAsFirstSibling();
            m_toggleTFs[1].GetComponent<Toggle>().isOn = false;
            m_toggleTFs[1].SetAsLastSibling();
            for (int i = 0; i < m_toggleTFs.Length; i++)
            {
                m_toggleTFs[i].gameObject.SetActive(true);
            }
        }
    }

    #endregion

    private void _OnSelect (byte index)
    {
        m_toggleTFs[m_selectedIndex].SetAsFirstSibling();
        m_toggleTFs[index].SetAsLastSibling();
        m_selectedIndex = index;
    }

    private void _Close ( )
    {
        m_toggleTFs[0].GetComponent<Toggle>().isOn = false;
        m_toggleTFs[1].GetComponent<Toggle>().isOn = true;
        m_toggleTFs[0].gameObject.SetActive(false);
        m_toggleTFs[1].gameObject.SetActive(false);
    }

}