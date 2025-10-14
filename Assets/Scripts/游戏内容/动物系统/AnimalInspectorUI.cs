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

    public void Show(AnimalInfo info)
    {
        rootPanel.SetActive(true);
        
        ageTxt.text = $"stay with you for: {info.age} days";
        
        closenessTxt.text = $"Closeness: {info.closeness:F1}";

        happyTxt.text = $"Mood: {info.mood:F0}";

        if (info.isHungry)
            hungryTxt.text = "Not eating";
        else
            hungryTxt.text = "Eating";

        if (info.isThirsty)
            thirstyTxt.text = "Not drinking";
        else
            thirstyTxt.text = "Drinking";
    }

    public void Close() => rootPanel.SetActive(false);
}
