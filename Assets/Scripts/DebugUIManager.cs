using UnityEngine;
using TMPro;

public class DebugUIManager : MonoBehaviour
{
    public static DebugUIManager Instance;

    [SerializeField] private TextMeshProUGUI debugText;

    void Awake()
    {
        if(Instance == null)
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }
    public void WriteUI(string text)
    {
        debugText.text = text;
    }
}
