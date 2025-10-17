using UnityEngine;

[CreateAssetMenu(menuName = "Animal/IDGenerator")]
public class AnimalIDGeneratorSO : ScriptableObject
{
    [SerializeField] private int currentID = 0;

    public string GetNextID(string prefix = "animal")
    {
        currentID++;
        return $"{prefix}_{currentID}";
    }
}
