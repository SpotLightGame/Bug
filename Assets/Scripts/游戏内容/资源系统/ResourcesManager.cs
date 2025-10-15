using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    public float gold;
    public float food;
    public float water;

    public TMP_Text goldText;
    public TMP_Text foodText;
    public TMP_Text waterText;

    public void Update()
    {
        UpdateResources();
    }
    private void UpdateResources()
    {
        goldText.text = "Gold: " + gold.ToString();
    }

    public void AddGold(float amount)
    {
        gold += amount;
    }

    public void CostGold(float amount)
    {
        gold -= amount;
    }
}
