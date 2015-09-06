/*************************************************************

** Auth: ysd
** Date: 15.7.25
** Desc: 所有任务的基类
** Vers: v1.0

*************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestTarget<T>
{
    public T targetType;
    public byte targetNumber;
    public byte completedNumber;
}

abstract public class Quest
{

    public string questName;
    public string sceneName;
    public string description;

    /// <summary>
    /// 奖励
    /// </summary>
    public Reward reward;

    /// <summary>
    /// 难度
    /// </summary>
    public byte stars;

    /// <summary>
    /// 任务类型
    /// </summary>
    protected ConstantDefine.QuestType m_type;

    public ConstantDefine.QuestType GetQuestType ( )
    {
        return m_type;
    }

    /// <summary>
    /// 任务进行的状态
    /// None-->Claimed-->InProgress-->Completed
    ///   ↑                               ↓
    ///   ------------领取奖励-------------
    /// </summary>
    protected ConstantDefine.QuestState m_state;

    public ConstantDefine.QuestState GetQuestState ( )
    {
        return m_state;
    }

    public Quest (string name, string scene, string desc, byte diff)
    {
        questName = name;
        sceneName = scene;
        description = desc;
        stars = diff;
        m_state = ConstantDefine.QuestState.None;
    }


    /// <summary>
    /// 接到某个任务时
    /// </summary>
    virtual public void OnClaim ( )
    {
        m_state = ConstantDefine.QuestState.InProgress;
    }

    /// <summary>
    /// 完成某个任务时
    /// </summary>
    virtual public void OnCompleted ( )
    {
        //到NPC处交任务时要检查是否是完成状态
        m_state = ConstantDefine.QuestState.Completed;
        
        //不再监听相应的事件
    }

    /// <summary>
    /// 任务失败时
    /// </summary>
    virtual public void OnFailed ( )
    {
        m_state = ConstantDefine.QuestState.Failed;
    }

    /// <summary>
    /// 得到奖励
    /// </summary>
    /// <param name="reward">奖励</param>
    virtual public void OnGetReward ( )
    {
        m_state = ConstantDefine.QuestState.None;

        //Quest奖励需要UI
        reward.DisplayRewardUI();
    }

    /// <summary>
    /// 检查是否完成
    /// </summary>
    /// <returns></returns>
    abstract protected bool CheckCompletion ( );

}