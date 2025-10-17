using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    public float damage = 10f;

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Boss"))
        {
            coll.gameObject.GetComponent<BossManager>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible() => Destroy(gameObject); // 出屏回收
}
