using System.Collections;
using UnityEngine;

public class BossStateCtrl : MonoBehaviour
{
    [Header("范围（两个空物体当对角）")]
    public Transform rangeUp_left;      // 左下角
    public Transform rangeDown_right;   // 右上角
    private Vector2 min;   // 左下
    private Vector2 max;   // 右上

    [Header("目标")]
    public GameObject heart1;
    public GameObject heart2;
    public GameObject heart3;
    public GameObject heart4;
    private GameObject targetHeart;

    [Header("移动/冲撞")]
    public float moveSpeed   = 3f;        // 巡逻速度
    public float chargeSpeed = 12f;       // 冲撞速度
    public float chargeRange = 6f;        // 开始后退→冲撞的判定半径
    public float chargeDamage = 20f;      // 每次冲撞对围栏伤害

    [Header("定时")]
    public float chargeInterval = 4f;     // 多久必冲一次

    private enum State { Idle, ChargeBack, Charging, Hit }
    private State state = State.Idle;
    private Vector2 wanderTarget;         // 当前巡逻点
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private BossManager boss;
    private Animator animator;
    private bool hasHitThisCharge = false;

    void Awake()
    {
        rb  = GetComponent<Rigidbody2D>();
        sr  = GetComponent<SpriteRenderer>();
        boss = GetComponent<BossManager>();
        animator = GetComponent<Animator>();

        CalculateArena();
        SetNewWanderPoint();

        StartCoroutine(ChargeTimer());
    }

    void Update() 
    {
        RunState();
        
        
    }

    /*----------------------------------------------------------*/
    void RunState()
    {
        switch (state)
        {
            case State.Idle:        UpdateIdle();  break;
            case State.ChargeBack:  UpdateBack();  break;
            case State.Charging:    UpdateCharge();break;
            case State.Hit:                           break; // 被子弹打断硬直
        }
    }

    /*---------------------  Idle  -----------------------------*/
    void UpdateIdle()
    {
        animator.SetBool("isMoving",true);
        // 到达目标就再随机一个
        if (Vector2.Distance(transform.position, wanderTarget) < 0.3f)
            SetNewWanderPoint();

        Vector2 dir = (wanderTarget - (Vector2)transform.position).normalized;
        rb.velocity = dir * moveSpeed;

    }

    void UpdateBack()
    {

    }

    void UpdateCharge()
    {

    }
    
    void SetNewWanderPoint()
    {
        float x = Random.Range(min.x, max.x);
        float y = Random.Range(min.y, max.y);
        wanderTarget = new Vector2(x, y);
    }

    /*--------------------  Charge  -----------------------------*/
    IEnumerator ChargeTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(chargeInterval);
            if (state == State.Idle)         // 不在 CD 才冲
                StartCharge();
        }
    }

    public void StartCharge()
    {
        StopAllCoroutines();               // 中断巡逻计时
        StartCoroutine(ChargeSequence());
    }

    IEnumerator ChargeSequence()
    {
        hasHitThisCharge = false;
        state = State.ChargeBack;

        // 1. 随机选一颗水晶（直接用 Transform 当终点）
        GameObject[] hearts = { heart1, heart2, heart3, heart4 };
        targetHeart = hearts[Random.Range(0, hearts.Length)];
        Vector2 endPos = targetHeart.transform.position;

        // 2. 方向
        Vector2 startPos = transform.position;
        Vector2 dirToHeart = (endPos - startPos).normalized;

        // 3. 后退蓄力
        rb.velocity = -dirToHeart * moveSpeed * 0.5f;
        yield return new WaitForSeconds(0.5f);

        // 4. 正式冲撞
        state = State.Charging;
        rb.velocity = dirToHeart * chargeSpeed;
        sr.color = Color.red;

        // 5. 等待触发器通知（碰到水晶就结束）
        yield return new WaitUntil(() => hasHitThisCharge);

        // 6. 扣血（触发器里已经写了，这里可再加一次保险）
        targetHeart.GetComponent<HeartManager>()?.TakeDamage(chargeDamage);

        sr.color = Color.white;
        state = State.Idle;
        rb.velocity = Vector2.zero;
        StartCoroutine(ChargeTimer());
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (state == State.Charging && other.gameObject.tag == "Heart" && !hasHitThisCharge)
        {
            hasHitThisCharge = true;
            //other.gameObject.GetComponent<HeartManager>()?.TakeDamage(chargeDamage);
            // 可选：立刻结束冲锋
            ResumeIdle();
        }
    }

    void ResumeIdle()
    {
        sr.color = Color.white;
        state = State.Idle;
        StartCoroutine(ChargeTimer());
    }

    #region bug效果
    private void BugEffect()
    {
        
    }
    #endregion

    void CalculateArena()
    {
        // 不再用 Collider，直接拿 Transform 当角点
        min = new Vector2(rangeUp_left.position.x,     rangeUp_left.position.y);
        max = new Vector2(rangeDown_right.position.x,  rangeDown_right.position.y);
    }
}