/*************************************************************

** Auth: ysd
** Date: 15.7.24
** Desc: 控制tabs
** Vers: v1.0

*************************************************************/

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System;

/// <summary>
/// tab及其内容框架
/// </summary>
[Serializable]
public struct ToggleContent
{
    [SerializeField]
    public Toggle toggle;

    [SerializeField]
    public GameObject panel;

    [SerializeField]
    public Text label;
}

public class ToggleController : MonoBehaviour
{

    public enum ContentChangeMode : byte
    {
        Simple = 0,
        HorizontalTween = 1,
        VerticalTween = 2
    }

    public ContentChangeMode mode = ContentChangeMode.Simple;

    private byte m_clickedIndex;

    [SerializeField]
    public ToggleContent[] tabs;

    public bool changeColor = false;

    public Color onColor;
    public Color offColor;

    private float m_width;
    private float m_height;

    public void Awake ( )
    {

    }

    public void Start ( )
    {
        for (byte i = 0; i < tabs.Length; i++)
        {
            byte index = i;
            tabs[i].toggle.onValueChanged.AddListener(delegate
            {
                OnSelect(index);
            });
        }
        Vector2 size = ((RectTransform)tabs[0].panel.transform).sizeDelta;
        m_width = size.x;
        m_height = size.y;
    }

    public void Update ( )
    {

    }

    private void OnSelect (byte index)
    {

        if (index == m_clickedIndex)
            return;

        switch (mode)
        {
            case ContentChangeMode.Simple:
                tabs[m_clickedIndex].panel.SetActive(false);
                tabs[index].panel.SetActive(true);
                break;
            case ContentChangeMode.HorizontalTween:
                for (int i = 0; i < tabs.Length; i++)
                {
                    RectTransform tf = tabs[i].panel.transform as RectTransform;
                    tf.DOAnchorPos(tf.anchoredPosition + new Vector2(m_width * (m_clickedIndex - index), 0), 0.2f);
                } 
                break;
            case ContentChangeMode.VerticalTween:
                break;
            default:
                break;
        }

        if (changeColor)
        {
            tabs[m_clickedIndex].label.color = offColor;
            tabs[index].label.color = onColor;
        }

        m_clickedIndex = index;

    }

}