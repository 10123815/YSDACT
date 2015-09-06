/*************************************************************

** Auth: ysd
** Date: 15.9.2
** Desc: 创建/加入比赛UI
** Vers: v1.0

*************************************************************/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.Networking.Types;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using KiiCorp.Cloud.Storage;

public class MatchUI : MonoBehaviour
{

    static public MatchUI instance;

    /// <summary>
    /// 自定义NetworkManager
    /// </summary>
    private FUCKACTNetworkManager m_networkManager;

    /// <summary>
    /// 显示等待UI时，不活动的UI
    /// create, list, match UI自身
    /// </summary>
    private CanvasGroup[] m_onWaitCanvasGroup = new CanvasGroup[3];

    #region UI/Canvas
    private GameObject m_createMatchCanvas;
    private GameObject m_matchListCanvas;
    #endregion

    #region UI/Widget
    private InputField m_roomNameInput;
    private Text m_currentPageNumText;
    private struct RoomButtonData
    {
        public Button button;
        public Text name;
        public Text size;
    }
    private RoomButtonData[] m_roomButtons;
    private InputField m_nameFilterInput;
    private Button m_searchButton;
    private Button m_previousPageButton;
    private Button m_nextPageButton;
    #endregion

    #region 创建/加入房间

    #endregion

    #region 房间列表
    private byte m_currentPageNum;
    private string m_nameFilter;
    #endregion

    void Awake ( )
    {
        instance = this;
    }

    void Start ( )
    {

        m_networkManager = (FUCKACTNetworkManager)FUCKACTNetworkManager.singleton;

        m_onWaitCanvasGroup[0] = GetComponent<CanvasGroup>();

        _InitCreateCanvas();
        _InitMatchListCanvas();

        Button[] buttons = GetComponentsInChildren<Button>();

        //create
        buttons[0].onClick.AddListener(( ) =>
        {
            _OpenPanel(m_createMatchCanvas);
        });

        //join
        buttons[1].onClick.AddListener(( ) =>
        {
            _OpenPanel(m_matchListCanvas);
            _Search();
        });

        if (KiiUser.CurrentUser == null)
        {
            var cg = GetComponent<CanvasGroup>();
            cg.alpha = 0.5f;
            cg.interactable = false;
            Instantiate(Resources.Load("ui/login"));
        }

    }

    /// <summary>
    /// 创建/加入房间UI初始化
    /// </summary>
    private void _InitCreateCanvas ( )
    {
        m_createMatchCanvas = GameObject.Instantiate(Resources.Load("ui/match/match create")) as GameObject;
        Transform createPanelTF = m_createMatchCanvas.transform.GetChild(0);

        m_roomNameInput = createPanelTF.Find("room name").GetComponent<InputField>();

        Button button = createPanelTF.Find("internet").GetComponent<Button>();
        //internet game
        button.onClick.AddListener(( ) =>
        {
            m_networkManager.CreateMatch(m_roomNameInput.text);
        });

        //lan game

        //close button
        button = createPanelTF.Find("cancel").GetComponent<Button>();
        button.onClick.AddListener(( ) =>
        {
            _ClosePanel(m_createMatchCanvas);
        });

        m_onWaitCanvasGroup[1] = m_createMatchCanvas.GetComponent<CanvasGroup>();
        m_createMatchCanvas.SetActive(false);

    }

