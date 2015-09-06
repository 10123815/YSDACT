/*************************************************************

** Auth: ysd  
** Date: 15.8.29
** Desc: 游戏中玩家输入
** Vers: v1.0

*************************************************************/

using UnityEngine;
using DG.Tweening;
using System.Collections;

public class PlayerGameInput : PlayerLobbyInput
{

    protected override void GetInput ( )
    {
        base.GetInput();

        if (ETCInput.GetButtonDown("jump"))
            m_playerAction.PlayerJump(m_movement);

        if (ETCInput.GetButtonDown("dash"))
            m_playerAction.PlayerDash(m_movement);

    }

}