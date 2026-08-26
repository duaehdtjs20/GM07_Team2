using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SequencePlate : MonoBehaviour
{
    [SerializeField]
    private Image _sushiImage;
    [SerializeField]
    private Button _button;
    [SerializeField]
    private TMP_Text _orderText;

    private int _index;
    private Action<int> _onClick;
    private bool _canClick = false;

    private void Awake()
    {
        _button?.onClick.AddListener(OnClick);
    }
    private void OnDestroy()
    {
        _button?.onClick.RemoveListener(OnClick);
    }
    public void Bind(int index, Sprite sprite, Action<int> onClick)
    {
        _index = index;
        if(_sushiImage != null)
        {
            _sushiImage.gameObject.SetActive(false);
            _sushiImage.sprite = sprite;
        }
        _onClick = onClick;
        _canClick = false;
        if (_orderText != null)
        {
            _orderText.text = string.Empty;
        }
    }
    public void SetOrderText(int order)
    {
        if(_orderText != null)
        {
            _orderText.text = order.ToString();
        }
    }
    public void ResetOrder()
    {
        if (_orderText != null)
        {
            _orderText.text = string.Empty;
        }
    }
    public void SetInteractable(bool interactable)
    {
        _canClick = interactable;
    }
    private void OnClick()
    {
        if (!_canClick)
        {
            return;
        }
        _canClick = false;
        if (_sushiImage != null)
        {
            _sushiImage.gameObject.SetActive(true);
        }
        _onClick?.Invoke(_index);
    }
}
