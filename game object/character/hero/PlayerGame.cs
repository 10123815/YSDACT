/*************************************************************

** Auth: ysd
** Date: 15.8.29
** Desc: 游戏中玩家角色行动
** Vers:v1.0

*************************************************************/

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerGame : NetworkBehaviour, IPlayerAction
{

    public float stationaryTurnSpeed = 180;
    public float movementTurnSpeed = 360;
    public byte jumpForce = 2;

    private Transform m_transform;
    private Animator m_animator;
    private Rigidbody m_rigidbody;

    private bool m_onGround = true;
    private bool m_airMove = false;

    void Start ( )
    {
        m_transform = transform;
        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnStartAuthority ( )
    {
        base.OnStartAuthority();
    }

    #region IPlayerAction 成员
    [ClientCallback]
    public void PlayerMove (Vector3 movement, float speed)
    {
        if (m_onGround)
        {
            if (movement.magnitude > 0.1f)
            {

                m_animator.SetBool("move", true);

                Vector3 groundNormal;
                m_onGround = CommonUtility.CheckGroundState(m_transform.position, out groundNormal);

                if (m_onGround)
                {
                    movement = m_transform.InverseTransformDirection(movement);
                    movement = Vector3.ProjectOnPlane(movement, groundNormal);
                    float turnAmount = Mathf.Atan2(movement.x, movement.z);
                    m_transform.Rotate(0, Mathf.Lerp(stationaryTurnSpeed, movementTurnSpeed, movement.z) * turnAmount * Time.deltaTime, 0);
                    m_transform.Translate(Vector3.forward * PlayerData.GetInstance().floatValues["move speed"] * (Mathf.PI - Mathf.Abs(turnAmount) / 2) / Mathf.PI * Time.deltaTime);
                }
            }
            else
            {
                m_animator.SetBool("move", false);
            }
        }
        else if (m_airMove)
        {
            m_transform.Translate(Vector3.forward * PlayerData.GetInstance().floatValues["move speed"] * Time.deltaTime);
        }
    }

    [ClientCallback]
    public void PlayerJump (Vector3 movement)
    {
        if (m_onGround)
        {
            m_animator.SetTrigger("jump");
            m_onGround = false;
            //跳跃式摇杆，则向前跳跃
            m_airMove = movement.magnitude > 0.1f;
            Vector3 extraGravityForce = -Physics.gravity * jumpForce;
            m_rigidbody.AddForce(extraGravityForce, ForceMode.Impulse);
        }

    }

    [ClientCallback]
    public void PlayerDash (Vector3 direction)
    {

    }

    #endregion
}