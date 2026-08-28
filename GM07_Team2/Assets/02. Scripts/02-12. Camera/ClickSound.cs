using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ClickSound : MonoBehaviour
{
    void Update()
    {
        // 마우스 입력이 없음
        if (Mouse.current == null)
        {
            return;
        }
        // 마우스 좌클릭을 누르지 않음
        if (!Mouse.current.leftButton.wasPressedThisFrame)
        {
            return;
        }
        // 마우스가 UI 위에 있지 않음
        if(!EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // 마우스 위치 레이캐스트 수행
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        // 캐스팅 된 오브젝트가 없음
        if(raycastResults.Count <= 0)
        {
            return;
        }

        // 클릭 시 가장 위에 있던 오브젝트
        var clickObject = raycastResults[0].gameObject;

        // 버튼 클릭
        var btn = clickObject.GetComponentInParent<Button>();
        if(btn != null && btn.interactable && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(EAudioType.Button);
            return;
        }
    }
}
