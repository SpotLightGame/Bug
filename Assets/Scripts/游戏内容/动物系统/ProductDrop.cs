using UnityEngine;

public class ProductDrop : MonoBehaviour
{
    public float productValue;
    [SerializeField] private AnimalProduct productType;

    public void PickUp()
    {
        Debug.Log("SOLD OUT");
        FindObjectOfType<ResourcesManager>().AddGold(productValue);
        Destroy(gameObject);
    }
    
}
