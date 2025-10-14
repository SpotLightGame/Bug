using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [System.Serializable]
    public class AnimalItem
    {
        public string   itemName;      // 商品名
        public Sprite   icon;          // 按钮上显示的图片
        public int      price;         // 售价
        public GameObject animalPrefab;// 买到后生成的动物
    }

    [Header("商店配置")]
    public List<AnimalItem> shopItems = new List<AnimalItem>();

    [Header("UI 引用")]
    public Transform   contentRoot;      // 按钮生成到这里（通常是个 ScrollRect 的 Content）
    public Button      itemButtonTemplate; // 可以留一个隐藏的模板按钮，也可以运行时动态 new
    public TMP_Text goldDisplay;        // 面板顶部显示当前金币

    [Header("生成位置")]
    public Transform   spawnPoint;       // 买到后动物刷新的世界坐标

    private ResourcesManager resMgr;

    void Start()
    {
        resMgr = FindObjectOfType<ResourcesManager>();
        if (resMgr == null)
            Debug.LogError("场景里找不到 ResourcesManager！");

        RefreshUI();
        SetupShop();
    }

    void SetupShop()
    {
        // 清空旧按钮
        foreach (Transform t in contentRoot) Destroy(t.gameObject);

        foreach (var item in shopItems)
        {
            Button btn = Instantiate(itemButtonTemplate, contentRoot);
            btn.gameObject.SetActive(true);

            // 图标
            btn.GetComponentInChildren<Image>().sprite = item.icon;
            // 名字+价格
            btn.GetComponentInChildren<TMP_Text>().text =
                $"{item.itemName}\n<size=22><color=yellow>{item.price} G</color></size>";

            // 钱不够时置灰
            bool affordable = resMgr.gold >= item.price;
            btn.interactable = affordable;
            if (!affordable)
                btn.GetComponentInChildren<Text>().text += "\n<color=red>金币不足</color>";

            // 点击事件
            var captured = item; // 闭包陷阱
            btn.onClick.AddListener(() => OnClickBuy(captured));
        }
    }

    void OnClickBuy(AnimalItem item)
    {
        // 二次确认
        ConfirmationWindow.I.Show($"是否花费 <color=yellow>{item.price}</color> 金币购买 {item.itemName}?",
                                  onConfirm: () => ReallyBuy(item));
    }

    void ReallyBuy(AnimalItem item)
    {
        if (resMgr.gold < item.price)
        {
            ConfirmationWindow.I.Show("金币不足！");
            return;
        }

        resMgr.CostGold(item.price);
        Instantiate(item.animalPrefab, spawnPoint.position, Quaternion.identity);

        RefreshUI(); // 刷新金币显示与按钮状态
    }

    void RefreshUI()
    {
        if (goldDisplay) goldDisplay.text = $"金币: {resMgr.gold}";
    }
}
