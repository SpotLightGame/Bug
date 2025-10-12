using System.Collections;
using UnityEngine;

public class FadeManager : MonoBehaviour
{

    [SerializeField] private GameObject FadeImage;
    public float fadeTime = 1f;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = FadeImage.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        FadeIn();
    }

    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(FadeTo(0f));
    }

    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeTo(1f));
    }

    private IEnumerator FadeTo(float target)
    {
        FadeImage.SetActive(true);
        float start = canvasGroup.alpha;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / fadeTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, t);
            yield return null;
        }
        
        canvasGroup.alpha = target;
        FadeImage.SetActive(false);
    }
}