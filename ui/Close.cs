/*************************************************************

** Auth: ysd
** Date: 15.8.22
** Desc: 关闭UI
** Vers: v1.0

*************************************************************/

using UnityEngine;
using System.Collections;

public class Close : MonoBehaviour
{

    public GameObject canvas;

    public void CloseCanvas()
    {
        if (!CommonCanvasManager.GetInstance().Return(canvas.name, true))
            canvas.SetActive(false);
    }

}