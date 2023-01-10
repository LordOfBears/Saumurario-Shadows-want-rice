using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devil : Entity
{
    [SerializeField] private bool move = false;
    [SerializeField] private float speed = 1f;

    [SerializeField] private bool agressive = false;
    [SerializeField] private Transform player;
    private Vector3 pos;
    public bool devilIsAttacking = false;
    public bool devilIsRecharged = true;
    [SerializeField] private Transform attackPos;
    [SerializeField] private float attackForce = 6f;
    [SerializeField] private int damage = 1;
    public float devilAttackRange;
    public LayerMask hero;
    public bool devilAttackInRight = true;

    private Vector3 dir;
    private SpriteRenderer sprite;
    private Animator anim;

    private bool isGrounded = false;
    [SerializeField] private Transform groundCheck;

    private States_Devil State
    {
        get { return (States_Devil)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    void Start()
    {
        if (!player)
            player = FindObjectOfType<Hero>().transform;
        lives = 5;
        devilIsRecharged = true;
        dir = transform.right;
        rbe = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }
    private void FixedUpdate()
    {
        CheckGround();
    }
    void Update()
    {
        if (lives > 0)
        {
            pos = player.position;
            if (isGrounded)
                State = States_Devil.Stand;
            if ((agressive) && ((pos.x < transform.position.x) && (pos.x - transform.position.x > -5)) || ((pos.x > transform.position.x) && (pos.x - transform.position.x < 5)))
                Agression();
            else
                if (move)
                Move();
        }
        else
        {
            State = States_Devil.Death;
        }
    }
    private void Move()
    {
        State = States_Devil.Run;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + transform.up * 0.1f + transform.right * dir.x * 0.7f, 0.1f);
        if (colliders.Length > 0)
        {
            dir *= -1f;
            sprite.flipX = dir.x < 0.0f;
        }
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
    }
    private void Agression()
    {
        if ((pos.x - transform.position.x > 1) || (pos.x - transform.position.x < -1))
        {
            State = States_Devil.Run;
            sprite.flipX = (pos.x - transform.position.x < 0);
            transform.position = Vector3.Lerp(transform.position, pos, speed * Time.deltaTime);
        }
        else
            Attack();
    }
    private void CheckGround()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(groundCheck.position, 0.1f);
        isGrounded = collider.Length > 1;
    }
    private void Attack()
    {
        State = States_Devil.Attack;
        devilIsAttacking = true;
        devilIsRecharged = false;

        StartCoroutine(AttackAnimation());
        StartCoroutine(AttackCoolDown());
    }
    private void OnAttackDevil()
    {
        if (sprite.flipX)
        {
            devilAttackInRight = false;
        }
        else
        {
            devilAttackInRight = true;
        }
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPos.position, devilAttackRange, hero);
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Hero>().GetDamage(damage);
            colliders[i].GetComponent<Hero>().GetOut(attackForce, devilAttackInRight);
            Debug.Log("Devil attack");
        }
    }
    private void OnDeathDevil()
    {
        if (sprite.flipX)
        {
            devilAttackInRight = false;
        }
        else
        {
            devilAttackInRight = true;
        }
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPos.position, devilAttackRange * 2, hero);
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Hero>().GetDamage(damage);
            colliders[i].GetComponent<Hero>().GetOut(attackForce, devilAttackInRight);
            Debug.Log("Devil attack");
        }
    }
    public override void GetDamage()
    {
        lives--;
        Debug.Log("Devil get damage, devil's lives: " + lives);
    }
    private IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(0.3f);
        devilIsAttacking = false;
    }
    private IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(0.6f);
        devilIsRecharged = true;
    }
}

public enum States_Devil
{
    Stand,
    Run,
    Attack,
    Jump,
    Death
}