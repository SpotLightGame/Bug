using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("相机")]
    public GameObject mainCam;
    public GameObject battleCam;
    [Header("Boss 对象")]
    public BossManager boss;
    [Header("战斗组件")]
    public GameObject battleObj;

    [Header("点击伤害")]
    public float damagePerClick = 10f;
    public float clickCooldown = 0.2f; // 连点间隔

    [Header("点击特效")]
    public GameObject hitTextPrefab;

    private bool canClick = true;

    void Awake()
    {
        battleObj.SetActive(false);
        battleCam.SetActive(false);
        // 自动找 Boss
        if (boss == null) boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossManager>();
    }

    void Update()
    {
        // 左键 / 单指点击
        if (canClick && (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)))
        {
            Vector2 pointer = Input.mousePosition;
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pointer), Vector2.zero);
            if (hit.collider != null && hit.collider.CompareTag("Boss"))
            {
                StartCoroutine(HitBoss());
            }
        }
    }

    IEnumerator HitBoss()
    {
        canClick = false;
        StartCoroutine(HitEffect());
        boss.TakeDamage(damagePerClick);   // 你已有的扣血函数
        yield return new WaitForSeconds(clickCooldown);
        canClick = true;
    }

    // Boss 死亡后由 BossManager 回调这里
    public void OnBossDead()
    {
        StartCoroutine(LoadAfter(1f));
    }

    IEnumerator LoadAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        battleObj.SetActive(false);
        SwitchToMainCamera();
    }

    public void SwitchToBattleCamera()
    {
        mainCam.SetActive(false);
        battleCam.SetActive(true);
    }

    public void SwitchToMainCamera()
    {
        battleCam.SetActive(false);
        mainCam.SetActive(true);
    }

    public void StartBattle()
    {
        battleObj.SetActive(true);
        SwitchToBattleCamera();
    }

    IEnumerator HitEffect()
    {
        // 1. 预设颜色池
        Color[] colors = { Color.red, Color.yellow, Color.green, Color.cyan, Color.blue, Color.magenta };
        Color randomColor = colors[Random.Range(0, colors.Length)];
        float targetSize  = Random.Range(0.8f, 1.4f);

        // 2. 生成特效（放在 Boss 头顶，避免被 UI Canvas 干扰）
        GameObject hitText = Instantiate(hitTextPrefab, 
                                        boss.transform.position + Vector3.up * 0.5f, 
                                        Quaternion.Euler(0, 0, Random.Range(-45f, 45f))); // 随机旋转

        SpriteRenderer sr = hitText.GetComponent<SpriteRenderer>();
        sr.color = randomColor;
        sr.transform.localScale = Vector3.zero; // 初始大小 0

        // 3. 0.2 秒内放大到目标大小
        float t = 0f;
        while (t < 0.2f)
        {
            t += Time.deltaTime;
            float scale = Mathf.Lerp(0f, targetSize, t / 0.2f);
            sr.transform.localScale = Vector3.one * scale;
            yield return null;
        }

        // 5. 向下掉落并淡出（再 0.8 秒）
        Vector3 dropStart = sr.transform.position;
        t = 0f;
        while (t < 0.8f)
        {
            t += Time.deltaTime;
            float p = t / 0.8f;

            // 向下掉
            sr.transform.position = dropStart + Vector3.down * (0.8f * p);
            // 淡出
            sr.color = new Color(randomColor.r, randomColor.g, randomColor.b, 1f - p);

            yield return null;
        }

        // 6. 销毁
        Destroy(hitText);
    }
}