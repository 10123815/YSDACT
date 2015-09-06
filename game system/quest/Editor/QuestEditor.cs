/*************************************************************

** Auth: ysd
** Date: 15.7.26
** Desc: 任务编辑器
** Vers: v1.0

*************************************************************/

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Xml;
using System.Collections;

public class QuestEditor : ScriptableWizard
{

    public string questName = "";
    public string desc = "";
    public int difficulty = 0;
    public Transform questButton;

    //[MenuItem("Window/Quest Editor")]
    //static public void Init ( )
    //{
    //    DisplayWizard<QuestEditor>("Quest Editor", "Create");
    //}

    void OnWizardCreate ( )
    {
        questButton.GetChild(0).GetComponent<Text>().text = questName;
        var stars = questButton.GetChild(1).GetComponentsInChildren<Image>();
        difficulty = difficulty > 3 ? 2 : difficulty - 1;
        Sprite sprite = Resources.Load<Sprite>("ui/quest/gold_star");
        for (int i = 0; i < difficulty; i++)
        {
            Debug.Log(stars[i].name);
            stars[i].sprite = sprite;
        }
    }

    [MenuItem("Window/test")]
    static public void Init ()
    {
        XmlDocument xmlDoc = new XmlDocument();
#if UNITY_EDITOR
        xmlDoc.Load(Application.dataPath + "/Resources/quest/quests.xml");
#elif UNITY_ANDROID
        TextAsset textAsset = (TextAsset)Resources.Load("quest/quests", typeof(TextAsset));
        xmlDoc.LoadXml(textAsset.text);
#endif
        
    }
}