using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiController : MonoBehaviour
{
    Character character;

    bool _attackLockL = false;
    bool _attackLockR = false;
    bool _jumpLock = false;

    // Use this for initialization
    void Start()
    {
        character = GetComponent<Character>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!character.IsAlive) { return; }
        character.OnForward();

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) && !_attackLockR)
        {
            character.OnAttackR();
            StartCoroutine(_AttackLock('R', 0.75f));
        }
        else if (Input.GetMouseButtonDown(1) && !_attackLockL)
        {
            character.OnAttackL();
            StartCoroutine(_AttackLock('L', 0.75f));
        }

        if (Input.GetButtonDown("Jump") && !_jumpLock)
        {
            character.OnJump();
            StartCoroutine(_JumpLock(2));
        }
#endif
    }

    public void OnClick_Jump()
    {
        if (_jumpLock || !character.IsAlive) { return; }
        character.OnJump();
        StartCoroutine(_JumpLock(1.5f));
    }

    public void Onclick_AttackL()
    {
        if (_attackLockL || !character.IsAlive) { return; }
        character.OnAttackL();
        StartCoroutine(_AttackLock('L', 0.75f));
    }

    public void OnClick_AttackR()
    {
        if (_attackLockR || !character.IsAlive) { return; }
        character.OnAttackR();
        StartCoroutine(_AttackLock('R', 0.75f));
    }

    IEnumerator _AttackLock(char hand, float waitTime = 1)
    {
        if (hand == 'L') { _attackLockL = true; }
        if (hand == 'R') { _attackLockR = true; }
        yield return new WaitForSeconds(waitTime);
        if (hand == 'L') { _attackLockL = false; }
        if (hand == 'R') { _attackLockR = false; }
    }

    IEnumerator _JumpLock(float waitTime = 1)
    {
        _jumpLock = true;
        yield return new WaitForSeconds(waitTime);
        _jumpLock = false;
    }
}
