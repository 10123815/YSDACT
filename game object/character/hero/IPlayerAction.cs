/*************************************************************

** Auth: ysd
** Date: 15.7.18
** Desc: 角色移动
** Vers: v1.0

*************************************************************/

using UnityEngine;
using System.Collections;

public interface IPlayerAction 
{

    void PlayerMove (Vector3 movement, float speed);

    void PlayerJump (Vector3 movement);

    void PlayerDash (Vector3 movement);
	
}
