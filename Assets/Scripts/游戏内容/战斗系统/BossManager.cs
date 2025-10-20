using UnityEngine;
using TMPro;

public class BossManager : MonoBehaviour
{
    [Header("Boss动画")]
    public RuntimeAnimatorController[] animatorControllers;
    private Animator animator;
    
    [Header("数据记录")]
    public BattleInfoRecord_SO battleInfoRecord;
    [Header("Boss信息")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float speed;
    [SerializeField] private float attack;
    private BossStateCtrl bossStateCtrl;
    [Header("UI")]
    [SerializeField] private GameObject health_UI;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        this.GetComponent<SpriteRenderer>().sprite = BattlePre.Instance.lastBugSprite;
        AnimatorSelected();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        UIUpdate();
        if (currentHealth <= 0)
        {
            Debug.Log("Boss死亡");
            FindObjectOfType<BattleManager>().OnBossDead();
            gameObject.SetActive(false);
            AnimalInfoRecord.Instance.state = 1;
            AnimalInfoRecord.Instance.NotifyBattleFinish();
            return;

        }
        if(bossStateCtrl.heart1.GetComponent<HeartManager>().isBreak 
            && bossStateCtrl.heart2.GetComponent<HeartManager>().isBreak 
            && bossStateCtrl.heart3.GetComponent<HeartManager>().isBreak 
            && bossStateCtrl.heart4.GetComponent<HeartManager>().isBreak)
        {
            Debug.Log("战斗失败");
            AnimalInfoRecord.Instance.state = 2;
        }
        
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        //GetComponent<BossStateCtrl>()?.OnHit(damage); // 打断冲锋
    }


    // IEnumerator LoadScene()
    // {
    //     yield return new WaitForSeconds(1f);
    //     SceneManager.LoadScene(sceneTo.SceneName);
    //     Destroy(gameObject);
    // }

    public void UIUpdate()
    {
        health_UI.GetComponent<TMP_Text>().text = "Bug维修进度:\n" + (maxHealth - currentHealth).ToString() + "/" + maxHealth.ToString();
    }
    
    private void AnimatorSelected()
    {
        switch (BattlePre.Instance.lastSelectedType)
        {
            case AnimalType.cow:
                animator.runtimeAnimatorController = animatorControllers[0];
                break;
            case AnimalType.sheep:
                animator.runtimeAnimatorController = animatorControllers[1];
                break;
            case AnimalType.horse:
                animator.runtimeAnimatorController = animatorControllers[2];
                break;
            default:
                break;
                
        }
    }
}
