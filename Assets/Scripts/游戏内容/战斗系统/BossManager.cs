using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    [Header("数据记录")]
    public BattleInfoRecord_SO battleInfoRecord;
    [Header("Boss信息")]
    [SerializeField] private float health;
    [SerializeField] private float speed;
    [SerializeField] private float attack;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<SpriteRenderer>().sprite = BattlePre.Instance.lastBugSprite;
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            Debug.Log("Boss死亡");
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Debug.Log("Boss被子弹击中");
            Destroy(collision.gameObject);
            TakeDamage(10);
        }
    }

    public void TakeDamage(float damage)
    {
        this.health -= damage;
        GetComponent<BossStateCtrl>()?.OnHit(damage); // 打断冲锋
    }
    

}
