using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_WasabiPressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private UI_WasabiGame _game;
    [SerializeField]
    private Button _button;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_button != null && !_button.interactable)
        {
            return;
        }

        if (_game != null)
        {
            _game.BeginSqueeze();
        }
    }
    public void OnPointerUp( PointerEventData eventData)
    {
        if (_button != null && !_button.interactable)
        {
            return;
        }

        if (_game != null)
        {
            _game.EndSqueeze();
        }
    }

}
