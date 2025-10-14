using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceManager : MonoBehaviour
{
    public AnimalPlace placeType;
    public bool hasBugAnimal;

    private void OnEnable()
    {
        TimeManager.Instance.OnNewDay += Refresh;
    }

    private void OnDisable()
    {
        TimeManager.Instance.OnNewDay -= Refresh;
    }

    public void Refresh()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<AnimalInfo>().isBug)
            {
                hasBugAnimal = true;
                break;
            }
        }
    }
}
