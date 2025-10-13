using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Animal/Data")]
public class AnimalData : ScriptableObject
{
    [Header("静态配置")]
    public string animalName;
    public Sprite   icon;
    public int      maxAge;

    [Header("动态存档")]
    public int age;
    public float closeness;
    public float mood;
    public bool isHungry;
    public bool isThirsty;
    
}
