using System.Collections;
using UnityEngine;

public class AnimalMove : MonoBehaviour
{
    [Header("移动区域")]
    public Transform areaL;
    public Transform areaR;

    [Header("移动速度")]
    public float speed = 2f;

    [Header("间隔时间（秒）")]
    public float interval = 15f;
    [Header("动画")]
    private Animator anim;

    private Vector3 targetPos; // 目标位置
    private bool canMove = true; // 能否移动
    
    private void Awake()
    {
        areaL = GameObject.FindGameObjectWithTag("AreaL").transform;
        areaR = GameObject.FindGameObjectWithTag("AreaR").transform;
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        // 先随机一个目标，防止第一帧卡住
        SetNewRandomTarget();
        // 启动计时器
        StartCoroutine(MoveTimer());
    }

    private void Update()
    {
        
        float dist = Vector3.Distance(transform.position, targetPos);

        anim.SetBool("isMoving", dist > 0.01f);

        Vector3 dir = (targetPos - transform.position).normalized;
        if (dir.x != 0)
        {
            float originalScaleX = Mathf.Abs(transform.localScale.x); // 保持原始大小
            float newSign = -Mathf.Sign(dir.x); // 反转方向
            transform.localScale = new Vector3(newSign * originalScaleX, transform.localScale.y, transform.localScale.z);
        }

        transform.position = Vector3.MoveTowards(
                                transform.position,
                                targetPos,
                                speed * Time.deltaTime);

        if (!canMove && dist < 0.01f)
        {
            canMove = true;
        }

        if (!canMove && Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            canMove = true;
        }
    }

    /* 计时器：每15s指定一次新地点，必须等动物到达后才继续倒计时 */
    private IEnumerator MoveTimer()
    {
        while (true)
        {
            float timer = 0f;
            while (timer < interval)
            {
                if (canMove)
                {
                    timer += Time.deltaTime;
                }
                yield return null;
            }

            SetNewRandomTarget();
            canMove = false;
        }
    }

    private void SetNewRandomTarget()
    {
        float x = Random.Range(areaL.position.x, areaR.position.x);
        float y = Random.Range(areaL.position.y, areaR.position.y);
        targetPos = new Vector3(x, y, 0f);
    }
}