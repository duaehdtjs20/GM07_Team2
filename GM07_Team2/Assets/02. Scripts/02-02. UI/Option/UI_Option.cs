using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Option : MonoBehaviour
{
    [SerializeField]
    private GameObject _optionPanel;
    [SerializeField]
    private Button _closeButton;

    private void Awake()
    {
        if (_closeButton != null)
        {
            _closeButton.onClick.AddListener(Save);
        }
    }

    private void Update()
    {
        if (!Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            return;
        }
        _optionPanel.SetActive(!_optionPanel.activeSelf);
        if (!_optionPanel.activeSelf)
        {
            Save();
        }
    }
    private void Save()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveOption();
        }
    }
}
