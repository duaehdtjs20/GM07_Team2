using System.Collections;
using GM07.Order;
using UnityEngine;
using UnityEngine.UI;

// 손님 머리 위 월드 스페이스 캔버스에 부착. 조리 완료(완성도 결정) 시점에
// TableOrderController → Customer.ShowQualityIcon()을 거쳐 호출됨.
// 평소에는 비활성화 상태로 시작, ShowQuality() 호출 시 잠깐 떴다가 자동으로 사라짐.
public class UI_CustomerQualityIcon : MonoBehaviour
{
    [SerializeField]
    private Image _iconImage;

    // Fail, Normal, Good, Great 순서 (EQuality 선언 순서와 동일하게 맞출 것)
    [SerializeField]
    private Sprite[] _qualityIcons = new Sprite[4];

    [SerializeField]
    private float _displayDuration = 1.5f;

    private Coroutine _hideCoroutine;

    public void ShowQuality(EQuality quality)
    {
        int index = (int)quality;
        if (index < 0 || index >= _qualityIcons.Length || _qualityIcons[index] == null)
        {
            Debug.LogWarning($"UI_CustomerQualityIcon: {quality}에 대응하는 아이콘이 비어있음");
            return;
        }

        if (_hideCoroutine != null)
        {
            StopCoroutine(_hideCoroutine);
        }

        _iconImage.sprite = _qualityIcons[index];
        gameObject.SetActive(true);
        _hideCoroutine = StartCoroutine(HideAfterDelayCo());
    }

    private IEnumerator HideAfterDelayCo()
    {
        yield return new WaitForSeconds(_displayDuration);
        _hideCoroutine = null;
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        _hideCoroutine = null;
    }
}