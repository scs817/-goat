using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Movement2d movement2D;
    private Transform target;
    private int damage;

    // Start is called before the first frame update
    public void Setup(Transform target, int damage)
    {
        movement2D = GetComponent<Movement2d>();
        this.target = target;
        this.damage = damage;
    }

    // Update is called once per frame
    public void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            movement2D.MoveTo(direction);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;
        if (collision.transform != target) return;

        collision.GetComponent<EnemyHP>().TakeDamage(damage);
        Destroy(gameObject);
    }



}
