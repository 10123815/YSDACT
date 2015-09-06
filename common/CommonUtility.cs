/*************************************************************

** Auth: ysd
** Date: 15.7.19
** Desc: 常用函数
** Vers: v1.0

*************************************************************/

using UnityEngine;
using System;
using System.Reflection;
using System.Xml;
using System.Collections.Generic;
using System.Collections;

static public class CommonUtility
{

    /// <summary>
    /// 检查地面状态，斜坡？。。。
    /// </summary>
    /// <param name="foot">某个gameobject的脚底坐标</param>
    /// <param name="distance">距离偏移</param>
    /// <param name="groundNormal">输出，地面的法向量</param>
    /// <param name="maxAngle">允许移动的斜坡最大角度</param>
    /// <returns>某个点是否在地面上</returns>
    static public bool CheckGroundState (Vector3 foot, out Vector3 groundNormal, float maxAngle = 0)
    {
        //斜坡？
        maxAngle = maxAngle > 60 ? 60 : maxAngle;
        float offset = 0.3f;
        if (maxAngle > 1)
            offset = 1 / Mathf.Cos(maxAngle * Mathf.Deg2Rad) * 0.3f;

        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(foot + Vector3.up * 0.25f, Vector3.down, out hit, offset))
        {
            groundNormal = hit.normal;
            return true;
        }
        else
        {
            groundNormal = Vector3.zero;
            return false;
        }
    }

    /// <summary>
    /// 为本地player添加头像到UI
    /// </summary>
    /// <param name="localPlayer"></param>
    static public void AddAvatarForLoaclPlayer (Transform localPlayer)
    {
        RenderTexture rt = _GetAvatar(localPlayer);
        //设置UI
        GameObject.FindGameObjectWithTag("avatar0").GetComponent<UnityEngine.UI.RawImage>().texture = rt;
    }

    /// <summary>
    /// 为本体其他用户player添加头像到UI
    /// </summary>
    /// <param name="other">被spawn到本地，但不归本地用户控制</param>
    static public GameObject AddAvatarForOthers (Transform other, string name)
    {
        var rt = _GetAvatar(other);
        var peerInfoPanel = GameObject.Instantiate(Resources.Load("ui/character info/peer info")) as GameObject;
        peerInfoPanel.GetComponentInChildren<UnityEngine.UI.RawImage>().texture = rt;
        peerInfoPanel.GetComponentInChildren<UnityEngine.UI.Text>().text = name;
        peerInfoPanel.transform.SetParent(GameObject.FindGameObjectWithTag("peer list").transform);
        peerInfoPanel.transform.localScale = Vector3.one;
        return peerInfoPanel;
    }

    /// <summary>
    /// 获取某个player的头像
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    private static RenderTexture _GetAvatar (Transform player)
    {
        RenderTexture rt = new RenderTexture(64, 64, 0);
        rt.depth = 16;
        player.GetComponentInChildren<Camera>().targetTexture = rt;
        return rt;
    }

    #region 从XML获取游戏数据

    /// <summary>
    /// 从XML中读取任务信息
    /// </summary>
    /// <param name="questName">任务名字</param>
    /// <returns></returns>
    static public Quest CreateQuestFromXmlWithQuestName (string questName = "Hell")
    {
        string path = "/quests/quest[name='" + questName + "']/";
        Quest quest = _CreateQuestWithXPath(path);
        return quest;
    }

    /// <summary>
    /// 取得Xml中第index个任务
    /// </summary>
    static public Quest CreateQuestFromXmlAt (byte index)
    {
        string path = "/quests/quest[" + index.ToString() + "]/";
        Quest quest = _CreateQuestWithXPath(path);
        return quest;
    }

    static public List<Quest> CreateAllQuestsFromXml ( )
    {
        List<Quest> quests = new List<Quest>();
        byte i = 1;
        while (true)
        {
            Quest quest = CreateQuestFromXmlAt(i);
            if (quest == null)
                break;
            i++;
            quests.Add(quest);
        }
        return quests;
    }

    /// <summary>
    /// 根据特定的XPath读取任务信息
    /// </summary>
    /// <param name="path">已包含特定信息，如任务名、序号等</param>
    /// <returns></returns>
    private static Quest _CreateQuestWithXPath (string path)
    {
        XmlElement root = _GetXmlRootElement();
        XmlNode nameNode = root.SelectSingleNode(path + "name");
        if (nameNode == null)
            return null;

        string name = nameNode.InnerText;
        string scene = root.SelectSingleNode(path + "scene").InnerText;
        string description = root.SelectSingleNode(path + "description").InnerText;
        byte star = byte.Parse(root.SelectSingleNode(path + "stars").InnerText);
        ConstantDefine.QuestType type = (ConstantDefine.QuestType)byte.Parse(root.SelectSingleNode(path + "type").InnerText);
        Quest quest = null;
        switch (type)
        {
            case ConstantDefine.QuestType.Crusade:
                //任务目标
                List<QuestTarget<ConstantDefine.EnemyType>> targets = new List<QuestTarget<ConstantDefine.EnemyType>>();
                XmlNodeList idList = root.SelectNodes(path + "target/id");
                XmlNodeList numberList = root.SelectNodes(path + "target/number");
                for (int i = 0; i < idList.Count; i++)
                {
                    QuestTarget<ConstantDefine.EnemyType> questTarget = new QuestTarget<ConstantDefine.EnemyType>();
                    questTarget.targetType = (ConstantDefine.EnemyType)byte.Parse(idList[i].InnerText);
                    questTarget.targetNumber = byte.Parse(numberList[i].InnerText);
                    questTarget.completedNumber = 0;
                    targets.Add(questTarget);
                }
                //创建任务
                quest = new CrusadeQuest(name, scene, description, star, targets);
                break;
            case ConstantDefine.QuestType.Collect:
                //任务目标
                ConstantDefine.CollectionType collectionType = (ConstantDefine.CollectionType)byte.Parse(root.SelectSingleNode(path + "id").InnerText);
                byte number = byte.Parse(root.SelectSingleNode(path + "number").InnerText);
                //创建任务
                quest = new CollectionQuest(name, scene, description, star, collectionType, number);
                break;
            default:
                break;
        }
        quest.reward = GetRewardInfoFromXmlByAt(byte.Parse(root.SelectSingleNode(path + "reward").InnerText));
        return quest;
    }

    public static ViewableItemInfo GetViewableItemInfoFromXmlByType (ConstantDefine.UnCollectionType type)
    {
        ViewableItemInfo info;
        string path = "/items/item[" + ((byte)type).ToString() + "]/";
        XmlElement root = _GetXmlRootElement("item/viewable item");
        info.name = root.SelectSingleNode(path + "name").InnerText;
        info.info = root.SelectSingleNode(path + "info").InnerText;
        return info;
    }

    public static CollectableItemInfo GetCollectableItemInfoFromXmlByType (ConstantDefine.CollectionType type)
    {
        CollectableItemInfo info;
        string path = "/items/item[" + ((byte)type).ToString() + "]/";
        XmlElement root = _GetXmlRootElement("item/collectable item");
        info.name = root.SelectSingleNode(path + "name").InnerText;
        info.info = root.SelectSingleNode(path + "info").InnerText;
        return info;
    }

    public static CollectableItemInfo GetCollectableItemInfoFromXmlAt (byte index)
    {
        CollectableItemInfo info;
        string path = "/items/item[" + index.ToString() + "]/";
        XmlElement root = _GetXmlRootElement("item/collectable item");
        info.name = root.SelectSingleNode(path + "name").InnerText;
        info.info = root.SelectSingleNode(path + "info").InnerText;
        return info;
    }

    public static Reward GetRewardInfoFromXmlByAt (byte index)
    {
        Reward reward = new Reward();
        string path = "/rewards/reward[" + index.ToString() + "]/";
        XmlElement root = _GetXmlRootElement("reward/reward");
        int exp = 0;
        if (int.TryParse(root.SelectSingleNode(path + "exp").InnerText, out exp))
        {
            reward.exp = exp;
            int gold = 0;
            if (int.TryParse(root.SelectSingleNode(path + "gold").InnerText, out gold))
            {
                reward.gold = gold;
                XmlNodeList nameNodes = root.SelectNodes(path + "items/name");
                XmlNodeList idNodes = root.SelectNodes(path + "items/id");
                XmlNodeList countNodes = root.SelectNodes(path + "items/count");
                BagItem[] bagItems = new BagItem[nameNodes.Count];
                byte[] counts = new byte[nameNodes.Count];
                for (int i = 0; i < nameNodes.Count; i++)
                {
                    //数量
                    byte count = 0;
                    if (byte.TryParse(countNodes[i].InnerText, out count))
                    {
                        counts[i] = count;
                        Type type = Type.GetType(nameNodes[i].InnerText);
                        if (type == null)
                        {
                            Debug.LogError("没有该物品");
                        }
                        byte id = 0;
                        ConstantDefine.CollectionType itemType;
                        if (byte.TryParse(idNodes[i].InnerText, out id))
                        {
                            itemType = (ConstantDefine.CollectionType)id;
                            BagItem item = Activator.CreateInstance(type, new object[1] { itemType }) as BagItem;
                            if (item != null)
                            {
                                bagItems[i] = item;
                            }
                        }
                    }
                    reward.items = bagItems;
                    reward.counts = counts;
                }
            }
        }
        return reward;
    }

    private static XmlElement _GetXmlRootElement (string path = "quest/quests")
    {
        XmlDocument xmlDoc = new XmlDocument();
        //#if UNITY_EDITOR
        // xmlDoc.Load(Application.dataPath + ConstantDefine.questXmlFileUrl);
        //#elif UNITY_ANDROID
        TextAsset textAsset = (TextAsset)Resources.Load(path, typeof(TextAsset));
        xmlDoc.LoadXml(textAsset.text);
        //#endif
        XmlElement root = xmlDoc.DocumentElement;
        return root;
    }


    #endregion

    #region UI

    public static GameObject GetMenuItemPanel (string tag, string path)
    {
        GameObject panel = GameObject.FindGameObjectWithTag(tag);
        if (panel == null)
        {
            panel = GameObject.Instantiate(Resources.Load(path)) as GameObject;
            panel.transform.GetChild(0).localPosition = Vector2.one * 10000;
        }
        return panel;
    }

    #endregion

    #region 扩展方法

    static public Vector3 GetPositionOffset (this Transform transform, Vector3 offset)
    {
        return new Vector3(transform.position.x + offset.x, transform.position.y + offset.y, transform.position.z + offset.z);
    }

    static public Vector3 GetPositionOffset (this Transform transform, float x, float y, float z)
    {
        return transform.GetPositionOffset(new Vector3(x, y, z));
    }

    #endregion


}

internal class CoroutineRunner : MonoBehaviour
{
    
}

public class CoroutineManager
{

    private static CoroutineRunner m_coroutineRunner;

    public static CoroutineManager GetInstance ( )
    {
        return Singleton<CoroutineManager>.GetInstance();
    }

    private CoroutineManager ( )
    {
        GameObject go = new GameObject();
        go.name = "CoroutineManager";
        m_coroutineRunner = go.AddComponent<CoroutineRunner>();
        GameObject.DontDestroyOnLoad(go);
    }

    public void StartCoroutine (IEnumerator coroutine)
    {
        m_coroutineRunner.StartCoroutine(coroutine);
    }

}