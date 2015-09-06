/*************************************************************

** Auth: ysd
** Date: 15.7.18
** Desc: 所有需要保存的用户数据
** Vers: v1.0

*************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerData
{

    public Dictionary<string, string> stringValues;
    public Dictionary<string, int> intValues;
    public Dictionary<string, byte> byteValues;
    public Dictionary<string, float> floatValues;

    public static PlayerData GetInstance ( )
    {
        return Singleton<PlayerData>.GetInstance();
    }

    private PlayerData ( )
    {
        stringValues = new Dictionary<string, string>();
        intValues = new Dictionary<string, int>();
        byteValues = new Dictionary<string, byte>();
        floatValues = new Dictionary<string, float>();

        #region test
        floatValues.Add("move speed", 3);
        intValues.Add("exp", 0);
        intValues.Add("gold", 0);
        #endregion
    }

    /// <summary>
    /// 保存某个属性
    /// </summary>
    /// <typeparam name="T">属性类型</typeparam>
    /// <param name="key">属性名</param>
    public void SaveValue<T>(string key)
    {
        
    }

    public void SaveAll()
    {
        
    }

}
