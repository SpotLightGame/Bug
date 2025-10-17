using System.Collections;
using UnityEngine;

public class BossStateCtrl : MonoBehaviour
{
    [Header("范围")]
    public GameObject rangeUp;
    public GameObject rangeDown;
    public GameObject rangeLeft;
    public GameObject rangeRight;

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
    private Bounds arena;

    void Awake()
    {
        rb  = GetComponent<Rigidbody2D>();
        sr  = GetComponent<SpriteRenderer>();
        boss = GetComponent<BossManager>();
        CalculateArena();
        StartCoroutine(ChargeTimer());    // 定时冲撞
    }

    void Update() => RunState();

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
        float x = Random.Range(arena.min.x, arena.max.x);
        float y = Random.Range(arena.min.y, arena.max.y);
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
        state = State.ChargeBack;
        Vector2 fenceCenter = Vector2.zero;
        Vector2 dirToFence = (fenceCenter - (Vector2)transform.position).normalized;

        // 1. 后退 0.5 秒（蓄力）
        rb.velocity = -dirToFence * moveSpeed * 0.5f;
        yield return new WaitForSeconds(0.5f);

        // 2. 正式冲撞
        state = State.Charging;
        rb.velocity = dirToFence * chargeSpeed;
        sr.color = Color.red;                       // 视觉提示

        // 3. 等待撞到东西或超出范围
        while (Vector2.Distance(transform.position, fenceCenter) > 0.5f)
            yield return null;

        // 4. 造成伤害并结束
        Collider2D fence = Physics2D.OverlapCircle(transform.position, 0.5f, 1 << LayerMask.NameToLayer("Fence"));
        if (fence) fence.GetComponent<FenceManager>()?.TakeDamage(chargeDamage);

        sr.color = Color.white;
        state = State.Idle;
        rb.velocity = Vector2.zero;
        StartCoroutine(ChargeTimer()); // 重启定时器
    }

    /*--------------------  被子弹打断  -------------------------*/
    public void OnHit(float damage)        // 由 BossManager 调用
    {
        if (state == State.Charging)      // 仅冲撞期可打断
        {
            StopAllCoroutines();
            state = State.Hit;
            sr.color = Color.yellow;
            rb.velocity = Vector2.zero;
            Invoke(nameof(ResumeIdle), 0.3f); // 硬直 0.3 秒
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

    //根据四段围栏计算矩形边界
    void CalculateArena()
    {
        // 上下左右 4 条碰撞盒
        Bounds up = rangeUp.GetComponent<Collider2D>().bounds;
        Bounds down = rangeDown.GetComponent<Collider2D>().bounds;
        Bounds left = rangeLeft.GetComponent<Collider2D>().bounds;
        Bounds right = rangeRight.GetComponent<Collider2D>().bounds;

        float minX = left.center.x + left.extents.x;
        float maxX = right.center.x - right.extents.x;
        float minY = down.center.y + down.extents.y;
        float maxY = up.center.y - up.extents.y;

        arena = new Bounds();
        arena.SetMinMax(new Vector3(minX, minY, 0),
                        new Vector3(maxX, maxY, 0));
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Fence"))
        {
            // 如果正在冲撞，直接结束冲撞；否则重新给巡逻点
            if (state == State.Charging)
                ResumeIdle();
            else
                SetNewWanderPoint();   // 立刻换个内部点
        }
    }
}