/*************************************************************

** Auth: ysd
** Date: 15.7.25
** Desc: 讨伐任务，杀某些类型的敌人若干只
** Vers: v1.0

*************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrusadeQuest : Quest
{

    /// <summary>
    /// 任务目标
    /// </summary>
    protected List<QuestTarget<ConstantDefine.EnemyType>> m_targets;

    public List<QuestTarget<ConstantDefine.EnemyType>> Targets
    {
        get
        {
            return m_targets;
        }
    }

    public CrusadeQuest (string name, string scene, string desc, byte diff, List<QuestTarget<ConstantDefine.EnemyType>> targets)
        : base(name, scene, desc, diff)
    {
        m_type = ConstantDefine.QuestType.Crusade;
        m_targets = targets;
    }

    public override void OnClaim ( )
    {
        //变更状态
        base.OnClaim();

        //注册监听器
        ConstantDefine.EnemyType[] types = new ConstantDefine.EnemyType[Targets.Count];
        for (int i = 0; i < types.Length; i++)
        {
            types[i] = Targets[i].targetType;
        }
        QuestManager.GetInstance().RegisterCrusadeQuestEventHandler(types, UpdateProgress);

        //UI不在这里
    }

    public override void OnCompleted ( )
    {
        //变更状态
        base.OnCompleted();

    }

    public override void OnFailed ( )
    {
        base.OnFailed();
    }

    protected override bool CheckCompletion ( )
    {
        bool isCompleted = true;
        for (int i = 0; i < Targets.Count; i++)
        {
            isCompleted &= Targets[i].completedNumber >= Targets[i].targetNumber;
        }
        return isCompleted;
    }

    /// <summary>
    /// 监听任务所需击杀的敌人的死亡事件，注册在QuestManager
    /// </summary>
    protected void UpdateProgress (byte id, ConstantDefine.EnemyType type)
    {

        for (int i = 0; i < Targets.Count; i++)
        {
            if (Targets[i].targetType.Equals(type))
            {
                Targets[i].completedNumber++;
            }
        }

        //检查是否完成
        if (CheckCompletion())
        {
            OnCompleted();
        }
    }
}