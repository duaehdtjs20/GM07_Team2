using System.Collections;

using UnityEngine;

public class ManualSystem : MonoBehaviour
{
    [SerializeField]
    private GameFlowManager _flowManager;
    [SerializeField]
    private GameObject _cover;
    [SerializeField]
    private GameObject[] _manuals;
    [SerializeField]
    private UI_typingText[] _typingText;
    [SerializeField]
    private float _waitSecond = 0.5f;
    private int _index = -1;

    private void Start()
    {
        if (_flowManager != null)
        {
            StartCoroutine(OpenManualCo());
        }
    }

    public void OpenNext()
    {
        if (_index >= _manuals.Length || _index >= _typingText.Length)
        {
            return;
        }

        if (!_typingText[_index].IsComplete)
        {
            _typingText[_index].Skip();
            return;
        }

        _index++;
        for (int i = 0; i < _manuals.Length; i++)
        {
            _manuals[i].SetActive(false);
        }
        if (_index < _manuals.Length)
        {
            _manuals[_index].SetActive(true);
        }
    }
    private void Open()
    {
        _index = 0;
        _manuals[_index].SetActive(true);
    }
    private IEnumerator OpenManualCo()
    {
        for (int i = 0; i < _manuals.Length; i++)
        {
            _manuals[i].SetActive(false);
        }
        _cover.SetActive(true);
        yield return new WaitForSeconds(_waitSecond);
        _cover.SetActive(false);
        if (_flowManager.CurrentDay == 1)
        {
            Open();
        }
    }
}
