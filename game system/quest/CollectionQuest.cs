/*************************************************************

** Auth: ysd
** Date: 15.7.26
** Desc: 采集任务
** Vers: v1.0

*************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollectionQuest : Quest
{


    /// <summary>
    /// 任务目标
    /// </summary>
    protected ConstantDefine.CollectionType m_targetType;

    public ConstantDefine.CollectionType TargetType
    {
        get
        {
            return m_targetType;
        }
    }

    protected byte m_targetNumber;

    public byte TargetNumber
    {
        get
        {
            return m_targetNumber;
        }
    }

    protected byte m_completedNumber;

    public CollectionQuest (string name, string scene, string desc, byte diff, ConstantDefine.CollectionType type, byte number)
        : base(name, scene, desc, diff)
    {
        m_type = ConstantDefine.QuestType.Collect;
        m_targetType = type;
        m_targetNumber = number;
        m_completedNumber = 0;
    }

    protected override bool CheckCompletion ( )
    {
        return m_completedNumber >= m_targetNumber;
    }

    protected void OnCollectOne( )
    {
        m_completedNumber++;
        if (CheckCompletion())
        {
            OnCompleted();
        }
    }

    public override void OnClaim ( )
    {
        base.OnClaim();

        //如果背包中已有目标物品....
        m_completedNumber = BagManager.GetInstance().sundryBag.GetCount(TargetType);
        if (CheckCompletion())
        {
            OnCompleted();
        }
        else
        {
            //注册
            QuestManager.GetInstance().RegisterCollectionQuestEventHandler(TargetType, this.OnCollectOne);
        }
    }

    public override void OnCompleted ( )
    {
        base.OnCompleted();

        QuestManager.GetInstance().RemoveCollectionQuestEventHandler(TargetType, this.OnCollectOne);
    }

    public override void OnGetReward ( )
    {
        //打开领取奖励UI
        base.OnGetReward( );
        //上交任务物品
        BagManager.GetInstance().sundryBag.Remove(TargetType, TargetNumber);
    }

    public override void OnFailed ( )
    {
        base.OnFailed();
    }

}