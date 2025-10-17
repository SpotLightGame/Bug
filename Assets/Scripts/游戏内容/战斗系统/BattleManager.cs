using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("UI 参考")]
    public RectTransform muzzle;
    public RectTransform crossHair;

    [Header("子弹")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public bool canShoot = true;
    public float shootInterval = 1f;
    private float shootTimer = 0f;


    private Camera cam;

    void Awake() => cam = Camera.main;

    private void Update()
    {
        if (canShoot)
        {
            muzzle.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.green;
        }
        else
        {
            muzzle.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.red;
        }
        ResetShootTimer();
    }

    public void Shoot()
    {
        if (canShoot)
        {
            // 1. UI 坐标 → 屏幕坐标
            Vector2 screenMuzzle = RectTransformUtility.WorldToScreenPoint(null, muzzle.position);
            Vector2 screenTarget = RectTransformUtility.WorldToScreenPoint(null, crossHair.position);

            // 2. 屏幕坐标 → 世界坐标（Z 用 0 即可）
            Vector2 worldMuzzle = cam.ScreenToWorldPoint(new Vector3(screenMuzzle.x, screenMuzzle.y, 0));
            Vector2 worldTarget = cam.ScreenToWorldPoint(new Vector3(screenTarget.x, screenTarget.y, 0));

            // 3. 生成子弹
            GameObject bullet = Instantiate(bulletPrefab, worldMuzzle, Quaternion.identity);

            // 4. 2D 方向 + 速度
            Vector2 dir = (worldTarget - worldMuzzle).normalized;
            bullet.GetComponent<Rigidbody2D>().velocity = dir * bulletSpeed;

            canShoot = false;
        }

    }
    
    public void ResetShootTimer()
    {
        if (!canShoot)
        {
            shootTimer += Time.deltaTime;
            if(shootTimer >= shootInterval)
            {
                canShoot = true;
                shootTimer = 0f;
            }
        }
    }
}