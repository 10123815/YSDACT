/*************************************************************

** Auth: ysd
** Date: 15.7.25
** Desc: 任务发布者,只有任务面板的UI操作
** Vers: v1.0

*************************************************************/

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class Quester : NPC, IClickable
{

    private GameObject m_questPanel;
    private Image m_questTypeImage;
    private Text m_questTypeText;
    private Text m_questDescText;

    private List<Transform> m_questButtons;

    /// <summary>
    /// 当前玩家可选的任务
    /// </summary>
    private List<Quest> m_availableQuests;

    /// <summary>
    /// 用于确定接受/提交任务
    /// </summary>
    private Button m_confirmButton;
    private Text m_confirmText;

    private byte m_selectedQuestIndex = 255;

    public void Awake ( )
    {

    }

    public void Start ( )
    {
        m_questButtons = new List<Transform>();
        m_npcType = ConstantDefine.NPCType.Quester;

        m_availableQuests = QuestManager.GetInstance().allQuests;
    }

    #region IClickable 成员

    public void OnClick (float distance = Mathf.Infinity)
    {
        //打开任务面板
        if (m_questPanel == null)
        {
            _InitQuestPanel();
        }

        if (!m_questPanel.activeSelf)
        {
            m_questPanel.SetActive(true);
            CanvasGroup canvasGroup = m_questPanel.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;

            Sequence seq = DOTween.Sequence();
            //透明度，大小补间动画
            seq.Append(canvasGroup.DOFade(1, 0.2f)).Join(m_questPanel.transform.GetChild(0).DOScale(1.2f, 0.2f)).AppendCallback(delegate
            {
                //动画结束后
                canvasGroup.interactable = true;
                OnSelectQuest(0);
            });
        }
    }

    #endregion

    protected void _InitQuestPanel ( )
    {
        m_questPanel = Instantiate(Resources.Load("ui/quest/quests panel")) as GameObject;

        //close button
        m_questPanel.transform.GetChild(0).GetChild(1).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate
        {
            CanvasGroup canvasGroup = m_questPanel.GetComponent<CanvasGroup>();
            canvasGroup.interactable = false;

            Sequence seq = DOTween.Sequence();
            seq.Append(canvasGroup.DOFade(0, 0.2f)).Join(m_questPanel.transform.GetChild(0).DOScale(0.5f, 0.2f)).AppendCallback(delegate
            {
                m_questPanel.SetActive(false);
            });
        });

        //confirm button
        m_confirmButton = GameObject.FindGameObjectWithTag("confirm quest").GetComponent<Button>();
        m_confirmText = m_confirmButton.GetComponentInChildren<Text>();
        m_confirmButton.onClick.AddListener(delegate
        {
            ConfirmQuest();
        });

        //设置quest button
        GameObject questListUI = GameObject.FindGameObjectWithTag("quest list");
        for (byte i = 0; i < m_availableQuests.Count; i++)
        {
            Transform questButton = (Instantiate(Resources.Load("ui/quest/quest")) as GameObject).transform;
            questButton.SetParent(questListUI.transform);
            questButton.localScale = Vector3.one;
            byte index = i;
            questButton.GetComponent<Button>().onClick.AddListener(delegate
            {
                OnSelectQuest(index);
            });
            _SetQuestButton(questButton, m_availableQuests[index].questName, m_availableQuests[index].stars);
            m_questButtons.Add(questButton);
        }

        //细节
        Transform questDetailUI = m_questPanel.transform.GetChild(0).GetChild(3).GetChild(0);
        m_questTypeImage = GameObject.FindGameObjectWithTag("quest type icon").GetComponent<Image>();
        m_questTypeText = questDetailUI.GetChild(1).GetComponent<Text>();
        m_questDescText = questDetailUI.GetChild(2).GetComponent<Text>();

        OnSelectQuest(0);

        m_questPanel.SetActive(false);
    }

    protected void _SetQuestButton (Transform button, string name, byte star)
    {
        button.GetComponentInChildren<Text>().text = name;
        var stars = button.GetChild(1).GetComponentsInChildren<Image>();
        Sprite sprite = Resources.Load<Sprite>("ui/quest/gold_star");
        for (int i = 0; i < star; i++)
        {
            stars[i].sprite = sprite;
        }
    }

    /// <summary>
    /// 从quests中选择一个，设置UI
    /// </summary>
    /// <param name="index"></param>
    public void OnSelectQuest (byte index)
    {
        //任务细节
        switch (m_availableQuests[index].GetQuestType())
        {
            case ConstantDefine.QuestType.Crusade:
                m_questTypeImage.color = ConstantDefine.crusadeQuestImageColor;
                m_questTypeText.color = ConstantDefine.crusadeQuestTextColor;
                m_questTypeImage.sprite = Resources.Load<Sprite>("ui/quest/crusade");
                m_questTypeText.text = "Crusade";
                break;
            case ConstantDefine.QuestType.Collect:
                m_questTypeImage.color = ConstantDefine.collectionQuestImageColor;
                m_questTypeText.color = ConstantDefine.collectionQuestTextColor;
                m_questTypeImage.sprite = Resources.Load<Sprite>("ui/quest/collection");
                m_questTypeText.text = "Collect";
                break;
            default:
                break;
        }
        m_questDescText.text = m_availableQuests[index].description;
        m_selectedQuestIndex = index;

        switch (m_availableQuests[index].GetQuestState())
        {
            case ConstantDefine.QuestState.None:
                _SetQuestNoneUI();
                break;
            case ConstantDefine.QuestState.InProgress:
                _SetQuestClaimedUI();
                break;
            case ConstantDefine.QuestState.Completed:
                _SetQuestCompletedUI();
                break;
            case ConstantDefine.QuestState.Failed:
                break;
            default:
                break;
        }
    }

    public void ConfirmQuest ( )
    {
        if (m_selectedQuestIndex == 255)
            return;

        switch (m_availableQuests[m_selectedQuestIndex].GetQuestState())
        {
            case ConstantDefine.QuestState.None:
                //领取任务
                m_availableQuests[m_selectedQuestIndex].OnClaim();
                //设置UI
                _SetQuestClaimedUI();
                break;
            case ConstantDefine.QuestState.Completed:
                m_availableQuests[m_selectedQuestIndex].OnGetReward();
                _SetQuestNoneUI();
                break;
            case ConstantDefine.QuestState.Failed:
                break;
            default:
                break;
        }

    }

    private void _SetQuestClaimedUI ( )
    {
        m_confirmButton.enabled = false;
        m_confirmText.text = "Complete";
        if (m_questButtons[m_selectedQuestIndex].childCount == 3)
        {
            GameObject claimed = Instantiate(Resources.Load("ui/quest/claimed")) as GameObject;
            claimed.transform.SetParent(m_questButtons[m_selectedQuestIndex]);
            claimed.transform.localScale = Vector3.one * 2;
            claimed.transform.localPosition = new Vector3(20, 0, 0);
            claimed.transform.DOScale(1, 0.2f).SetEase(Ease.OutElastic);
        }
    }

    private void _SetQuestNoneUI ( )
    {
        m_confirmButton.enabled = true;
        m_confirmText.text = "Comfirm";
        if (m_questButtons.Count > m_selectedQuestIndex && m_questButtons[m_selectedQuestIndex].childCount == 4)
        {
            Destroy(m_questButtons[m_selectedQuestIndex].GetChild(3).gameObject);
        }
    }

    private void _SetQuestCompletedUI ( )
    {
        m_confirmButton.enabled = true;
        m_confirmText.text = "Reward";
    }

}