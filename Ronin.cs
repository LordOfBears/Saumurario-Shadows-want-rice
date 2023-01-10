using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ronin : Entity
{
    [SerializeField] private bool move = false;
    [SerializeField] private float speed = 1f;

    [SerializeField] private bool agressive = false;
    [SerializeField] private Transform player;
    private Vector3 pos;
    public bool roninIsAttacking = false;
    public bool roninIsRecharged = true;
    [SerializeField] private Transform attackPosRight;
    [SerializeField] private Transform attackPosLeft;
    [SerializeField] private Transform attackPos;
    [SerializeField] private float attackForce = 6f;
    [SerializeField] private int damage = 1;
    public float roninAttackRange;
    public LayerMask hero;
    public bool roninAttackInRight = true;

    private Vector3 dir;
    private SpriteRenderer sprite;
    private Animator anim;

    private bool isGrounded = false;
    [SerializeField] private Transform groundCheck;

    private States_Ronin State
    {
        get { return (States_Ronin)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    void Start()
    {
        if (!player)
            player = FindObjectOfType<Hero>().transform;
        lives = 7;
        roninIsRecharged = true;
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
                State = States_Ronin.Stand;
            else
                State = States_Ronin.Jump;
            if ((agressive) && ((pos.x < transform.position.x) && (pos.x - transform.position.x > -5)) || ((pos.x > transform.position.x) && (pos.x - transform.position.x < 5)))
                Agression();
            else
                if (move)
                Move();
        }
        else
        {
            State = States_Ronin.Death;
        }
    }
    private void Move()
    {
        State = States_Ronin.Run;
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
        if ((pos.x - transform.position.x > 1.5) || (pos.x - transform.position.x < -1.5))
        {
            State = States_Ronin.Run;
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
        State = States_Ronin.Attack;
        roninIsAttacking = true;
        roninIsRecharged = false;

        StartCoroutine(AttackAnimation());
        StartCoroutine(AttackCoolDown());
    }
    private void OnAttackRonin()
    {
        if (sprite.flipX)
        {
            attackPos = attackPosLeft;
            roninAttackInRight = false;
        }
        else
        {
            attackPos = attackPosRight;
            roninAttackInRight = true;
        }
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPos.position, roninAttackRange, hero);
        Debug.Log("colliders.Length " + colliders.Length);
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Hero>().GetDamage(damage);
            colliders[i].GetComponent<Hero>().GetOut(attackForce, roninAttackInRight);
            Debug.Log("Ronin attack");
        }
    }
    public override void GetDamage()
    {
        lives--;
        Debug.Log("Ronin get damage, ronin's lives: " + lives);
    }
    private IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(0.55f);
        roninIsAttacking = false;
    }
    private IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(1.1f);
        roninIsRecharged = true;
    }
}

public enum States_Ronin
{
    Stand,
    Run,
    Attack,
    Jump,
    Death
}
