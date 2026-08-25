using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UI_IceBlock : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Image _image;

    private UI_IceBreakGame _game;
    private int _x;
    private int _y;

    public Image Image => _image;
    private void Awake()
    {
        if(_image == null)
        {
            _image = GetComponent<Image>();
        }
    }

    public void Bind(UI_IceBreakGame game, int x, int y)
    {
        _game = game;
        _x = x;
        _y = y;
        _game.OnChanged += Refresh;
    }
    private void OnDestroy()
    {
        if(_game != null)
        {
            _game.OnChanged -= Refresh;
        }
    }
    public void Refresh()
    {
        if(_image == null)
        {
            _image = GetComponent<Image>();
        }
        switch (_game[_x, _y])
        {
            case EIceBlockState.Intact:
                Image.color = new Color(Image.color.r, Image.color.g, Image.color.b, 1.0f);
                break;
            case EIceBlockState.Cracked:
                Image.color = new Color(Image.color.r, Image.color.g, Image.color.b, 0.5f);
                break;
            case EIceBlockState.Breaked:
                Image.color = new Color(Image.color.r, Image.color.g, Image.color.b, 0.0f);
                break;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        _game.BreakBlock(_x, _y);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _game.EnterBlock(_x, _y);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _game.ExitBlock(_x, _y);
    }
}
