/*************************************************************

** Auth: ysd
** Date: 15.8.21
** Desc: 被多个方法使用的UI的管理器
** Vers: v1.0

*************************************************************/

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class CommonCanvasManager
{

    public Dictionary<string, GameObject> m_commonCanvases;

    private CommonCanvasManager ( )
    {
        m_commonCanvases = new Dictionary<string, GameObject>();
    }

    static public CommonCanvasManager GetInstance ( )
    {
        return Singleton<CommonCanvasManager>.GetInstance();
    }

    /// <summary>
    /// 获取某个通用UI的引用
    /// </summary>
    /// <param name="name">UI的名字</param>
    /// <param name="canvas">返回的UI</param>
    /// <param name="tween">是否有动画</param>
    /// <returns>如果该UI正在被使用，则返回false</returns>
    public bool OpenCommonCanvas (string name, out GameObject canvas, bool tween = false)
    {
        if (!m_commonCanvases.ContainsKey(name))
        {
            GameObject go = GameObject.Instantiate(Resources.Load("ui/" + name)) as GameObject;
            m_commonCanvases.Add(name, go);
            //刚创建的没在被使用
            m_commonCanvases[name].SetActive(false);
        }
        bool inUsed = m_commonCanvases[name].activeSelf;
        canvas = m_commonCanvases[name];
        canvas.SetActive(true);
        if (tween && !inUsed)
            _OpenCommonUITween(canvas.transform.GetChild(0).rectTransform());
        //正在使用为不可用（有例外）
        return !inUsed;
    }

    public bool Return (string name, bool tween = false)
    {
        if (m_commonCanvases.ContainsKey(name))
        {
            if (tween)
                _CloseCommonUITween(m_commonCanvases[name]);
            else if (m_commonCanvases[name] != null)
                m_commonCanvases[name].SetActive(false);
            return true;
        }
        return false;
    }

    private void _CloseCommonUITween (GameObject canvas)
    {
        CanvasGroup canvasGroup = canvas.GetComponentInChildren<CanvasGroup>();
        RectTransform uitf = canvas.transform.GetChild(0).rectTransform();
        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            Sequence seq = DOTween.Sequence();
            seq.Append(uitf.DOScale(0.5f, 0.2f)).Join(canvasGroup.DOFade(0, 0.2f)).OnComplete(delegate
            {
                canvas.SetActive(false);
            });
        }
        else
        {
            uitf.DOScale(0.5f, 0.2f).OnComplete(delegate
            {
                canvas.SetActive(false);
            });
        }
    }

    private void _OpenCommonUITween (RectTransform uitf)
    {
        uitf.localScale = Vector3.one * 0.5f;
        CanvasGroup canvasGroup = uitf.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            Sequence seq = DOTween.Sequence();
            seq.Append(uitf.DOScale(1, 0.2f)).Join(canvasGroup.DOFade(1, 0.2f)).OnComplete(delegate
            {
                canvasGroup.interactable = true;
            });
        }
        else
        {
            uitf.DOScale(1, 0.2f);
        }
    }

    /// <summary>
    /// 显示各种提示，显示时某些UI不活动
    /// </summary>
    /// <param name="msg">要显示的信息</param>
    /// <param name="canvasGroups">不活动的UI</param>
    /// <param name="seconds">显示时间，不为0自动关闭，为0由其他代码控制</param>
    public GameObject ShowMessage (string msg, CanvasGroup[] canvasGroups, float seconds = 0)
    {
        GameObject canvas;
        OpenCommonCanvas("message", out canvas);
        canvas.GetComponentInChildren<Text>().text = msg;
        //不活动UI
        if (canvasGroups != null && canvasGroups.Length > 0)
        {
            for (int i = 0; i < canvasGroups.Length; i++)
            {
                if (canvasGroups[i] != null)
                {
                    //canvasGroups[i].alpha = 0;
                    canvasGroups[i].interactable = false;
                }
            } 
        }
        if (seconds > 0)
        {
            CoroutineManager.GetInstance().StartCoroutine(_CloseMessagePanel(canvasGroups, seconds));
        }
        return canvas;
    }

    /// <summary>
    /// 关闭信息Panel
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="canvasGroups"></param>
    /// <param name="seconds"></param>
    /// <returns></returns>
    private IEnumerator _CloseMessagePanel (CanvasGroup[] canvasGroups, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (canvasGroups != null && canvasGroups.Length > 0)
        {
            for (int i = 0; i < canvasGroups.Length; i++)
            {
                if (canvasGroups[i] != null)
                {
                    canvasGroups[i].alpha = 1;
                    canvasGroups[i].interactable = true;
                }
            } 
        }
        Return("message");
    }

    public void CloseMessagePanel (CanvasGroup[] canvasGroups)
    {
        if (canvasGroups != null && canvasGroups.Length > 0)
        {
            for (int i = 0; i < canvasGroups.Length; i++)
            {
                if (canvasGroups[i] != null)
                {
                    canvasGroups[i].alpha = 1;
                    canvasGroups[i].interactable = true;
                }
            }
        }
        Return("message");
    }

}