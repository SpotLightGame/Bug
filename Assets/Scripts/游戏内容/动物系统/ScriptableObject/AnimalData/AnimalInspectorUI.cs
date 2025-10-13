using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AnimalInspectorUI : MonoBehaviour
{
    public static AnimalInspectorUI I;   // 单例

    [SerializeField] TMP_Text nameTxt, ageTxt, closenessTxt, happyTxt, hungryTxt, thirstyTxt;
    [SerializeField] Image      iconImg;
    [SerializeField] GameObject rootPanel; // 整个面板

    void Awake()
    {
        I = this;
        rootPanel.SetActive(false);
    }

    public void Show(AnimalData data)
    {
        rootPanel.SetActive(true);
        nameTxt.text = data.animalName;
        
        ageTxt.text = $"stay with you for: {data.age} days";
        
        closenessTxt.text = $"Closeness: {data.closeness:F1}";

        happyTxt.text = $"Mood: {data.mood:F0}";

        if (data.isHungry)
            hungryTxt.text = "Not eating";
        else
            hungryTxt.text = "Eating";

        if (data.isThirsty)
            thirstyTxt.text = "Not drinking";
        else
            thirstyTxt.text = "Drinking";

            iconImg.sprite = data.icon;
    }

    public void Close() => rootPanel.SetActive(false);
}
