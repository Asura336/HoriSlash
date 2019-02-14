using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Animator animator;
    public ParticleSystem deadFlash;
    public ParticleSystem swordL;
    public ParticleSystem swordR;
    public AudioSource audioSource;
    public AudioClip attackSound;
    public AudioClip faintSound;
    Rigidbody rigid;
    bool hasHolyLight = false;  // PRAISE THE HOLY LIGHT!
    public float holyLightTime = 0.33f;  // HOLY LIGHT TIME!

    float _moveSpeed;
    public float maxSpeed = 2;
    public float deltaSpeed = 0.5f;
    public float jumpHeight = 2.5f;

    bool _isAlive = true;
    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            if (GameMode.Instance != null)
            { GameMode.Instance.IsPlayerAlive = value; }
            if (!value)
            {
                if (GameMode.Instance != null)
                { GameMode.Instance.OnPlayerDead(); }
                _OnDead();
            }
        }
    }

    private void Awake()
    {
        if (GameMode.Instance != null)
        {
            GameMode.Instance.PlayerTrans = transform;
            GameMode.Instance.Player = this;
        }
    }

    private void Start()
    {
        if (deadFlash != null)
        {
            deadFlash.Stop();
        }
        if (swordL != null)
        {
            swordL.Stop();
            swordL.startColor = GameMode.Instance.typeOne;
        }
        if (swordR != null)
        {
            swordR.Stop();
            swordR.startColor = GameMode.Instance.typeTwo;
        }
        rigid = GetComponent<Rigidbody>();

        IsAlive = true;
    }

    public void OnForward()
    {
        if (!IsAlive) { return; }
        transform.position += new Vector3(0, 0, _moveSpeed * Time.deltaTime);
        _moveSpeed = _moveSpeed < maxSpeed ? _moveSpeed + deltaSpeed * Time.deltaTime : maxSpeed;
    }

    public void OnAttackL()
    {
        if (!IsAlive) { return; }
        animator.SetTrigger("Attack_L");
        swordL.Play();
        audioSource.PlayOneShot(attackSound);
        StartCoroutine(_HolyLight(holyLightTime));
    }

    public void OnAttackR()
    {
        if (!IsAlive) { return; }
        animator.SetTrigger("Attack_R");
        swordR.Play();
        audioSource.PlayOneShot(attackSound);
        StartCoroutine(_HolyLight(holyLightTime));
    }
    IEnumerator _HolyLight(float time)
    {
        hasHolyLight = true;
        yield return new WaitForSeconds(time);
        hasHolyLight = false;
    }

    public void OnFaint()
    {
        if (!IsAlive) { return; }
        _moveSpeed = 0;
        animator.SetTrigger("Faint");
        audioSource.PlayOneShot(faintSound);

        if (GameMode.Instance != null) { GameMode.Instance.OnPlayerFaint(); }
    }

    public void OnJump()
    {
        if (!IsAlive) { return; }
        float upSpeed = Mathf.Sqrt(jumpHeight * GameMode.G * 2);
        rigid.velocity = Vector3.up * upSpeed;
        animator.SetTrigger("Faint");
    }

    void _OnDead()
    {
        _moveSpeed = 0;
        deadFlash.Play();
        StartCoroutine(_SetUnactive(0.5f));
    }
    IEnumerator _SetUnactive(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }

    public void OnBeDamage()
    {
        OnFaint();
        if (!hasHolyLight) { IsAlive = false; }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var monster = collision.transform.GetComponent<Monster>();
        if (monster != null && monster.IsAlive) { OnBeDamage(); }
    }
}
