/*************************************************************

** Auth: ysd
** Date: 15.7.8
** Desc: 房间内player
** Vers: v1.0

*************************************************************/

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerLobby : NetworkLobbyPlayer, IPlayerAction
{

    [SyncVar]
    public string playerName;

    public float stationaryTurnSpeed = 180;
    public float movementTurnSpeed = 360;

    //[SerializeField]
    //private Vector3 m_direction;

    private Transform m_transform;

    private Animator m_animator;

    /// <summary>
    /// Spawn在本地的其他用户的用户信息，离开时销毁
    /// </summary>
    private GameObject m_peerInfoPanel;

    void Awake ( )
    {

        if (isLocalPlayer)
        {
        }

    }

    // Use this for initialization
    void Start ( )
    {
        OtherStart();
    }

    public override void OnClientEnterLobby ( )
    {
        
    }

    public override void OnStartAuthority ( )
    {
        
    }

    public override void OnStartClient ( )
    {
        
    }

    /// <summary>
    /// 初始化本地的该物体
    /// </summary>
    public override void OnStartLocalPlayer ( )
    {
        m_transform = transform;
        m_animator = GetComponent<Animator>();
        _SetMainCamera();
        CommonUtility.AddAvatarForLoaclPlayer(transform);
    }

    private void _SetMainCamera ( )
    {
        var cameraFollow = Camera.main.gameObject.AddComponent<CameraFollow>();
        cameraFollow.SetFollow(transform);
    }

    /// <summary>
    /// 初始化本设备上的其他用户的lobbyplayer
    /// </summary>
    void OtherStart ( )
    {
        if (isClient && !isLocalPlayer)
        {
            m_peerInfoPanel = CommonUtility.AddAvatarForOthers(transform, playerName);
        }
    }

    // Update is called once per frame
    //void Update ( )
    //{
    //    if (!hasAuthority)
    //    {
    //        return;
    //    }

    //    PlayerMove();
    //}

    public void OnDestroy ( )
    {
        if (m_peerInfoPanel != null)
            Destroy(m_peerInfoPanel);
    }

    #region IPlayerMove 成员

    [ClientCallback]
    void IPlayerAction.PlayerMove (Vector3 movement, float speed)
    {
        if (movement.magnitude < 0.1f)
        {
            m_animator.SetBool("move", false);
            return;
        }

        m_animator.SetBool("move", true);
        movement = m_transform.InverseTransformDirection(movement);
        float turnAmount = Mathf.Atan2(movement.x, movement.z);
        m_transform.Rotate(0, Mathf.Lerp(stationaryTurnSpeed, movementTurnSpeed, movement.z) * turnAmount * Time.deltaTime, 0);
        m_transform.Translate(Vector3.forward * PlayerData.GetInstance().floatValues["move speed"] * (Mathf.PI - Mathf.Abs(turnAmount) / 2) / Mathf.PI * Time.deltaTime);

    }

    public void PlayerJump (Vector3 movement)
    {
        //do nothing
    }

    public void PlayerDash (Vector3 movement)
    {
        //do nothing
    }

    #endregion
}
