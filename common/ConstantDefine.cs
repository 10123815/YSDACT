/*************************************************************

** Auth: ysd
** Date: 15.7.16
** Desc: 定义常量
** Vers: v1.0

*************************************************************/

using UnityEngine;
using System.Collections;

public class ConstantDefine
{

    #region 资源路径

    static public string questXmlFileUrl = "/Resources/quest/quests.xml";

    #endregion

    #region 网络

    /// <summary>
    /// 房间内的出生点
    /// </summary>
    static public Vector3 lobbyPLayerStartPosition = new Vector3(20, 10, -16);

    public enum NetworkState : byte
    {
        None = 0,
        Host = 1,
        Client = 2
    }
    #endregion

    #region 角色

    /// <summary>
    /// 角色头顶位置
    /// </summary>
    static public Vector3 playerHeadPosition = new Vector3(0, 2, 0);

    /// <summary>
    /// 触发动画的bool数组
    /// </summary>
    static public byte AnimationTriggerCount = 5;
    public enum AnimationTrigger : byte
    {
        Move = 0,
        Jump = 1,
        Dash = 2,
        Latk = 3,
        Hatk = 4
    }

    public enum NPCType : byte
    {
        Quester = 0
    }

    public enum EnemyType : byte
    {
    }

    public enum BagItemType : byte
    {
        Equipment = 0,
        Potion = 1,
        Sundry = 2,
        Money = 3
    }

    /// <summary>
    /// 可采集物，可点击采集/其他方式获取（任务奖励等）
    /// </summary>
    public enum CollectionType : byte
    {
        Money = 0,
        MinorHealthPotion = 1,
        Key = 2
    }

    /// <summary>
    /// 不可采集物，储物箱/石碑等可调查物品
    /// </summary>
    public enum UnCollectionType : byte
    {
        Desk = 1,
    }

    #endregion

    #region 游戏性

    static public Color crusadeQuestImageColor = new Color(160.0f / 255.0f, 80.0f / 255.0f, 80.0f / 255.0f);
    static public Color collectionQuestImageColor = new Color(80.0f / 255.0f, 160.0f / 255.0f, 80.0f / 255.0f);
    static public Color crusadeQuestTextColor = new Color(120.0f / 255.0f, 30.0f / 255.0f, 30.0f / 255.0f);
    static public Color collectionQuestTextColor = new Color(30.0f / 255.0f, 120.0f / 255.0f, 30.0f / 255.0f);

    public enum QuestType : byte
    {
        Crusade = 0,
        Collect = 1
    }

    public enum QuestState : byte
    {
        None = 0,
        InProgress = 1,
        Completed = 2,
        Failed = 3
    }

    #endregion

}
