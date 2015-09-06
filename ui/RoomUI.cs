/*************************************************************

** Auth: ysd
** Date: 15.9.3
** Desc: 显示角色信息、菜单栏、虚拟摇杆等
** Vers: v1.0

*************************************************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class RoomUI : MonoBehaviour
{

    private RectTransform m_tf;

    private Transform m_menuTf;
    private Transform m_menuButtonTf;
    private CanvasGroup m_menuCanvasGroup;
    private bool m_isMenuOpened = false;

    void Start ( )
    {
        _InitRoomUI();
    }

    private void _InitRoomUI ( )
    {
        m_tf = transform.rectTransform();
        m_menuTf = m_tf.Find("menu").GetChild(0);
        m_menuButtonTf = m_tf.Find("menu").GetChild(1);
        m_menuCanvasGroup = m_menuTf.GetComponent<CanvasGroup>();
    }

    public void OpenMenu ( )
    {
        if (m_isMenuOpened)
        {
            // 关闭菜单
            Sequence seq = DOTween.Sequence();
            m_menuCanvasGroup.interactable = false;
            seq.Append(m_menuCanvasGroup.DOFade(0, 0.2f)).Join(m_menuButtonTf.DORotate(new Vector3(0, 0, 180), 0.2f));
            for (int i = 0; i < m_menuTf.childCount; i++)
            {
                seq.Join(m_menuTf.GetChild(i).rectTransform().DOAnchorPos(Vector2.zero, 0.2f));
            }
            seq.AppendCallback(( ) =>
            {
                m_isMenuOpened = false;
            });
        }
        else
        {
            // 打卡菜单
            Sequence seq = DOTween.Sequence();
            seq.Append(m_menuCanvasGroup.DOFade(1, 0.2f)).Join(m_menuButtonTf.DORotate(Vector3.zero, 0.2f));
            for (int i = 0; i < m_menuTf.childCount; i++)
            {
                seq.Join(m_menuTf.GetChild(i).rectTransform().DOAnchorPos(new Vector2(80 * (i + 1), 0), 0.2f));
            }
            seq.AppendCallback(( ) =>
            {
                m_menuCanvasGroup.interactable = true;
                m_isMenuOpened = true;
            });
        }
    }

    public void OpenBag (Button button)
    {
        GameObject canvas;
        CommonCanvasManager.GetInstance().OpenCommonCanvas(button.name, out canvas, true);
        RectTransform panelTF = canvas.transform.GetChild(0).rectTransform();

        // 需要打开时在初始化
        BagManager bm = BagManager.GetInstance();
        bm.equipmentBag.Init(GameObject.FindGameObjectWithTag("equipment bag").transform.rectTransform(), canvas);
        bm.potionBag.Init(GameObject.FindGameObjectWithTag("potion bag").transform.rectTransform(), canvas);
        bm.sundryBag.Init(GameObject.FindGameObjectWithTag("sundry bag").transform.rectTransform(), canvas);

        button.onClick.AddListener(delegate
        {
            Sequence seq = DOTween.Sequence();
            if (!canvas.activeSelf)
            {
                canvas.SetActive(true);
                CanvasGroup canvasGroup = panelTF.GetComponent<CanvasGroup>();
                seq.Append(canvasGroup.DOFade(1, 0.2f))
                   .Join(panelTF.DOScale(1, 0.2f))
                   .AppendCallback(delegate
                   {
                       canvasGroup.interactable = true;
                   });
            }
        });

    }

}