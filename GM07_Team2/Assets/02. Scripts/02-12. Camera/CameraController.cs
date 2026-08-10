using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("카메라 설정")]
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private float _sensitivity = 1.0f;

    private bool _isPressed;
    private Vector3 _prevPosition = Vector3.zero;

    private void Start()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }
    }

    private void Update()
    {
        Pressed();
        Move();
    }

    private void Pressed()
    {
        // 마우스 좌클릭을 하는 경우
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // 마우스 포인터가 UI 위에 없을 시
            if(!EventSystem.current.IsPointerOverGameObject())
            {
                _isPressed = true;
                _prevPosition = Mouse.current.position.ReadValue();
            }
        }
        // 마우스 좌클릭을 해제 하는 경우
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _isPressed = false;
        }
    }
    private void Move()
    {
        if (!_isPressed)
        {
            return;
        }

        // 마우스 움직임 벡터 구하기
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Vector3 moveVector = _prevPosition - mousePosition;

        // 벡터 방향 돌리기
        Vector3 resultVector = Quaternion.AngleAxis(45f, Vector3.up) * moveVector;

        // 최종 벡터 값 움직이기
        _camera.transform.position += resultVector * _sensitivity * Time.deltaTime;

        _prevPosition = mousePosition; 
    }
}
