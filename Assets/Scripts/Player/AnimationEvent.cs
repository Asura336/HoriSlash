using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    public SphereCollider attackArea;
    Animator animator;
    float oriAnimSpeed;

    private void Start()
    {
        animator = GetComponent<Animator>();
        oriAnimSpeed = animator.speed;

        attackArea.isTrigger = true;
        attackArea.radius = 1.5f;
        attackArea.center = Vector3.zero;
        attackArea.transform.localPosition = new Vector3(0, 0, 0.5f);
    }

    // L = TypeOne and R = TypeTwo
    public void OnAnimationEvent_Attack(int flag)
    {
        var tars = Physics.OverlapSphere(attackArea.transform.position,
                                         attackArea.radius, 
                                         GameMode.EnemyLayerMask);
        bool hit = false;
        foreach (var tar in tars)
        {
            var monster = tar.transform.GetComponent<Monster>();
            if (monster)
            {
                if (monster.ColorType == MonsterColor.NONE || 
                   (monster.ColorType == MonsterColor.TYPE_ONE && flag == 'L') ||
                   (monster.ColorType == MonsterColor.TYPE_TWO && flag == 'R'))
                {
                    monster.OnBeDamage(flag);
                    hit = true;
                }
            }
        }
        if (hit)
        {
            StartCoroutine(_HitAnim(0.2f));
        }
    }

    IEnumerator _HitAnim(float time)
    {
        animator.speed = oriAnimSpeed * 0.1f;
        yield return new WaitForSeconds(time);
        animator.speed = oriAnimSpeed;
    }
}
