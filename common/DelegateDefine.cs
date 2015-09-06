/*************************************************************

** Auth: ysd
** Date: 15.7.13
** Desc: 所有委托/事件等的定义
** Vers: v1.0

*************************************************************/

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class DelegateDefine
{

    /// <summary>
    /// 当点击背包内物品UI时，可能有不同操作：使用、交易等
    /// </summary>
    public delegate void OnBagItemChecked ( );
}

public class MessageDefine
{

    #region 网络

    /// <summary>
    /// Client向Host发送该消息，Host踢出Client
    /// </summary>
    public static short ClientLeaveRoomMsgId = MsgType.Highest + 1;
    public class ClientLeaveRoomMessage : MessageBase
    {
        public string playerName;  //离开房间玩家的姓名
        public NodeID nodeId;
        
        //必须提供无参构造函数
        public ClientLeaveRoomMessage ( )
        {
        }

        public ClientLeaveRoomMessage (string pn, NodeID nodeId)
        {
            this.playerName = pn;
            this.nodeId = nodeId;
        }
    }

    /// <summary>
    /// client发送聊天信息到server
    /// </summary>
    public static short SendChatMsgId = MsgType.Highest + 2;

    public class SendChatMessage : MessageBase
    {

        public string words;

        public SendChatMessage ( )
        {
        }

    }

    /// <summary>
    /// server发送聊天信息到任意Client
    /// </summary>
    public static short ChatMsgId = MsgType.Highest + 3;

    public class ChatMessage : MessageBase
    {

        public string playerName;
        public string words;

        public ChatMessage ( )
        {
        }

    }

    /// <summary>
    /// 创建lobbyplayer的必要信息
    /// </summary>
    public static short PlayerInfoMsgId = MsgType.Highest + 4;

    public class PlayerInfoMessage : MessageBase
    {
        public string name;

        public PlayerInfoMessage ( )
        {
        }
    }

    /// <summary>
    /// server以获取client发送的info，clien可请求创建lobbyplayer;
    /// 使用空消息即可
    /// </summary>
    public static short ServerGotPlayerInfoMsgId = MsgType.Highest + 5;

    #endregion

    #region 角色相关

    /// <summary>
    /// 移动相机的方式
    /// </summary>
    public delegate void CameraMoveCallback ( );

    #endregion

    #region 游戏性相关

    /// <summary>
    /// 敌人死亡事件,奖励系统、任务系统、称号/成就等奖监听这个事件.
    /// 在各个客户端上的敌人GameObject触发.
    /// 场景中死亡敌人的ID/敌人类型.
    /// </summary>
    public class EnemyDeathEvent : UnityEvent<byte, ConstantDefine.EnemyType>
    {
    }

    /// <summary>
    /// 物品被采集事件.
    /// 与敌人死亡不同，该事件不在网络中同步
    /// </summary>
    public class ItemCollectedEvent : UnityEvent<ConstantDefine.CollectionType>
    {
    }

    /// <summary>
    /// 监听任务的进行
    /// </summary>
    public delegate void QuestProceedEventHandler ( );

    #endregion

}
