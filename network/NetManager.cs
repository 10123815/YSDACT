/*************************************************************

** Auth: ysd
** Date: 15.7.13     
** Desc: 管理多人游戏的网络状态，
         管理玩家创建、查看、加入房间等
         方法UI*代表操作UI的方法
** Vers: v1.0

*************************************************************/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.Networking.Types;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class NetManager : NetworkLobbyManager
{

    private ConstantDefine.NetworkState m_netStt = ConstantDefine.NetworkState.None;

    #region offline

    /// <summary>
    /// 这个界面用于玩家创建、选择、加入一个房间;玩家进入房间后，应该被销毁
    /// </summary>
    private GameObject m_matchUI;

    /// <summary>
    /// 创建房间需要填写的表单
    /// </summary>
    private GameObject m_matchCreatingForm;
    private InputField m_matchNameIF;
    private InputField m_playerNameIF;
    private string m_playerName = "sb";
    private byte m_roomSize = 3;
    //private Toggle m_publicToggle;

    /// <summary>
    /// 房间列表
    /// </summary>
    private GameObject m_matchListPanel;
    private int m_resultPageSize = 5;
    private int m_currentPageNum = 0;
    private Text m_currentPageNumText;
    private Button[] roomButtons;
    private string m_nameFilter = "";
    private InputField m_nameFilterIF;

    /// <summary>
    /// 在这个面板输入密码，确认加入
    /// </summary>
    //private byte m_matchAttemptIndex;
    //private GameObject m_matchConfirmPanel;
    //private InputField m_matchPswdEnterIF;

    /// <summary>
    /// 等待面板，覆盖其他的UI
    /// </summary>
    private GameObject m_waitingPanel;
    private Text m_waitingTipsText;

    /// <summary>
    /// 创建、加入房间失败
    /// </summary>
    private GameObject m_errorPanel;
    private Text m_errorTipsText;

    #endregion

    /// <summary>
    /// 进入房间后，显示房间信息，成员列表，和一个退出的按钮
    /// </summary>
    private GameObject m_roomUI;

    /// <summary>
    /// 显示房间内的其他玩家信息
    /// </summary>
    //private GameObject m_peerPanel;

    /// <summary>
    /// 游戏UI
    /// </summary>
    private GameObject m_gameUI;

    #region 客户端

    void Start ( )
    {
        _OfflineSceneInit();
    }

    private void _OfflineSceneInit ( )
    {

        if (m_roomUI != null)
            Destroy(m_roomUI);

        if (m_matchUI != null)
            return;

        m_matchUI = Instantiate(Resources.Load("ui/match ui")) as GameObject;
        DontDestroyOnLoad(m_matchUI);

        var m_matchPanel = m_matchUI.transform.GetChild(0);

        //创建/加入房间按钮
        Button[] buttons = m_matchPanel.GetChild(0).GetComponentsInChildren<Button>();
        if (buttons.Length != 2)
            Debug.LogError(buttons.Length);
        buttons[0].onClick.AddListener(delegate
        {
            UICreateMatch();
        });
        buttons[1].onClick.AddListener(delegate
        {
            UIListMatch();
        });


        //创建房间表单
        m_matchCreatingForm = m_matchPanel.GetChild(1).gameObject;

        //表单项
        var inputFields = m_matchCreatingForm.GetComponentsInChildren<InputField>();
        m_matchNameIF = inputFields[0];
        m_playerNameIF = inputFields[1];

        //确认/取消创建
        buttons = m_matchCreatingForm.GetComponentsInChildren<Button>();
        if (buttons.Length != 2)
            Debug.LogError("");
        buttons[0].onClick.AddListener(delegate
        {
            UIConfirmCreateMatch();
        });
        buttons[1].onClick.AddListener(delegate
        {
            UICancelCreateMatch();
        });

        m_matchCreatingForm.SetActive(false);

        //房间列表
        m_matchListPanel = m_matchPanel.GetChild(2).gameObject;
        buttons = m_matchListPanel.GetComponentsInChildren<Button>();
        //room列表 + close + 刷新 + 左右
        if (buttons.Length != m_resultPageSize + 4)
            Debug.LogError(buttons.Length);
        //
        m_nameFilterIF = m_matchListPanel.GetComponentInChildren<InputField>();
        //显示当前页数
        m_currentPageNumText = m_matchListPanel.GetComponentInChildren<Text>();
        m_currentPageNumText.text = "0";

        //点击第几个个按钮，就加入第几个房间
        roomButtons = new Button[m_resultPageSize];
        for (byte i = 0; i < m_resultPageSize; i++)
        {
            roomButtons[i] = buttons[i];
            byte index = i;
            roomButtons[i].onClick.AddListener(delegate
            {
                UIJoinMatch(index);
            });
        }

        //close按钮
        buttons[m_resultPageSize].onClick.AddListener(delegate
        {
            UICloseMatchesList();
        });

        //search
        buttons[m_resultPageSize + 1].onClick.AddListener(delegate
        {
            UIListMatch();
        });

        //previous/next
        buttons[m_resultPageSize + 2].onClick.AddListener(delegate
        {
            UIListMatchPrevious();
        });
        buttons[m_resultPageSize + 3].onClick.AddListener(delegate
        {
            UIListMatchNext();
        });

        m_matchListPanel.SetActive(false);

        m_waitingPanel = m_matchUI.transform.GetChild(1).gameObject;
        m_waitingTipsText = m_waitingPanel.GetComponentInChildren<Text>();
        m_waitingPanel.GetComponentInChildren<Button>().onClick.AddListener(delegate
        {
            UICloseWaitingPanel();
        });
        m_waitingPanel.SetActive(false);

        m_errorPanel = m_matchUI.transform.GetChild(2).gameObject;
        m_errorTipsText = m_errorPanel.GetComponentInChildren<Text>();
        m_errorPanel.GetComponentInChildren<Button>().onClick.AddListener(delegate
        {
            UICloseErrorPanel();
        });
        m_errorPanel.SetActive(false);

    }

    private void _RoomSceneInit ( )
    {
        if (m_matchUI != null)
            Destroy(m_matchUI);

        if (m_roomUI != null)
            return;

        m_roomUI = Instantiate(Resources.Load("ui/room ui")) as GameObject;

        m_waitingPanel = m_roomUI.transform.GetChild(3).gameObject;
        m_waitingTipsText = m_waitingPanel.GetComponentInChildren<Text>();
        RectTransform menuTF = m_roomUI.transform.GetChild(4) as RectTransform;
        float width = ((RectTransform)menuTF).sizeDelta.x / 5;
        Button[] buttons = menuTF.GetComponentsInChildren<Button>();
        //箭头
        buttons[0].onClick.AddListener(delegate
        {
            RectTransform buttonTf = buttons[0].rectTransform();
            Sequence seq = DOTween.Sequence();
            if (buttonTf.localRotation.eulerAngles.z > 90)
            {
                seq.Append(menuTF.DOAnchorPos(menuTF.anchoredPosition - new Vector2(width * 4, 0), 0.5f))
                   .Join(buttonTf.DORotate(new Vector3(0, 0, 0), 0.2f));
            }
            else
            {
                seq.Append(menuTF.DOAnchorPos(menuTF.anchoredPosition + new Vector2(width * 4, 0), 0.5f))
                   .Join(buttonTf.DORotate(new Vector3(0, 0, 180), 0.2f));
            }
        });
        //角色、背包、设定
        for (int i = 1; i < 4; i++)
        {
            string name = buttons[i].name;
            GameObject canvas;
            CommonCanvasManager.GetInstance().OpenCommonCanvas(name, out canvas, true);
            RectTransform panelTF = canvas.transform.GetChild(0).rectTransform();
            switch (i)
            {
                //背包面板
                case 2:
                    BagManager bm = BagManager.GetInstance();
                    bm.equipmentBag.Init(GameObject.FindGameObjectWithTag("equipment bag").transform.rectTransform(), canvas);
                    bm.potionBag.Init(GameObject.FindGameObjectWithTag("potion bag").transform.rectTransform(), canvas);
                    bm.sundryBag.Init(GameObject.FindGameObjectWithTag("sundry bag").transform.rectTransform(), canvas);
                    break;
                default:
                    break;
            }
            buttons[i].onClick.AddListener(delegate
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
            canvas.SetActive(false);
        }
        m_waitingPanel.SetActive(false);
    }

    private void _GameSceneInit ( )
    {
        if (m_matchUI != null)
            Destroy(m_matchUI);

        if (m_gameUI != null)
            return;

        m_gameUI = Instantiate(Resources.Load("ui/game ui")) as GameObject;
    }

    /// <summary>
    /// 等待连接时的提示
    /// </summary>
    /// <param name="tips">提示内容</param>
    /// <param name="timeOut">超时</param>
    /// <param name="num">'.'的个数</param>
    /// <example>"CREATING..."</example>
    /// <returns></returns>
    private IEnumerator _Waiting (string tips, int timeOut = 1000, byte num = 3)
    {
        m_matchUI.GetComponentInChildren<CanvasGroup>().interactable = false;
        m_waitingPanel.SetActive(true);
        int init = 2;
        int enterTime = (int)Time.time;
        while (m_waitingPanel != null && m_waitingPanel.activeSelf)
        {
            int waitingTime = (int)Time.time - enterTime;

            //超时
            if (waitingTime >= timeOut)
            {
                _Error("TIMED OUT");
                m_waitingPanel.SetActive(false);
                break;
            }

            int t = waitingTime % num;
            if (init != t)
            {
                string points = "";
                for (byte i = 0; i < num; i++)
                {
                    points += ".";
                }
                m_waitingTipsText.text = tips + points;
            }
            yield return 0;
        }
        //连接正常就会进入房间
        //m_matchPanel.GetComponent<CanvasGroup>().interactable = true;
    }

    public void UICloseWaitingPanel ( )
    {
        m_matchUI.GetComponentInChildren<CanvasGroup>().interactable = true;
        m_waitingTipsText.text = "";
        m_waitingPanel.SetActive(false);
    }

    /// <summary>
    /// 发生错误时的提示
    /// </summary>
    private void _Error (string errorStr)
    {
        m_errorPanel.SetActive(true);
        m_errorTipsText.text = errorStr;
    }

    public void UICloseErrorPanel ( )
    {
        m_matchUI.GetComponentInChildren<CanvasGroup>().interactable = true;
        m_errorTipsText.text = "";
        m_errorPanel.SetActive(false);
    }

    #region 进入房间

    #region 创建
    /// <summary>
    /// 弹出创建房间表单
    /// </summary>
    public void UICreateMatch ( )
    {

        m_matchListPanel.SetActive(false);
        m_matchCreatingForm.SetActive(true);

        //play a music

    }

    public void UICancelCreateMatch ( )
    {
        m_matchNameIF.text = "default";
        m_playerNameIF.text = "sb";
        m_matchCreatingForm.SetActive(false);
    }

    /// <summary>
    /// 点击创建房间按钮后，根据之前填写的表单创建房间
    /// </summary>
    public void UIConfirmCreateMatch ( )
    {
        if (matchMaker == null)
            StartMatchMaker();

        CreateMatchRequest creating = new CreateMatchRequest();
        creating.name = m_matchNameIF.text.Equals(string.Empty) ? "default" : m_matchNameIF.text;
        m_playerName = m_playerNameIF.text;
        creating.password = /*m_matchPswdIF.text*/"";
        creating.size = m_roomSize;
        //creating.advertise = m_publicToggle;
        creating.advertise = true;
        matchMaker.CreateMatch(creating, _OnCreateMatch);
        StartCoroutine(_Waiting("CREATING"));
    }

    /// <summary>
    /// 创建房间后的回调函数
    /// </summary>
    /// <param name="response">
    /// 包含已创建的房间的信息
    /// </param>
    private void _OnCreateMatch (CreateMatchResponse response)
    {
        if (response.success)
        {
            Utility.SetAccessTokenForNetwork(response.networkId, new NetworkAccessToken(response.accessTokenString));
            client = StartHost(new MatchInfo(response));
            m_netStt = ConstantDefine.NetworkState.Host;
        }
        else
        {
            m_waitingPanel.SetActive(false);
            _Error("FAILED TO CREATE A ROOM");
        }
    }

    #endregion

    #region 获取房间列表

    public void UIListMatch ( )
    {
        if (m_nameFilterIF != null && m_nameFilterIF.IsActive())
            m_nameFilter = m_nameFilterIF.text;
        _ListMatch();
    }

    public void UIListMatchPrevious ( )
    {
        m_currentPageNum--;
        m_currentPageNum = (m_currentPageNum < 0) ? 0 : m_currentPageNum;
        m_currentPageNumText.text = m_currentPageNum.ToString();
        _ListMatch();
    }

    public void UIListMatchNext ( )
    {
        m_currentPageNum++;
        m_currentPageNumText.text = m_currentPageNum.ToString();
        _ListMatch();
    }

    public void UICloseMatchesList ( )
    {
        m_currentPageNum = 0;
        m_currentPageNumText.text = "0";
        m_nameFilter = "";
        m_matchListPanel.SetActive(false);
    }

    private void _ListMatch ( )
    {
        if (matchMaker == null)
            StartMatchMaker();

        m_matchCreatingForm.SetActive(false);
        matchMaker.ListMatches(m_currentPageNum, m_resultPageSize, m_nameFilter, _OnListMatch);
        m_matchListPanel.SetActive(true);
        StartCoroutine(_Waiting("SEARCHING"));
    }

    private void _OnListMatch (ListMatchResponse response)
    {
        m_waitingPanel.SetActive(false);
        if (response.success)
        {
            m_matchUI.GetComponentInChildren<CanvasGroup>().interactable = true;
            matches = response.matches;

            for (byte i = 0; i < matches.Count; i++)
            {
                Text[] texts = roomButtons[i].GetComponentsInChildren<Text>();
                //房间名称
                texts[0].text = matches[i].name;
                //当前人数
                texts[1].text = matches[i].currentSize + "/" + m_roomSize;

                //是否超出人数
                roomButtons[i].interactable = matches[i].currentSize < m_roomSize;
            }

            for (byte i = (byte)matches.Count; i < m_resultPageSize; i++)
            {
                Text[] texts = roomButtons[i].GetComponentsInChildren<Text>();
                texts[0].text = "";
                texts[1].text = "*/*";
                roomButtons[i].interactable = false;
            }

        }
        else
        {
            m_waitingPanel.SetActive(false);
            _Error("FAILED TO LIST ROOMS");
        }
    }

    #endregion

    #region 加入房间

    public void UIJoinMatch (byte index)
    {
        if (matchMaker == null)
            StartMatchMaker();

        if (index >= matches.Count)
            return;

        //m_matchAttemptIndex = index;
        //m_matchConfirmPanel.SetActive(true);
        matchMaker.JoinMatch(matches[(int)index].networkId, "", _OnJoinMatch);
        StartCoroutine(_Waiting("ENTERING"));

    }

    //public void UIConfirmJoin()
    //{
    //    matchMaker.JoinMatch(matches[(int)m_matchAttemptIndex].networkId, m_matchPswdEnterIF.text, _OnJoinMatch);
    //    m_matchConfirmPanel.SetActive(false);
    //    StartCoroutine(_Waiting("ENTERING THE ROOM"));
    //}

    //public void UICancelJoin()
    //{
    //    m_matchPswdEnterIF.text = "";
    //    m_matchConfirmPanel.SetActive(false);
    //}

    private void _OnJoinMatch (JoinMatchResponse response)
    {
        if (response.success)
        {
            if (Utility.GetAccessTokenForNetwork(response.networkId) == null)
                Utility.SetAccessTokenForNetwork(response.networkId, new NetworkAccessToken(response.accessTokenString));
            client = StartClient(new MatchInfo(response));
            m_netStt = ConstantDefine.NetworkState.Client;
        }
        else
        {
            m_waitingPanel.SetActive(false);
            _Error("FAILED TO ENTER A ROOM");
        }
    }

    #endregion

    #endregion 进入房间

    #region 玩家进入房间后

    private void _UIUpdateChatPanel (string text)
    {
    }

    #region 消息处理

    private void _ResgisterClientHandler ( )
    {
        if (client == null)
            Debug.LogError("");

    }

    private void _OnClientGetChatMessage (NetworkMessage msg)
    {
        MessageDefine.ChatMessage chatMsg = msg.ReadMessage<MessageDefine.ChatMessage>();
        string text = chatMsg.playerName + ":" + chatMsg.words;
        _UIUpdateChatPanel(text);
    }

    #endregion 消息处理

    public override void OnLobbyStartClient (NetworkClient client)
    {
        _ResgisterClientHandler();

    }

    public override void OnLobbyClientEnter ( )
    {
        _RoomSceneInit();
        Button.ButtonClickedEvent onclick = GameObject.FindGameObjectWithTag("exit room").GetComponent<Button>().onClick;
        switch (m_netStt)
        {
            case ConstantDefine.NetworkState.Host:
                onclick.AddListener(delegate
                {
                    DestroyMatchRequest request = new DestroyMatchRequest();
                    request.accessTokenString = matchInfo.accessToken.GetByteString();
                    request.appId = Utility.GetAppID();
                    request.networkId = matchInfo.networkId;
                    matchMaker.DestroyMatch(matchInfo.networkId, _OnDestroyMatch);
                });
                break;
            case ConstantDefine.NetworkState.Client:
                onclick.AddListener(delegate
                {
                    client.Send(MessageDefine.ClientLeaveRoomMsgId, new MessageDefine.ClientLeaveRoomMessage(m_playerName, matchInfo.nodeId));
                    //StartCoroutine(_Waiting("EXITING"));
                });
                break;
        }

    }

    public override void OnLobbyClientDisconnect (NetworkConnection conn)
    {
        //Debug.Log("OnLobbyClientDisconnect");
        StopClient();
    }

    public override void OnLobbyStopClient ( )
    {
        _OfflineSceneInit();
    }



    #endregion 玩家进入房间后

    #endregion 客户端

    #region 服务器端

    private string _GetPlayerNameFormConnectionId (int id)
    {
        return "";
    }

    public override void OnLobbyStartHost ( )
    {
        _ResgisterServerHandler();
    }

    public override GameObject OnLobbyServerCreateLobbyPlayer (NetworkConnection conn, short playerControllerId)
    {
        Transform startPosition = GetStartPosition();
        var player = Instantiate(lobbyPlayerPrefab.gameObject, startPosition.position, startPosition.rotation) as GameObject;
        //player.name = m_playerName;
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        return player;
    }

    public override void OnLobbyServerDisconnect (NetworkConnection conn)
    {
        NetworkServer.SetClientNotReady(conn);
    }

    #region 自定义Network消息处理

    private void _ResgisterServerHandler ( )
    {
        NetworkServer.RegisterHandler(MessageDefine.ClientLeaveRoomMsgId, _OnPlayerLeave);
        NetworkServer.RegisterHandler(MessageDefine.SendChatMsgId, _OnServerGetChatMessage);
    }

    private void _OnPlayerLeave (NetworkMessage msg)
    {
        MessageDefine.ClientLeaveRoomMessage leaveMsg = msg.ReadMessage<MessageDefine.ClientLeaveRoomMessage>();
        matchMaker.DropConnection(matchInfo.networkId, leaveMsg.nodeId, _OnDropMatch);
    }

    /// <summary>
    /// client将聊天信息发给server，server再将信息发往其他client
    /// </summary>
    /// <param name="msg"></param>
    private void _OnServerGetChatMessage (NetworkMessage msg)
    {
        List<NetworkConnection> conns = NetworkServer.connections;

        MessageDefine.ChatMessage chatMsg = new MessageDefine.ChatMessage();
        chatMsg.playerName = _GetPlayerNameFormConnectionId(msg.conn.connectionId);
        chatMsg.words = msg.ReadMessage<MessageDefine.SendChatMessage>().words;

        for (int i = 0; i < conns.Count; i++)
        {
            if (conns[i].connectionId != msg.conn.connectionId)
            {
                NetworkServer.SendToClient(conns[i].connectionId, MessageDefine.ChatMsgId, chatMsg);
            }
        }
    }

    #endregion 自定义Network消息处理

    private void _OnDropMatch (BasicResponse response)
    {
        if (response.success)
        {

        }
    }

    private void _OnDestroyMatch (BasicResponse response)
    {
        if (response.success)
        {
            StopHost();
        }
    }

    #endregion 服务器端

}
