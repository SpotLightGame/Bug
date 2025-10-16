using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject animalPrefab;
    public Transform spawnPoint;
    public float price;

    [Header("动物父物体")]
    public Transform animalsParent;
    private ResourcesManager resMgr;

    void Start()
    {
        resMgr = FindObjectOfType<ResourcesManager>();
        spawnPoint = GameObject.FindGameObjectWithTag("Place").transform;
    }


    public void OnClickBuy()
    {
        ConfirmationWindow.I.Show($"是否购买？",
                                  onConfirm: () => ReallyBuy());
    }
    
    void ReallyBuy()
    {
        if (resMgr.gold < price)
        {
            ConfirmationWindow.I.Show("金币不足！");
            return;
        }

        resMgr.CostGold(price);
        GameObject animal = Instantiate(animalPrefab, spawnPoint.position, Quaternion.identity);
        animal.transform.SetParent(animalsParent, worldPositionStays: true);
    }
}
