/*************************************************************

** Auth: ysd
** Date: 15.7.25
** Desc: 保存所有接到的任务，每接一个任务，就开始监听某些事件。
         比如接到某个任务要求杀死3只史莱姆，则监听史莱姆死亡事件，
         史莱姆死亡将会使QuestManager的某个监听器触发，并将该事件
         转发给任务。
         不联网，QuestManager只对本地玩家接到的任务负责
** Vers: 

*************************************************************/

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class QuestManager
{

    public List<Quest> allQuests;

    private Dictionary<ConstantDefine.EnemyType, MessageDefine.EnemyDeathEvent> m_enemyDeathHandlers;
    private Dictionary<ConstantDefine.CollectionType, UnityEvent> m_collectionHandlers;

    /// <summary>
    /// 单例
    /// </summary>
    /// <returns></returns>
    static public QuestManager GetInstance ( )
    {
        return Singleton<QuestManager>.GetInstance();
    }

    private QuestManager ( )
    {
        allQuests = CommonUtility.CreateAllQuestsFromXml();

        m_enemyDeathHandlers = new Dictionary<ConstantDefine.EnemyType, MessageDefine.EnemyDeathEvent>();
        m_collectionHandlers = new Dictionary<ConstantDefine.CollectionType, UnityEvent>();
    }

    /// <summary>
    /// 为讨伐任务进行事件添加处理程序
    /// 例如某个任务杀3个史莱姆，那么每杀一个，就出发一次任务进行事件
    /// </summary>
    /// <param name="types">需要监听其死亡的敌人</param>
    /// <param name="handler">事件处理器</param>
    public void RegisterCrusadeQuestEventHandler (ConstantDefine.EnemyType[] types, UnityAction<byte, ConstantDefine.EnemyType> handler)
    {
        for (int i = 0; i < types.Length; i++)
        {
            if (!m_enemyDeathHandlers.ContainsKey(types[i]))
            {
                var proceedEvent = new MessageDefine.EnemyDeathEvent();
                m_enemyDeathHandlers.Add(types[i], proceedEvent);
            }
            m_enemyDeathHandlers[types[i]].AddListener(handler);
        }
    }

    /// <summary>
    /// 当任务完成时，不再监听
    /// </summary>
    /// <param name="types">需要监听其死亡的敌人</param>
    /// <param name="handler">事件处理器</param>
    public void RemoveCrusadeQuestEventHandler (ConstantDefine.EnemyType[] types, UnityAction<byte, ConstantDefine.EnemyType> handler)
    {
        for (int i = 0; i < types.Length; i++)
        {
            if (m_enemyDeathHandlers.ContainsKey(types[i]))
            {
                m_enemyDeathHandlers[types[i]].RemoveListener(handler);
            }
        }
    }

    /// <summary>
    /// 为采集任务进行事件添加处理程序
    /// </summary>
    /// <param name="type">需要采集的物品类型（这里采集任务只采一种）</param>
    /// <param name="handler"></param>
    public void RegisterCollectionQuestEventHandler (ConstantDefine.CollectionType type, UnityAction handler)
    {
        if (!m_collectionHandlers.ContainsKey(type))
        {
            var collectEvent = new UnityEvent();
            m_collectionHandlers.Add(type, collectEvent);
        }
        m_collectionHandlers[type].AddListener(handler);
    }

    public void RemoveCollectionQuestEventHandler (ConstantDefine.CollectionType type, UnityAction handler)
    {
        if (m_collectionHandlers.ContainsKey(type))
        {
            m_collectionHandlers[type].RemoveListener(handler);
        }
    }

    /// <summary>
    /// 当某个敌人死亡，此函数触发，并将事件转发给监听该敌人死亡的任务
    /// 场景中每出生一个敌人，这个敌人就注册该函数为其死亡事件的监听器
    /// </summary>
    /// <param name="enemyId">场景中敌人ID</param>
    /// <param name="enemyType">敌人类型</param>
    public void EnemyDeathEventHandler (byte enemyId, ConstantDefine.EnemyType enemyType)
    {
        m_enemyDeathHandlers[enemyType].Invoke(enemyId, enemyType);
    }

    /// <summary>
    /// 当某个物品被采集，此函数触发，并将事件转发给任务
    /// 每出生一个可采集物品，就注册该函数
    /// </summary>
    /// <param name="collectionType"></param>
    public void ItemCollectedEventHandler (ConstantDefine.CollectionType collectionType)
    {
        if (m_collectionHandlers.ContainsKey(collectionType))
            m_collectionHandlers[collectionType].Invoke();
    }

}