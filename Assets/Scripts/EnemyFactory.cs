using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可被外界调用的接口项:
///     <see cref="SetInstance"/> 函数, 设置新的怪物并指定位置, 活动状态, 颜色, 速度. 
/// </summary>
public class EnemyFactory : MonoBehaviour
{
    public static EnemyFactory Instance { get; private set; } = null;
    public GObjPool Pool { get; private set; }
    public GameObject prefab;
    public Vector3 relaxPos;

    public int ActiveCount { get { return Pool.ActiveCount; } }

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            Pool = new GObjPool(prefab, relaxPos);
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    public void SetInstance(Vector3 mPos, MonsterActState mState, MonsterColor mColor, float mSpeed)
    {
        GameObject obj = Pool.BirthCell();
        obj.transform.position = mPos;
        Monster monster = obj.GetComponent<Monster>();
        monster.MonsterState = mState;
        monster.OnSetState();
        monster.ColorType = mColor;
        monster.SetInitSpeed(mSpeed);
    }
}
