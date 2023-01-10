using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    protected int lives;
    public Rigidbody2D rbe;
    public virtual void GetDamage()
    {
        lives--;
        Debug.Log("Entity get damage");
        if (lives < 1)
            Die();
    }

    public virtual void Die()
    {
        Destroy(this.gameObject);
    }

    public virtual void GetOut(float force, bool inRight)
    {
        if (inRight)
            rbe.velocity = Vector2.right * force;
        else
            rbe.velocity = Vector2.left * force;
    }
}
