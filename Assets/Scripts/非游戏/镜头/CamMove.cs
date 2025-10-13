using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour
{
    [Header("可移动范围")]
    public float left   = -10;
    public float right  =  10;
    public float top    =  6;
    public float bottom = -6;

    [Header("阻尼")]
    public float moveLerp = 12f;

    Camera cam;
    Vector3 targetPos;      // 目标位置（受限制后）
    Vector2 lastTouch;      // 上一帧触摸点（屏幕像素）

    void Awake() => cam = GetComponent<Camera>();

    void Start() => targetPos = transform.position;   // 初始位置

    void Update()
    {
        // 1. 取得当前触摸（单指或双指都用第一个）或鼠标
        Vector2 curPos = Vector2.zero;
        if (Input.touchCount > 0) curPos = Input.GetTouch(0).position;
        else if (Input.GetMouseButton(0)) curPos = Input.mousePosition;
        else return;          // 没有拖动就跳过

        // 2. 只在“刚按下”时记录 lastTouch，避免抖动
        if (Input.GetMouseButtonDown(0) ||
            (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            lastTouch = curPos;
            return;
        }

        // 3. 计算屏幕偏移 → 世界偏移
        Vector2 screenDelta = curPos - lastTouch;
        float aspect = cam.pixelWidth / (float)cam.pixelHeight;
        float worldDeltaX = screenDelta.x * cam.orthographicSize * 2f / cam.pixelHeight;
        float worldDeltaY = screenDelta.y * cam.orthographicSize * 2f / cam.pixelHeight;

        // 4. 计算“想要”的新位置（还未限制）
        Vector3 wish = targetPos;
        wish.x -= worldDeltaX;   // 注意减号：手指向右 → 场景向左
        wish.y -= worldDeltaY;

        // 5. 用 Mathf.Clamp 限制
        wish.x = Mathf.Clamp(wish.x, left, right);
        wish.y = Mathf.Clamp(wish.y, bottom, top);

        targetPos = wish;
        lastTouch = curPos;      // 更新上一帧

        // 6. 插值真正移动摄像机
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            moveLerp * Time.deltaTime);
    }
}
