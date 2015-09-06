/*************************************************************

** Auth: ysd
** Date: 15.9.2     
** Desc: 管理多人游戏的网络状态，
         管理玩家创建、查看、加入房间等
         改自NetManager
** Vers: v1.0

*************************************************************/

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.Networking.Types;
using System.Collections;
using System.Collections.Generic;
using KiiCorp.Cloud.Storage;

public class FUCKACTNetworkManager : NetworkLobbyManager
{

    #region client properties

    /// <summary>
    /// 网络状态
    /// </summary>
    private ConstantDefine.NetworkState m_networkState = ConstantDefine.NetworkState.None;

    /// <summary>
    /// 需要同步的player info
    /// </summary>
    private struct NetworkedPlayerInfo
    {
        public string name;
    }

    #endregion

    #region server properties

    private Dictionary<int, NetworkedPlayerInfo> m_playerInfos;

    #endregion


    #region 创建/加入房间

    /// <summary>
    /// 点击确认创建，创建比赛
    /// </summary>
    /// <param name="matchName">房间名</param>
    public void CreateMatch (string matchName)
    {
        if (matchMaker == null)
            StartMatchMaker();

        matchMaker.CreateMatch(matchName.Equals(string.Empty) ? "default" : matchName, 3, true, "", _OnCreateMatch);
        MatchUI.instance.Wait();
    }


    /// <summary>
    /// 创建房间后的回调函数
    /// </summary>
    /// <param name="response">
    /// 包含已创建的房间的信息
    /// </param>
    private void _OnCreateMatch (CreateMatchResponse response)
    {
        MatchUI.instance.StopWait();
        if (response.success)
        {
            //令牌
            Utility.SetAccessTokenForNetwork(response.networkId, new NetworkAccessToken(response.accessTokenString));
            //创建后立即进入房间
            client = StartHost(new MatchInfo(response));
            m_networkState = ConstantDefine.NetworkState.Host;
        }
        else
        {
            //处理错误
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void ListMatch (byte pageNumber, string nameFilter)
    {
        if (matchMaker == null)
            StartMatchMaker();

        matchMaker.ListMatches(pageNumber, 5, nameFilter, _OnListMatch);
        MatchUI.instance.Wait();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="response"></param>
    private void _OnListMatch (ListMatchResponse response)
    {
        MatchUI.instance.StopWait();
        if (response.success)
        {
            matches = response.matches;
            //填充房间列表UI
            MatchUI.instance.OnGotMatches(response.matches);
        }
        else
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="networkId"></param>
    public void JoinMatch (byte index)
    {
        if (matchMaker == null)
            StartMatchMaker();

        if (index < matches.Count)
        {
            NetworkID networkId = matches[index].networkId;
            matchMaker.JoinMatch(networkId, "", _OnJoinMatch);
            MatchUI.instance.Wait();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="response"></param>
    private void _OnJoinMatch (JoinMatchResponse response)
    {
        MatchUI.instance.StopWait();
        if (response.success)
        {
            if (Utility.GetAccessTokenForNetwork(response.networkId) == null)
                Utility.SetAccessTokenForNetwork(response.networkId, new NetworkAccessToken(response.accessTokenString));
            client = StartClient(new MatchInfo(response));
            m_networkState = ConstantDefine.NetworkState.Client;
        }
        else
        {

        }
    }

    #endregion


    #region 进入房间后

    #region client

    public override void OnLobbyStartClient (NetworkClient client)
    {
        _ResgisterClientEventHandler();
    }

    public override void OnLobbyClientEnter ( )
    {
        GameObject[] matchUI = GameObject.FindGameObjectsWithTag("match ui");
        for (int i = 0; i < matchUI.Length; i++)
        {
            Destroy(matchUI[i]);
        }
        GameObject roomUI = Instantiate(Resources.Load("ui/room ui")) as GameObject;
        MessageDefine.PlayerInfoMessage playerInfoMsg = new MessageDefine.PlayerInfoMessage();
        playerInfoMsg.name = KiiUser.CurrentUser.Displayname;
        client.Send(MessageDefine.PlayerInfoMsgId, playerInfoMsg);
    }

    #region 自定义Network消息处理

    private void _ResgisterClientEventHandler ( )
    {
        if (client == null)
            Debug.LogError("");

        client.RegisterHandler(MessageDefine.ServerGotPlayerInfoMsgId, _OnClientGotAckForAddPlayer);
    }

    /// <summary>
    /// client确认server受到他发送的player info
    /// </summary>
    /// <param name="msg"></param>
    private void _OnClientGotAckForAddPlayer (NetworkMessage msg)
    {
        ClientScene.AddPlayer(client.connection, 0);
    }

    #endregion 自定义Network消息处理

    #endregion client

    #region server

    private string _GetPlayerNameFormConnectionId (int id)
    {
        return "";
    }

    public override void OnLobbyStartHost ( )
    {
        m_playerInfos = new Dictionary<int, NetworkedPlayerInfo>();
        _ResgisterServerHandler();
    }

    public override GameObject OnLobbyServerCreateLobbyPlayer (NetworkConnection conn, short playerControllerId)
    {
        Transform startPosition = GetStartPosition();
        var player = Instantiate(lobbyPlayerPrefab.gameObject, startPosition.position, startPosition.rotation) as GameObject;
        if (m_playerInfos.ContainsKey(conn.connectionId))
        {
            player.GetComponent<PlayerLobby>().playerName = m_playerInfos[conn.connectionId].name;
        }
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
        NetworkServer.RegisterHandler(MessageDefine.SendChatMsgId, _OnServerGotChatMessage);
        NetworkServer.RegisterHandler(MessageDefine.PlayerInfoMsgId, _OnServerGotPlayerInfo);
    }

    /// <summary>
    /// 某个玩家请求离开房间
    /// </summary>
    /// <param name="msg"></param>
    private void _OnPlayerLeave (NetworkMessage msg)
    {
        MessageDefine.ClientLeaveRoomMessage leaveMsg = msg.ReadMessage<MessageDefine.ClientLeaveRoomMessage>();
        matchMaker.DropConnection(matchInfo.networkId, leaveMsg.nodeId, _OnDropMatch);
    }

    /// <summary>
    /// client将聊天信息发给server，server再将信息发往其他client
    /// </summary>
    /// <param name="msg"></param>
    private void _OnServerGotChatMessage (NetworkMessage msg)
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

    private void _OnServerGotPlayerInfo (NetworkMessage msg)
    {
        MessageDefine.PlayerInfoMessage infoMsg = msg.ReadMessage<MessageDefine.PlayerInfoMessage>();
        NetworkedPlayerInfo info;
        info.name = infoMsg.name;
        if (m_playerInfos == null)
            m_playerInfos = new Dictionary<int, NetworkedPlayerInfo>();
        m_playerInfos.Add(msg.conn.connectionId, info);
        var ack = new EmptyMessage();
        NetworkServer.SendToClient(msg.conn.connectionId, MessageDefine.ServerGotPlayerInfoMsgId, ack);
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

    #endregion

    #endregion


}