/*************************************************************

** Auth: ysd
** Date: 15.7.25
** Desc: 所有NPC的基类
         一个NPC不一定是可以点击的/对话的
** Vers: v1.0

*************************************************************/

using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour
{

    /// <summary>
    /// NPC的类型，tag应与其一致
    /// </summary>
    protected ConstantDefine.NPCType m_npcType;
    
}