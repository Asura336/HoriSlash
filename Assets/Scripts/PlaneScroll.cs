using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneScroll : MonoBehaviour
{
    public Transform planA;
    public Transform planB;
    Transform player;

    [Header("__Don't touch it__")]
    public float planWidth = 50;
    public float allPlanWidth = 100;

    Vector3 _posA;
    Vector3 _posB;

    // Use this for initialization
    void Start()
    {
        player = GameMode.Instance.PlayerTrans;
        _posA = planA.position;
        _posB = planB.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameMode.Instance.IsPlayerAlive)
        {
            OnPlayerMoving(planA, _posA);
            OnPlayerMoving(planB, _posB);
        }
    }

    void OnPlayerMoving(Transform plan, Vector3 ori)
    {
        float dist = player.position.z - ori.z;
        int n = Mathf.RoundToInt(dist / allPlanWidth);
        plan.position = new Vector3(ori.x, ori.y, ori.z + n * allPlanWidth);
    }
}
