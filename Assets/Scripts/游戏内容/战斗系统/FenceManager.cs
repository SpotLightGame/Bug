using UnityEngine;

public class FenceManager : MonoBehaviour
{
    public float hp = 100f;
    public void TakeDamage(float dmg) { hp -= dmg; if (hp <= 0) Debug.Log("围栏破损！"); }
}
