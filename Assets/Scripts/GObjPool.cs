using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 泛用的 GameObject 对象池
/// <see cref="BirthCell"/> 从池提取节点
/// <see cref="DeleteCell(UnityEngine.GameObject)"/> 将节点送回池
/// </summary>
public class GObjPool
{
    public int InitLen { get; set; } = 8;
    public float DevelopMultiplier { get; set; } = 1.5f;
    public int WaterLine { get; set; } = 4;
    public int ActiveCount { get; private set; } = 0;
    public Vector3 RelaxPos { get; set; }

    public GameObject Prefab { get; set; }

    private Queue<GameObject> useableObjs = null;
    private List<GameObject> allObjs = null;
    private Dictionary<GameObject, IGObjNode> obj2I = null;

    public GObjPool(GameObject prefab, Vector3 relaxPos, int initLen = 8)
    {
        if (prefab.GetComponent<IGObjNode>() == null)
        {
            Debug.LogError("预制体需要继承接口 IGObjNode。");
        }
        Prefab = prefab;
        InitLen = initLen;
        RelaxPos = relaxPos;
        useableObjs = new Queue<GameObject>();
        allObjs = new List<GameObject>();
        obj2I = new Dictionary<GameObject, IGObjNode>();

        IncreaseTo(InitLen);
    }

    // 扩增对象池到指定长度
    void IncreaseTo(int newLen)
    {
        for (int i = 0; i < newLen - allObjs.Count; i++)
        {
            GameObject fresh = Object.Instantiate(Prefab, RelaxPos, Quaternion.identity, null);
            allObjs.Add(fresh);
            obj2I[fresh] = fresh.GetComponent<IGObjNode>();
            obj2I[fresh].Master = this;
            DeleteCell(fresh);
        }
    }

    // 从队列拿出新成员
    public GameObject BirthCell()
    {
        if (useableObjs.Count < WaterLine)
        {
            IncreaseTo(Mathf.FloorToInt(DevelopMultiplier * allObjs.Count));
        }
        GameObject reverse = useableObjs.Dequeue();
        obj2I[reverse].OnNodeBirth();
        ActiveCount++;
        return reverse;
    }

    // 将销毁成员退给队列
    public void DeleteCell(GameObject obj)
    {
        if (!obj2I.ContainsKey(obj)) { return; }
        obj.transform.position = RelaxPos;
        useableObjs.Enqueue(obj);
        obj2I[obj].OnNodeDead();
        ActiveCount--;
    }
}

/// <summary>
/// GObjPool 对象池节点需要继承此接口
/// <see cref="Master"/>节点所属的对象池
/// <see cref="OnNodeBirth"/> 节点从对象池取出时调用的函数，决定节点的具体行为
/// <see cref="OnNodeDead"/> 节点退回对象池时调用的函数，决定节点的具体行为
/// </summary>
public interface IGObjNode
{
    GObjPool Master { get; set; }
    void OnNodeBirth();  // 节点出生
    void OnNodeDead();  // 节点销毁
}