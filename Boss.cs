using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Entity
{
    [SerializeField] private bool move = false;
    [SerializeField] private float speed = 1f;

    [SerializeField] private bool agressive = false;
    [SerializeField] private Transform player;
    private int attackCount = 0;
    private Vector3 pos;
    public bool bossIsAttacking = false;
    public bool bossIsRecharged = true;
    [SerializeField] private Transform attackPosRight12;
    [SerializeField] private Transform attackPosLeft12;
    [SerializeField] private Transform attackPos12;
    [SerializeField] private Transform attackPos3;
    [SerializeField] private float attackForce = 6f;
    [SerializeField] private int damage = 2;
    public float bossAttackRange12;
    public float bossAttackRange3;
    public LayerMask hero;
    public bool bossAttackInRight = true;

    private Vector3 dir;
    private SpriteRenderer sprite;
    private Animator anim;

    private bool isGrounded = false;
    [SerializeField] private Transform groundCheck;

    private States_Boss State
    {
        get { return (States_Boss)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    void Start()
    {
        if (!player)
            player = FindObjectOfType<Hero>().transform;
        lives = 15;
        bossIsRecharged = true;
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
        if (!bossIsAttacking)
        {
            if (lives > 0)
            {
                pos = player.position;
                if (isGrounded)
                    State = States_Boss.Stand;
                if ((agressive) && ((pos.x < transform.position.x) && (pos.x - transform.position.x > -5)) || ((pos.x > transform.position.x) && (pos.x - transform.position.x < 5)))
                    Agression();
                else
                    if (move)
                    Move();
            }
            else
            {
                Die();
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject)
        {
            Hero.Instance.GetDamage(1);
            if (Hero.Instance.transform.position.x - transform.position.x > 0)
                Hero.Instance.GetOut(6f, true);
            else
                Hero.Instance.GetOut(6f, false);
        }
    }
    private void Move()
    {
        State = States_Boss.Run;
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
        if (((pos.x < transform.position.x) && (pos.x - transform.position.x < -3)) || ((pos.x > transform.position.x) && (pos.x - transform.position.x > 3)))
        {
            State = States_Boss.Run;
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
        bossIsAttacking = true;
        bossIsRecharged = false;
        if (attackCount == 0)
        {
            State = States_Boss.Attack1;
            attackCount++;
            
            StartCoroutine(Attack1Animation());
            StartCoroutine(Attack1CoolDown());
        }
        else
        {
            if (attackCount == 1)
            {
                State = States_Boss.Attack2;
                attackCount++;
                
                StartCoroutine(Attack2Animation());
                StartCoroutine(Attack2CoolDown());
            }
            else
            {
                State = States_Boss.Attack3;
                attackCount = 0;
                
                StartCoroutine(Attack3Animation());
                StartCoroutine(Attack3CoolDown());
            }
        }
    }
    private void OnAttack1Boss()
    {
        if (sprite.flipX)
        {
            attackPos12 = attackPosLeft12;
            bossAttackInRight = false;
        }
        else
        {
            attackPos12 = attackPosRight12;
            bossAttackInRight = true;
        }
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPos12.position, bossAttackRange12, hero);
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Hero>().GetDamage(damage);
            colliders[i].GetComponent<Hero>().GetOut(attackForce, bossAttackInRight);
            Debug.Log("Boss attack");
        }
    }
    private void OnAttack2Boss()
    {
        if (sprite.flipX)
        {
            attackPos12 = attackPosLeft12;
            bossAttackInRight = false;
        }
        else
        {
            attackPos12 = attackPosRight12;
            bossAttackInRight = true;
        }
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPos12.position, bossAttackRange12, hero);
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Hero>().GetDamage(damage);
            colliders[i].GetComponent<Hero>().GetOut(attackForce, bossAttackInRight);
            Debug.Log("Boss attack");
        }
    }
    private void OnAttack3Boss()
    {
        if (sprite.flipX)
            bossAttackInRight = false;
        else
            bossAttackInRight = true;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPos3.position, bossAttackRange3, hero);
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Hero>().GetDamage(damage);
            colliders[i].GetComponent<Hero>().GetOut(attackForce, bossAttackInRight);
            Debug.Log("Boss attack");
        }
    }
    public override void GetDamage()
    {
        lives--;
        Debug.Log("Boss get damage, bosse's lives: " + lives);
    }
    private IEnumerator Attack1Animation()
    {
        yield return new WaitForSeconds(1.35f);
        bossIsAttacking = false;
    }
    private IEnumerator Attack1CoolDown()
    {
        yield return new WaitForSeconds(2.7f);
        bossIsRecharged = true;
    }
    private IEnumerator Attack2Animation()
    {
        yield return new WaitForSeconds(1.36f);
        bossIsAttacking = false;
    }
    private IEnumerator Attack2CoolDown()
    {
        yield return new WaitForSeconds(2.72f);
        bossIsRecharged = true;
    }
    private IEnumerator Attack3Animation()
    {
        yield return new WaitForSeconds(1.42f);
        bossIsAttacking = false;
    }
    private IEnumerator Attack3CoolDown()
    {
        yield return new WaitForSeconds(2.84f);
        bossIsRecharged = true;
    }
}

public enum States_Boss
{
    Stand,
    Run,
    Attack1,
    Attack2,
    Attack3
}
