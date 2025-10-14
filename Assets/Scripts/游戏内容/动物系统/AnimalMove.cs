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

    private Vector3 targetPos; // 目标位置
    private bool canMove = true; // 能否移动
    
    private void Awake()
    {
        areaL = GameObject.FindGameObjectWithTag("AreaL").transform;
        areaR = GameObject.FindGameObjectWithTag("AreaR").transform;
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
        transform.position = Vector3.MoveTowards(
                                transform.position,
                                targetPos,
                                speed * Time.deltaTime);

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