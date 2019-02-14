using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可被外界调用的设置项:
///     <see cref="ColorType"/>: 枚举, 决定身躯颜色
///     <see cref="MonsterState"/>: 枚举，决定怪物行为
///     <see cref="OnSetState"/>: 函数, 设置行为后调用
///     <see cref="SetInitSpeed"/>: 函数, 设置移动速率
///     <see cref="IsAlive"/>: 属性, 重设是否存活
///     <see cref="rigid"/>: 字段, 设置物理效果
/// </summary>
public class Monster : MonoBehaviour, IGObjNode
{
    public Animator animator;
    public ParticleSystem deadFlash;
    public AudioSource audioSource;
    public AudioClip deadSound;
    [SerializeField] private MeshRenderer meshRenderer;
    public Material materialTypeOne;
    public Material materialTypeTwo;
    public Rigidbody rigid;
    public SphereCollider trigger;
    private BoxCollider selfcollider;
    private Quaternion initRotation;

    private static readonly int m_IsForward = Animator.StringToHash("IsForward");
    private static readonly int m_Hurt = Animator.StringToHash("Hurt");

    MonsterColor _colorType = MonsterColor.NONE;
    public MonsterColor ColorType
    {
        get
        {
            return _colorType;
        }
        set
        {
            _colorType = value;
            switch (value)
            {
                case MonsterColor.TYPE_ONE:
                    meshRenderer.material = materialTypeOne;
                    deadFlash.startColor = GameMode.Instance.typeOne;
                    break;
                case MonsterColor.TYPE_TWO:
                    meshRenderer.material = materialTypeTwo;
                    deadFlash.startColor = GameMode.Instance.typeTwo;
                    break;
                default:
                    deadFlash.startColor = Color.white;
                    return;
            }
        }
    }

    private bool _posLock = false;  // 被对象池回收时置为真

    private bool _isAlive = true;
    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            if (!value)
            {
                _OnDead();
                if (GameMode.Instance != null)
                { GameMode.Instance.OnMonsterDead(); }
            }
            _isAlive = value;
        }
    }

    public MonsterActState MonsterState { get; set; }
    private bool jumpLock = false;
    [SerializeField] private float jumpHeight = 2.5f;
    [SerializeField] private float triggerRadius = 5;
    private float initMoveSpeed;
    [SerializeField] private float _moveSpeed = 2;
    Vector3 _dir;

    public Vector3 Movement
    {
        get
        {
            if (!IsAlive)
            {
                return Vector3.zero;
            }
            if (animator != null) { animator.SetBool(m_IsForward, _moveSpeed != 0); }
            return _dir * _moveSpeed;
        }
    }

    public GObjPool Master { get; set; }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        selfcollider = GetComponent<BoxCollider>();
    }

    // Use this for initialization
    void Start()
    {
        if (deadFlash) { deadFlash.Stop(); }
        trigger.isTrigger = true;
        trigger.radius = triggerRadius;
        initMoveSpeed = _moveSpeed;
        initRotation = transform.rotation;
        IsAlive = true;
        OnSetState();
    }

    // Update is called once per frame
    void Update()
    {
        if (_posLock)
        {
            transform.position = Master.RelaxPos;
            return;
        }
        transform.position += Movement * Time.deltaTime;

        // 玩家越过怪物头顶时怪物跳起
        RaycastHit hit;
        switch (MonsterState)
        {
            case MonsterActState.FALLBACK:
                if (Physics.Raycast(transform.position, Vector3.up, out hit, 10) &&
                    hit.rigidbody != null && hit.rigidbody.tag == "Player")
                {
                    Jump();
                }
                break;
            case MonsterActState.NEAR:
                if (Physics.Raycast(transform.position, Vector3.up, out hit, 10) &&
                    hit.rigidbody != null && hit.rigidbody.tag == "Player")
                {
                    Jump();
                    MonsterState = MonsterActState.FALLBACK;
                    OnSetState();
                }
                break;
            default:
                break;
        }
    }

    public void OnSetState()
    {
        switch (MonsterState)
        {
            case MonsterActState.NONE:
                _dir = Vector3.zero;
                break;
            case MonsterActState.JUMPABLE:
                _dir = Vector3.back;
                break;
            case MonsterActState.NEAR:
                _dir = Vector3.back;
                break;
            case MonsterActState.FALLBACK:
                _dir = Vector3.forward;
                break;
            default:
                Debug.LogError("Monster state was out of range.");
                break;
        }
        transform.rotation = Quaternion.LookRotation(_dir);
    }

    public void SetInitSpeed(float speed)
    {
        initMoveSpeed = speed;
        _moveSpeed = speed;
    }

    #region 跳跃动作

    public void Jump()
    {
        if (!IsAlive || jumpLock) { return; }
        float upSpeed = Mathf.Sqrt(jumpHeight * GameMode.G * 2);
        rigid.velocity = Vector3.up * upSpeed;
        StartCoroutine(_OnJump());
    }
    IEnumerator _OnJump()
    {
        animator.SetBool(m_IsForward, !animator.GetBool(m_IsForward));
        yield return new WaitForSeconds(0.5f);
        animator.SetBool(m_IsForward, !animator.GetBool(m_IsForward));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsAlive &&
            MonsterState == MonsterActState.JUMPABLE && 
            other.tag == "Player")
        {
            Jump(); 
            StartCoroutine(_JumpableDash(0.75f));
        }
    }
    IEnumerator _JumpableDash(float waitTime = 1)
    {
        yield return new WaitForSeconds(waitTime);
        if (IsAlive)
        {
            MonsterState = MonsterActState.FALLBACK;
            OnSetState();
        }
        yield return new WaitForSeconds(1);
        if (IsAlive)
        {
            _moveSpeed *= 8;
            jumpLock = true;
        }
        yield return new WaitForSeconds(1.5f);
        if (IsAlive)
        {
            _moveSpeed = initMoveSpeed;
            jumpLock = false;
        }
    }

    #endregion

    void _OnDead()
    {
        deadFlash.Play();
        if (Master != null)
        {
            StartCoroutine(_ToDelete(0.75f));
            return;
        }
        Destroy(gameObject, 0.75f);
    }
    IEnumerator _ToDelete(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Master.DeleteCell(gameObject);
    }

    public void OnBeDamage(int flag = 0)
    {
        if (!IsAlive) { return; }
        IsAlive = false;
        int x = 0;
        if (flag == 'L') { x = -1; }
        if (flag == 'R') { x = 1; }
        animator.SetTrigger(m_Hurt);
        audioSource.PlayOneShot(deadSound);
        rigid.velocity = new Vector3(Random.Range(3, 4) * x,
                                     Random.Range(4, 8),
                                     Random.Range(2, 6));
    }

    public void OnNodeBirth()
    {
        IsAlive = true;
        transform.rotation = initRotation;
        _posLock = false;
    }

    public void OnNodeDead()
    {
        transform.rotation = initRotation;
        _posLock = true;
    }
}

public enum MonsterActState
{
    NONE,
    FALLBACK,
    NEAR,
    JUMPABLE
}

public enum MonsterColor
{
    NONE,
    TYPE_ONE,
    TYPE_TWO
}