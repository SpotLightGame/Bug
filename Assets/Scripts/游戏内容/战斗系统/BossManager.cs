using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BossManager : MonoBehaviour
{
    [Header("回归")]
    public SceneField sceneTo;
    [Header("数据记录")]
    public BattleInfoRecord_SO battleInfoRecord;
    [Header("Boss信息")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float speed;
    [SerializeField] private float attack;
    [Header("UI")]
    [SerializeField] private GameObject health_UI;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<SpriteRenderer>().sprite = BattlePre.Instance.lastBugSprite;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        UIUpdate();
        if (currentHealth <= 0)
        {
            Debug.Log("Boss死亡");
            StartCoroutine(LoadScene());


        }
        
    }
    


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Debug.Log("Boss被子弹击中");
            Destroy(collision.gameObject);
            TakeDamage(10);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        GetComponent<BossStateCtrl>()?.OnHit(damage); // 打断冲锋
    }


    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneTo.SceneName);
        Destroy(gameObject);
    }
    
    public void UIUpdate()
    {
        health_UI.GetComponent<TMP_Text>().text = "Bug维修进度:\n" + (maxHealth - currentHealth).ToString() + "/" + maxHealth.ToString();
    }
}
