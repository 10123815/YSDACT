/*************************************************************

** Auth: ysd
** Date: 15.8.19
** Desc: 泛型包囊
** Vers: v1.0

*************************************************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Bag<T> where T : BagItem
{

    public class BagItemData
    {
        public byte count;
        public byte index;
        public GameObject itemUI;

        public BagItemData (byte c, byte i, GameObject UI)
        {
            count = c;
            index = i;
            itemUI = UI;
        }
    }

    public ConstantDefine.BagItemType bagType;

    private GameObject m_canvas;
    private RectTransform m_bagGridTF;

    public const byte capacity = 20;

    /// <summary>
    /// 已被放入背包的物品及其数量
    /// </summary>
    private Dictionary<ConstantDefine.CollectionType, BagItemData> m_bagItems;

    public Bag (ConstantDefine.BagItemType type)
    {

        m_bagItems = new Dictionary<ConstantDefine.CollectionType, BagItemData>(capacity);

        bagType = type;
    }

    public byte GetCount(ConstantDefine.CollectionType type)
    {
        if (m_bagItems.ContainsKey(type))
            return m_bagItems[type].count;
        else
            return 0;
    }

    /// <summary>
    /// 想背包中添加物品
    /// </summary>
    public void Add (T item, ConstantDefine.BagItemType bagType, byte count = 1)
    {

        bool opned = m_canvas.activeSelf;
        m_canvas.SetActive(true);
        if (m_bagItems.ContainsKey(item.itemType))
        {
            m_bagItems[item.itemType].count += count;

            //增加数量显示UI
            AddCountForIndexUI(m_bagItems[item.itemType].index, count);
        }
        else if (m_bagItems.Count < capacity)
        {

            //没有该物品，但背包不满


            //创建物品UI
            GameObject itemUIGO = Object.Instantiate(Resources.Load("ui/item/item")) as GameObject;
            itemUIGO.GetComponent<Image>().sprite = Resources.Load<Sprite>("ui/item/" + item.itemInfo.name);
            BagItemUI bagItemUI = itemUIGO.GetComponent<BagItemUI>();
            bagItemUI.bagType = bagType;
            bagItemUI.itemInfo = item.itemInfo;
            bagItemUI.type = item.itemType;
            //当选择此物品时，点击使用就会触发OnUsed
            bagItemUI.onUsedMethod = item.OnUsed;

            m_bagItems.Add(item.itemType, new BagItemData(count, (byte)m_bagItems.Count, itemUIGO));

            //在背包UI中添加
            Transform gridElementTF = m_bagGridTF.GetChild(m_bagItems.Count - 1);
            RectTransform itemUITF = itemUIGO.transform as RectTransform;
            itemUITF.SetParent(gridElementTF, false);
            //只在背景之上
            itemUITF.SetSiblingIndex(1);
            //数量
            Text countText = gridElementTF.GetChild(2).GetComponent<Text>();
            countText.text = m_bagItems[item.itemType].count.ToString();
        }
        else
        {
            //背包满，UI提示
        }

        if (!opned)
            m_canvas.SetActive(false);

    }

    /// <summary>
    /// 移除
    /// </summary>
    public void Remove (ConstantDefine.CollectionType itemType, byte count = 1)
    {
        if (m_bagItems.ContainsKey(itemType))
        {
            byte currentCount = m_bagItems[itemType].count;
            if (currentCount > count)
            {
                m_bagItems[itemType].count -= count;
                AddCountForIndexUI(m_bagItems[itemType].index, count, false);   
            }
            else
            {
                Abondon(itemType);
            }
        }
    }

    public void Abondon (ConstantDefine.CollectionType itemType, string function = "use")
    {
        if (m_bagItems.ContainsKey(itemType))
        {
            Text countText = m_bagGridTF.GetChild(m_bagItems[itemType].index).GetChild(2).GetComponent<Text>();
            countText.text = "";
            GameObject.Destroy(m_bagItems[itemType].itemUI);
            m_bagItems.Remove(itemType);
            CommonCanvasManager.GetInstance().Return(function);
        }
    }

    public void Init(RectTransform bagGridTF, GameObject canvas)
    {
        m_canvas = canvas;
        m_bagGridTF = bagGridTF;
    }

    private void AddCountForIndexUI (byte index, byte increment, bool add = true)
    {
        Text countText = m_bagGridTF.GetChild(index).GetChild(2).GetComponent<Text>();
        //当前UI显示的
        byte c;
        if (!byte.TryParse(countText.text, out c))
        {
            c = 0;
        }
        if (add)
            countText.text = (c + increment).ToString();
        else
            countText.text = (c - increment).ToString();
    }


}
