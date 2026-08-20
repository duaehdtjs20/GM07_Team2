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


    // 플레이어가 보는 쿼터뷰 카메라 — 인스펙터에서 직접 연결 (비워두면 Camera.main으로 자동 탐색)
    [SerializeField]
    private Camera _viewCamera;

    // _followTarget(손님 모델) 기준 머리 위로 띄울 오프셋
    [SerializeField]
    private Vector3 _followOffset = new Vector3(0f, 2.0f, 0f);

    private Transform _followTarget;


    private Coroutine _hideCoroutine;

    private void Start()
    {
        if (_viewCamera == null)
        {
            _viewCamera = Camera.main;
        }
    }

    private void LateUpdate()
    {
        if (_followTarget != null)
        {
            transform.position = _followTarget.position + _followOffset;
        }

        // 손님이 어느 방향으로 앉든 항상 플레이어 화면 정면으로 보이게 카메라 회전을 그대로 따라감
        if (_viewCamera != null)
        {
            transform.rotation = _viewCamera.transform.rotation;
        }
    }

    // 손님 모델이 정해지는 시점(Customer.Init)에 호출 — 이후 이 트랜스폼의 위치를 따라감
    public void SetFollowTarget(Transform target)
    {
        _followTarget = target;
    }


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