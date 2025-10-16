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

        ageTxt.text = $"已陪伴你: {info.age} 天";

        //亲密值
        if (info.closenessLevel == 0)
        {
            closenessTxt.text = $"对你感到陌生";
        }
        else if (info.closenessLevel == 1)
        {
            closenessTxt.text = $"1";
        }
        else if (info.closenessLevel == 2)
        {
            closenessTxt.text = $"2";
        }
        else if (info.closenessLevel == 3)
        {
            closenessTxt.text = $"3";
        }
        else if (info.closenessLevel == 4)
        {
            closenessTxt.text = $"4";
        }
        else
        {
            closenessTxt.text = $"5";
        }
        
        //心情
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
