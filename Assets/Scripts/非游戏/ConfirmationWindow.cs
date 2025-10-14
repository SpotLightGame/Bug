// ConfirmationWindow.cs
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationWindow : MonoBehaviour
{
    public static ConfirmationWindow I;   // 单例
    [SerializeField] Text infoText;       // 提示文字
    [SerializeField] Button yesBtn;       // 确定按钮
    [SerializeField] Button noBtn;        // 取消按钮

    System.Action onYes;   // 缓存回调

    void Awake()
    {
        I = this;
        gameObject.SetActive(false);      // 默认隐藏
        yesBtn.onClick.AddListener(() => { gameObject.SetActive(false); onYes?.Invoke(); });
        noBtn.onClick.AddListener(() => { gameObject.SetActive(false); });
    }

    // 外部调用接口
    public void Show(string msg, System.Action onConfirm = null)
    {
        infoText.text = msg;
        onYes = onConfirm;
        gameObject.SetActive(true);
    }
}