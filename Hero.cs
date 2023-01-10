using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Entity
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float attackForce = 3f;
    private int extraJumps = 1;
    private int jumpsCount = 0;

    private bool isGrounded = false;
    [SerializeField] private Transform groundCheck;

    public bool heroIsAttacking = false;
    public bool heroIsRecharged = true;
    [SerializeField] private Transform attack1PosRight;
    [SerializeField] private Transform attack1PosLeft;
    [SerializeField] private Transform attack2PosRight;
    [SerializeField] private Transform attack2PosLeft;
    [SerializeField] private Transform attackPos;
    private int attacksSwitcher = 0;
    public float heroAttackRange;
    public LayerMask enemy;
    public bool heroAttackInRight = true;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;

    public static Hero Instance { get; set; }

    private States_Hero State
    {
        get { return (States_Hero)anim.GetInteger("State"); }
        set { anim.SetInteger("State", (int)value); }
    }

    private void Start()
    {
        lives = 5;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        Instance = this;
        heroIsRecharged = true;
    }
    
    private void FixedUpdate()
    {
        CheckGround();
    }

    private void Update()
    {
        if (isGrounded)
        {
            State = States_Hero.Stand;
            jumpsCount = 0;
        }

        if (Input.GetButton("Horizontal"))
            Run();

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (isGrounded)
                Jump();
            else if (jumpsCount < extraJumps)
                Jump();
        }

        if (Input.GetKeyDown(KeyCode.Space))
            LightAttack();

    }

    private void Run()
    {
        if (isGrounded) 
            State = States_Hero.Run;
        Vector3 dir = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
        sprite.flipX = dir.x < 0.0f;
    }

    private void Jump()
    {
        StartCoroutine(JumpAnimation());
        rb.velocity = Vector2.up * jumpForce;
        jumpsCount++;
        //rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);

    }

    private void LightAttack()
    {
        heroIsAttacking = true;
        heroIsRecharged = false;

        StartCoroutine(AttackAnimation());
        StartCoroutine(AttackCoolDown());

        if (attacksSwitcher == 0)
            State = States_Hero.Attack1;
        else 
            State = States_Hero.Attack2;
        attacksSwitcher = 1 - attacksSwitcher;
        
    }

    private void OnAttackHero()
    {
        if (sprite.flipX)
        {
            if (attacksSwitcher == 1)
                attackPos = attack1PosLeft;
            else
                attackPos = attack2PosLeft;
            heroAttackInRight = false;
        }
        else
        {
            if (attacksSwitcher == 1)
                attackPos = attack1PosRight;
            else
                attackPos = attack2PosRight;
            heroAttackInRight = true;
        }
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPos.position, heroAttackRange, enemy);
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Entity>().GetDamage();
            colliders[i].GetComponent<Entity>().GetOut(attackForce, heroAttackInRight);
            Debug.Log("Hero attack");
        }
    }

    public override void GetOut(float force, bool inRight)
    {
        if (inRight)
        {
            rb.velocity = Vector2.up * force;
            //rb.velocity = Vector2.right * (force / 2);
        }
        else
        {
            rb.velocity = Vector2.up * force;
            //rb.velocity = Vector2.left * (force / 2);
        }
        jumpsCount++;
    }

    /*private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, heroAttackRange);
    }*/

    private void CheckGround()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(groundCheck.position, 0.1f);
        isGrounded = collider.Length > 1;

        if (!isGrounded) State = States_Hero.Jump;
    }

    public void GetDamage(int damage)
    {
        lives -= damage;
        Debug.Log("Hero lives: " + lives);
        /*if (lives < 1)
            Die();*/
    }

    private IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(0.4f);
        heroIsAttacking = false;
    }
    private IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(0.5f);
        heroIsRecharged = true;
    }
    private IEnumerator JumpAnimation()
    {
        State = States_Hero.Stand;
        yield return new WaitForSeconds(0.001f);
        State = States_Hero.Jump;
    }
}

public enum States_Hero
{
    Stand,
    Run,
    Jump,
    Attack1,
    Attack2
}