using UnityEngine;

/// <summary>
/// 全局点击管理：只在手指/鼠标抬起时检测一次 2D 物体
/// </summary>
public class ClickManager : MonoBehaviour
{
    public static ClickManager Instance { get; private set; }

    private Camera mainCam;

    private void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        mainCam = Camera.main;
    }

    private void Update()
    {
        bool mouseUp = Input.GetMouseButtonUp(0);
        bool touchEnded = Input.touchCount > 0 &&
                          (Input.GetTouch(0).phase == TouchPhase.Ended ||
                           Input.GetTouch(0).phase == TouchPhase.Canceled);

        if (mouseUp || touchEnded)
        {
            Vector2 screenPos = Input.touchCount > 0 ?
                                (Vector2)Input.GetTouch(0).position :
                                (Vector2)Input.mousePosition;

            Vector2 worldPos = mainCam.ScreenToWorldPoint(screenPos);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("AnimalProduct"))
            {
                hit.collider.GetComponent<ProductDrop>()?.PickUp();
            }
        }
    }
}