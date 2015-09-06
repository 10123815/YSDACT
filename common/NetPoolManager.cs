/*************************************************************

** Auth: ysd
** Date: 15.7.13
** Desc: 池管理器，管理多个对象池;
         对象池应该支持Network，即某个对象被放回池中时，其他客户端
         的同一个对象也应该被放回本地的对象池中
** Vers: v1.0

*************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetPoolManager 
{

    private static Dictionary<string, ObjectPool> m_pools = new Dictionary<string,ObjectPool>();

    /// <summary>
    /// 单例
    /// </summary>
    /// <returns></returns>
	public static NetPoolManager GetInstance()
    {
        return Singleton<NetPoolManager>.GetInstance();
    }

    private NetPoolManager ( )
    {
    }

    public class ObjectPool
	{
		
	}

}
