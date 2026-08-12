using System.Collections;
using TMPro;
using UnityEngine;

public class UI_ToastMessage : MonoBehaviourSingleton<UI_ToastMessage>
{
    [SerializeField]
    private GameObject _messageObject;
    [SerializeField]
    private TMP_Text _messageText;
    [SerializeField]
    private float _duration;

    private Coroutine _messageCoroutine;

    private void Start()
    {
        if(_messageObject != null)
        {
            _messageObject.SetActive(false);
        }
        if(_messageCoroutine != null)
        {
            StopCoroutine(_messageCoroutine);
            _messageCoroutine = null;
        }
    }
    public void Show(string message)
    {
        if (_messageObject == null || _messageText == null)
        {
            return;
        }
        if (_messageCoroutine != null)
        {
            StopCoroutine(_messageCoroutine);
        }
        _messageText.text = message;
        _messageObject.SetActive(true);
        _messageCoroutine = StartCoroutine(ToastMessageCo());
    }
    private IEnumerator ToastMessageCo()
    {
        yield return new WaitForSeconds(_duration);
        if(_messageObject != null)
        {
            _messageObject.SetActive(false);
        }
        _messageCoroutine = null;
    }
}