    /// <summary>
    /// 房间列表UI初始化
    /// </summary>
    private void _InitMatchListCanvas ( )
    {
        m_matchListCanvas = GameObject.Instantiate(Resources.Load("ui/match/match list")) as GameObject;
        Transform listPanelTF = m_matchListCanvas.transform.GetChild(0);

        m_currentPageNumText = listPanelTF.GetChild(0).GetChild(0).GetComponent<Text>();

        m_roomButtons = new RoomButtonData[5];
        for (byte i = 0; i < 5; i++)
        {
            var buttonTF = listPanelTF.GetChild(i + 1);
            m_roomButtons[i].button = buttonTF.GetComponent<Button>();
            byte index = i;
            m_roomButtons[i].button.onClick.AddListener(( ) =>
            {
                _JoinMatch(index);
            });
            var texts = buttonTF.GetComponentsInChildren<Text>();
            m_roomButtons[i].name = texts[0];
            m_roomButtons[i].size = texts[1];
        }

        m_nameFilterInput = listPanelTF.GetChild(6).GetComponent<InputField>();

        Button closeButton = listPanelTF.GetChild(7).GetComponent<Button>();
        CanvasGroup canvasGroup = m_matchListCanvas.GetComponent<CanvasGroup>();
        m_onWaitCanvasGroup[2] = canvasGroup;
        closeButton.onClick.AddListener(( ) =>
        {
            _ClosePanel(m_matchListCanvas);
        });

        // search button
        closeButton = listPanelTF.GetChild(8).GetComponent<Button>();
        closeButton.onClick.AddListener(( ) =>
        {
            _Search();
        });

        // previous page
        closeButton = listPanelTF.GetChild(9).GetComponent<Button>();
        closeButton.onClick.AddListener(( ) =>
        {
            _PreviousPage();
        });

        // next page
        closeButton = listPanelTF.GetChild(10).GetComponent<Button>();
        closeButton.onClick.AddListener(( ) =>
        {
            _NextPage();
        });

        m_matchListCanvas.SetActive(false);

    }

    /// <summary>
    /// 当获取房间列表时，更新UI
    /// </summary>
    /// <param name="matches"></param>
    public void OnGotMatches (List<MatchDesc> matches)
    {
        for (byte i = 0; i < matches.Count; i++)
        {
            MatchDesc match = matches[i];
            m_roomButtons[i].name.text = match.name;
            m_roomButtons[i].size.text = match.currentSize + "/3";
            //是否超出人数
            m_roomButtons[i].button.interactable = match.currentSize < 3;
        }
    }

    /// <summary>
    /// 插值动画打开某一UI
    /// </summary>
    /// <param name="panel"></param>
    private void _OpenPanel (GameObject panel)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        Transform matchListTF = panel.transform.GetChild(0);
        Sequence seq = DOTween.Sequence();
        panel.SetActive(true);
        seq.Append(matchListTF.DOScale(1, 0.2f)).Join(canvasGroup.DOFade(1, 0.2f)).AppendCallback(( ) =>
        {
            canvasGroup.interactable = true;
        });
    }

    private void _ClosePanel (GameObject panel)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        Transform matchListTF = panel.transform.GetChild(0);
        canvasGroup.interactable = false;
        Sequence seq = DOTween.Sequence();
        seq.Append(matchListTF.DOScale(0.5f, 0.2f)).Join(canvasGroup.DOFade(0, 0.2f)).AppendCallback(( ) =>
        {
            panel.SetActive(false);
        });
    }

    public void Wait ( )
    {
        //message UI
        GameObject canvas = CommonCanvasManager.GetInstance().ShowMessage("Wait", m_onWaitCanvasGroup);
    }

    public void StopWait ( )
    {
        CommonCanvasManager.GetInstance().CloseMessagePanel(m_onWaitCanvasGroup);
    }

    public void OnDestroy ( )
    {
        Destroy(m_createMatchCanvas);
        Destroy(m_matchListCanvas);
        CommonCanvasManager.GetInstance().Return("message");
    }

    #region 创建/加入



    #endregion

    #region 房间列表

    private void _JoinMatch (byte index)
    {
        m_networkManager.JoinMatch(index);
    }

    private void _PreviousPage ( )
    {
        if (m_currentPageNum > 0)
            m_currentPageNum--;
        m_currentPageNumText.text = m_currentPageNum.ToString();
        m_networkManager.ListMatch(m_currentPageNum, m_nameFilter);
    }

    private void _NextPage ( )
    {
        m_currentPageNum++;
        m_currentPageNumText.text = m_currentPageNum.ToString();
        m_networkManager.ListMatch(m_currentPageNum, m_nameFilter);
    }

    private void _Search ( )
    {
        m_currentPageNumText.text = "0";
        m_nameFilter = m_nameFilterInput.text;
        m_networkManager.ListMatch(0, m_nameFilter);
    }

    #endregion
}