/*************************************************************

** Auth: ysd
** Date: 7.18
** Desc: 摄像机跟随，玩家不可操作，根据场景动态移动相机
** Vers:

*************************************************************/

using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{

    static public CameraFollow instance;

    [System.NonSerialized]
    public Transform target;

    private Vector3 m_offset;
    private Transform m_tf;

    private MessageDefine.CameraMoveCallback m_cameraMoveMethod = delegate
    {
    };

    void Awake ( )
    {
        if (!tag.Equals("MainCamera"))
            Debug.LogError("CameraFollow必须放在主相机上");

        instance = this;
        m_tf = transform;
    }

    // Use this for initialization
    void Start ( )
    {
    }

    // Update is called once per frame
    void LateUpdate ( )
    {
        m_cameraMoveMethod();
    }

    protected virtual void Follow ( )
    {
        if (target != null)
            m_tf.position = target.position + m_offset;
    }

    /// <summary>
    /// 设置相机的初始位置并设置其跟随的目标
    /// 在Editor中调整好相机的位置，记录其rotation的x和y
    /// </summary>
    /// <param name="t">目标</param>
    /// <param name="distance">和目标的距离</param>
    /// <param name="angleX">相机rotation的x</param>
    /// <param name="angleY">相机rotation的y</param>
    public void SetFollow (Transform t, float distance = 10, float angleX = 50, float angleY = 135)
    {
        target = t;
        //向左转
        Vector3 targetDir = Quaternion.AngleAxis(-angleY, Vector3.up) * Vector3.right;
        Vector3 right = Quaternion.AngleAxis(90 - angleY, Vector3.up) * Vector3.right;
        //向上转
        targetDir = Quaternion.AngleAxis(-angleX, right) * targetDir;
        m_offset = distance * targetDir.normalized + ConstantDefine.playerHeadPosition;
        transform.position = target.position + m_offset;
        transform.LookAt(target);
        m_cameraMoveMethod = Follow;
    }

}
