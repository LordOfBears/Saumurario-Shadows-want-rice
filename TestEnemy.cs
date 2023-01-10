using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : Entity
{
    [SerializeField] private bool move = false;
    [SerializeField] private float speed = 2f;

    private Vector3 dir;
    private SpriteRenderer sprite;

    private void Start()
    {
        lives = 3;
        dir = transform.right;
        rbe = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject)
        {
            Hero.Instance.GetDamage();
            if (Hero.Instance.transform.position.x - transform.position.x > 0)
                Hero.Instance.GetOut(6f, true);
            else
                Hero.Instance.GetOut(6f, false);
            lives -= 1;
            Debug.Log("TestEnemys lives: " + lives);
        }
        if (lives < 1)
            Die();
    }

    private void Update()
    {
        if (move)
            Move();
    }

    private void Move()
    {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + transform.up * 0.1f + transform.right * dir.x * 0.7f, 0.1f);
        if (colliders.Length > 0)
        {
            dir *= -1f;
        }
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
    }

}
