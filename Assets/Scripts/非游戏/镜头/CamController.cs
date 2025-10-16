using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public bool canMove = true;
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
        if (!canMove) return;

        bool mouseDown = Input.GetMouseButtonDown(0);
        bool touchBegan = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;

        if (mouseDown || touchBegan)
        {
            // 1. 先拾取
            Vector2 screenPos = Input.touchCount > 0 ? (Vector2)Input.GetTouch(0).position
                                                    : Input.mousePosition;
            Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit.collider != null && hit.collider.CompareTag("AnimalProduct"))
            {
                hit.collider.GetComponent<ProductDrop>()?.PickUp();
                // 如果拾取成功，就不再进入拖拽逻辑（可自己加标记决定是否继续）
                return;
            }

            // 2. 没捡到，才记录拖拽起点
            lastTouch = screenPos;
            return;
        }

        //移动屏幕
        Vector2 curPos = Vector2.zero;
        if (Input.touchCount > 0) curPos = Input.GetTouch(0).position;
        else if (Input.GetMouseButton(0)) curPos = Input.mousePosition;
        else return;

        Vector2 screenDelta = curPos - lastTouch;
        float worldDeltaX = screenDelta.x * cam.orthographicSize * 2f / cam.pixelHeight;
        float worldDeltaY = screenDelta.y * cam.orthographicSize * 2f / cam.pixelHeight;

        Vector3 wish = targetPos;
        wish.x -= worldDeltaX;
        wish.y -= worldDeltaY;
        wish.x = Mathf.Clamp(wish.x, left, right);
        wish.y = Mathf.Clamp(wish.y, bottom, top);
        targetPos = wish;
        lastTouch = curPos;

        transform.position = Vector3.Lerp(transform.position, targetPos, moveLerp * Time.deltaTime);
    }
}
