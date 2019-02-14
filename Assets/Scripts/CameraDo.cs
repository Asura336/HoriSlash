using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDo : MonoBehaviour
{
    public Transform target;
    Vector3 pixel;

    public float followPower = 0.25f;
    public float rotatePower = 0.5f;

    // Use this for initialization
    void Start()
    {
        pixel = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameMode.Instance.IsPlayerAlive)
        {
            var newPos = target.position + pixel;
            newPos.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position, newPos, followPower);
        }
    }

    void LookNewForward(Vector3 newForward)
    {
        var r = Quaternion.LookRotation(newForward);
        transform.rotation = Quaternion.Slerp(transform.rotation, r, rotatePower);
    }
}
