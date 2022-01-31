using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Slider))]
public class SliderText : MonoBehaviour
{
    [SerializeField] private TMP_Text text = null;
    private Slider slider = null;
    
    [Header("Text")]
    [Tooltip("Xn - n number of decimal place \nX format - F (fixed-point), D (decimal), C (currency) P (percent), etc")]
    // https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-3.0/dwhawy9k(v=vs.85)?redirectedfrom=MSDN
    [SerializeField] private string textFormat = "F2";
    [SerializeField] private string preText = "";
    [SerializeField] private string postText = "";
    
    void Awake()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnValueChanged);
        OnValueChanged(slider.value);
    }

    private void OnDestroy() 
    {
        slider.onValueChanged.RemoveListener(OnValueChanged);
    }

    public void OnValueChanged(float pValue)
    {
        text.text = preText + pValue.ToString(textFormat) + postText;
    }
}