using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class JellyClick : MonoBehaviour, IPointerClickHandler
{
    public float punch = 0.3f;
    public float duration = 0.4f;

    private Vector3 baseScale;

    void Awake() => baseScale = transform.localScale;

    public void OnPointerClick(PointerEventData _) => StartCoroutine(Jelly());

    IEnumerator Jelly()
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            // 用正弦包络做出挤压+回弹
            float s = 1 + Mathf.Sin(t * Mathf.PI * 4) * punch * (1 - t);
            transform.localScale = baseScale * s;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = baseScale;
    }
}