/*************************************************************

** Auth: ysd
** Date: 15.7.13
** Desc: 泛型单例，使用
         public static T GetInstance ( )
         {
            return Singleton<T>.GetInstance();
         }
         来获得T的唯一实例，需要非公的无参构造函数
** Vers: v1.0

*************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

public class Singleton<T> where T : class
{

    private static T m_instace;

    public static T GetInstance ( )
    {
        if (m_instace == null)
        {
            Type type = typeof(T);
            ConstructorInfo ctor;
            ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                          null, new Type[0], new ParameterModifier[0]);
            m_instace = (T)ctor.Invoke(new object[0]);
        }
        return m_instace;
    }
}

