using UnityEngine;
using UnityEngine.UI;

public class UI_AudioSlider : MonoBehaviour
{
    [SerializeField]
    private Slider _slider;
    [SerializeField]
    private ESliderType _type;

    private void Awake()
    {
        if (_slider == null)
        {
            _slider = GetComponent<Slider>();
        }
    }
    private void Start()
    {
        Init();
    }

    public void SetVolume()
    {
        if (AudioManager.Instance == null)
        {
            return;
        }
        switch (_type)
        {
            case ESliderType.Master:
                SetMasterVolume();
                break;
            case ESliderType.BGM:
                SetBGMVolume();
                break;
            case ESliderType.SFX:
                SetSFXVolume();
                break;
        }
    }

    private void SetMasterVolume()
    {
        AudioManager.Instance.SetMasterVolume(_slider.value);
    }
    private void SetBGMVolume()
    {
        AudioManager.Instance.SetBGMVolume(_slider.value);
    }
    private void SetSFXVolume()
    {
        AudioManager.Instance.SetSFXVolume(_slider.value);
    }
    private void Init()
    {
        if (AudioManager.Instance == null)
        {
            return;
        }
        float value = 0.0f;
        switch (_type)
        {
            case ESliderType.Master:
                AudioManager.Instance.Mixer.GetFloat("Master", out value);
                break;
            case ESliderType.BGM:
                AudioManager.Instance.Mixer.GetFloat("BGM", out value);
                break;
            case ESliderType.SFX:
                AudioManager.Instance.Mixer.GetFloat("SFX", out value);
                break;
        }
        _slider.value = Mathf.Pow(10, value / 20);
    }
}
