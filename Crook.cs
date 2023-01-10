using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crook : Entity
{
    [SerializeField] private bool move = false;
    [SerializeField] private float speed = 1f;

    [SerializeField] private bool agressive = false;
    [SerializeField] private Transform player;
    private Vector3 pos;
    public bool crookIsAttacking = false;
    public bool crookIsRecharged = true;
    [SerializeField] private Transform attackPosRight;
    [SerializeField] private Transform attackPosLeft;
    [SerializeField] private Transform attackPos;
    [SerializeField] private float attackForce = 6f;
    [SerializeField] private int damage = 1;
    public float crookAttackRange;
    public LayerMask hero;
    public bool crookAttackInRight = true;

    private Vector3 dir;
    private SpriteRenderer sprite;
    private Animator anim;

    private bool isGrounded = false;
    [SerializeField] private Transform groundCheck;

    private States_Crook State
    {
        get { return (States_Crook)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    void Start()
    {
        if (!player)
            player = FindObjectOfType<Hero>().transform;
        lives = 3;
        crookIsRecharged = true;
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
                State = States_Crook.Stand;
            else
                State = States_Crook.Jump;
            if ((agressive) && ((pos.x < transform.position.x) && (pos.x - transform.position.x > -5)) || ((pos.x > transform.position.x) && (pos.x - transform.position.x < 5)))
                Agression();
            else
                if (move)
                    Move();
        }
        else
        {
            State = States_Crook.Death;
        }
    }
    private void Move()
    {
        State = States_Crook.Run;
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
            State = States_Crook.Run;
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
        State = States_Crook.Attack;
        crookIsAttacking = true;
        crookIsRecharged = false;

        StartCoroutine(AttackAnimation());
        StartCoroutine(AttackCoolDown());
    }
    private void OnAttackCrook()
    {
        if (sprite.flipX)
        {
            attackPos = attackPosLeft;
            crookAttackInRight = false;
        }
        else
        {
            attackPos = attackPosRight;
            crookAttackInRight = true;
        }
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPos.position, crookAttackRange, hero);
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Hero>().GetDamage(damage);
            colliders[i].GetComponent<Hero>().GetOut(attackForce, crookAttackInRight);
            Debug.Log("Crook attack");
        }
    }
    public override void GetDamage()
    {
        lives--;
        Debug.Log("Crook get damage, crook's lives: " + lives);
    }
    private IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(0.4f);
        crookIsAttacking = false;
    }
    private IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(0.8f);
        crookIsRecharged = true;
    }
}

public enum States_Crook
{
    Stand,
    Run,
    Attack,
    Jump,
    Death
}