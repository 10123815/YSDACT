/*************************************************************

** Auth: ysd
** Date: 15.7.23
** Desc: 在hierarchy中找出带有某个Component的GameObject
** Vers: v1.0

*************************************************************/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class Finder : ScriptableWizard
{

    public string componentName = "";

    [MenuItem("Window/Finder")]
    public static void Init()
    {
        DisplayWizard<Finder>("Find GameObjects with Component", "FIND");
    }
    
    void OnWizardCreate()
    {
        var gos = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        List<GameObject> results = new List<GameObject>();
        for (int i = 0; i < gos.Length; i++)
        {
            if (gos[i].GetComponent(componentName))
            {
                results.Add(gos[i]);
            }
        }
        Selection.objects = results.ToArray();
    }
}