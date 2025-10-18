using UnityEngine;

public class CamController : MonoBehaviour
{
    public bool canMove = true;
    [Header("可移动范围")]
    public float left = -10, right = 10, top = 6, bottom = -6;
    [Header("阻尼")]
    public float moveLerp = 12f;

    Camera cam;
    Vector3 targetPos;
    Vector2 lastTouch;

    void Awake()
    {
        // 强制竖屏
        Screen.autorotateToPortrait          = true;
        Screen.autorotateToPortraitUpsideDown= false;
        Screen.autorotateToLandscapeLeft     = false;
        Screen.autorotateToLandscapeRight    = false;
        Screen.orientation = ScreenOrientation.Portrait;
        
        cam = GetComponent<Camera>();

    }
    void Start() => targetPos = transform.position;

    void Update()
    {
        if (!canMove) return;

        // 只处理单指拖屏
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved ||
            Input.GetMouseButton(0))
        {
            Vector2 delta;
            if (Input.touchCount > 0)
                delta = Input.GetTouch(0).deltaPosition;
            else
                delta = (Vector2)Input.mousePosition - lastTouch;

            float worldDeltaX = delta.x * cam.orthographicSize * 2f / cam.pixelHeight;
            float worldDeltaY = delta.y * cam.orthographicSize * 2f / cam.pixelHeight;

            targetPos -= new Vector3(worldDeltaX, worldDeltaY, 0);
            targetPos.x = Mathf.Clamp(targetPos.x, left, right);
            targetPos.y = Mathf.Clamp(targetPos.y, bottom, top);
        }

        transform.position = Vector3.Lerp(transform.position, targetPos,
                                        1f - Mathf.Exp(-moveLerp * Time.deltaTime));
        lastTouch = Input.mousePosition;
    }
}