using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HeartManager : MonoBehaviour
{
    public TMP_Text heartText;
    public float maxHp = 100f;
    public float currentHp;
    public bool canClick;
    public bool isBreak;
    public Sprite brokenHeart;

    private void Start()
    {
        canClick = true;
        currentHp = maxHp;
    }

    private void Update()
    {
        if(currentHp >= maxHp) currentHp = maxHp;
        heartText.text = currentHp.ToString() + "/" + maxHp.ToString();

        FixHeart();
    }
    public void TakeDamage(float dmg)
    {
        currentHp -= dmg;
        Debug.Log($"生命水晶血量：{currentHp} / {maxHp}");
        
    }

    public void HeartBreak()
    {
        if (currentHp <= 0)
        {
            Debug.Log("水晶破损！");
            isBreak = true;
            gameObject.GetComponent<SpriteRenderer>().sprite = brokenHeart;
        }
    }

    private void FixHeart()
    {
        if(canClick && (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)))
        {
            Vector2 pointer = Input.mousePosition;
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pointer), Vector2.zero);
            if (hit.collider != null && hit.collider.CompareTag("Heart"))
            {
                if (currentHp >= maxHp) return;
                Debug.Log("修复了生命水晶！");
                currentHp += 5;
            }
            
        }
        

    }
}
