/*************************************************************

** Auth: ysd
** Date: 15.7.18
** Desc: 只获取用户输入，来自EasyTouch
** Vers: v1.0

*************************************************************/

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(IPlayerAction))]
[RequireComponent(typeof(NetworkIdentity))]
public class PlayerLobbyInput : MonoBehaviour
{

    public static PlayerLobbyInput instance;

    //角色移动
    protected Vector3 m_movement;
    protected IPlayerAction m_playerAction;

    protected NetworkIdentity m_netId;

    protected Transform m_camera;

    void Awake ( )
    {
    }

    // Use this for initialization
    void Start ( )
    {
        m_netId = GetComponent<NetworkIdentity>();
        //if (!m_netId.hasAuthority)
        //{
        //    return;
        //}

        instance = this;
        m_playerAction = GetComponent<IPlayerAction>();
        m_camera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update ( )
    {
        if (!m_netId.hasAuthority)
        {
            return;
        }

        GetInput();
        ChechClick();
    }

    protected virtual void GetInput ( )
    {
        Vector3 m_CamForward = Vector3.Scale(m_camera.forward, new Vector3(1, 0, 1)).normalized;
        m_movement = ETCInput.GetAxis("v") * m_CamForward + ETCInput.GetAxis("h") * m_camera.right;

        //移动
        m_playerAction.PlayerMove(m_movement, PlayerData.GetInstance().floatValues["move speed"]);
    }

    protected virtual void ChechClick ( )
    {

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject go = hit.collider.gameObject;
                IClickable clickableGameObject = go.GetComponent<IClickable>();
                if (clickableGameObject != null)
                {
                    float distance = Vector3.Distance(transform.position, hit.collider.transform.position);
                    clickableGameObject.OnClick(distance);
                }
            }
        }
#elif UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount > 0 && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(0))
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject go = hit.collider.gameObject;
                    IClickable clickableGameObject = go.GetComponent<IClickable>();
                    if (clickableGameObject != null)
                    {
                        float distance = Vector3.Distance(transform.position, hit.collider.transform.position);
                        clickableGameObject.OnClick(distance);
                    }
                }
            }
        }
#endif

    }
}
