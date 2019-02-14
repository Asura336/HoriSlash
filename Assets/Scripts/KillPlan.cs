using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 接触到触发器的怪物被送回对象池
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class KillPlan : MonoBehaviour
{
    private void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var monster = other.GetComponent<Monster>();
        if (monster != null && EnemyFactory.Instance != null)
        {
            EnemyFactory.Instance.Pool.DeleteCell(monster.gameObject);
        }
    }
}
