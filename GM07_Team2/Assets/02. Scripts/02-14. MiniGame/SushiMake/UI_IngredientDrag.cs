using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_IngredientDrag : MonoBehaviour, IBeginDragHandler,IDragHandler,IEndDragHandler
{
    [SerializeField]
    private EIngredientType _ingredientType;
    [SerializeField]
    private Image _iconImage;
    [SerializeField]
    private List<Image> _fishImages;

    private Canvas _rootCanvas;
    private RectTransform _rootCanvasRect;

    private Transform _originalParent;
    private Vector3 _originalWorldPosition;
    private int _originalSiblingIndex;

    public EIngredientType IngredientType => _ingredientType;

    public RecipeData RecipeData { get; private set; }

    private void Awake()
    {
        Canvas parentCanvas = GetComponentInParent<Canvas>();

        if(parentCanvas != null )
        {
            _rootCanvas = parentCanvas.rootCanvas;
            _rootCanvasRect = _rootCanvas.transform as RectTransform;
        }
    }
    public void InitFish(RecipeData recipe)
    {
        _ingredientType = EIngredientType.Fish;
        RecipeData = recipe;
        if(_fishImages != null)
        {
            foreach(Image image in _fishImages)
            {
                image.sprite = recipe.IngredientIcon;
            }
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(_rootCanvas == null)
        {
            return;
        }
        _originalParent = transform.parent;
        _originalWorldPosition = transform.position;
        _originalSiblingIndex = transform.GetSiblingIndex();

        transform.SetParent(_rootCanvas.transform, true);

        transform.SetAsLastSibling();

        if(_iconImage != null)
        {
            _iconImage.raycastTarget = false;
        }
        MoveToPointer(eventData);
    }
    public void OnDrag(PointerEventData eventData)
    {
        MoveToPointer(eventData);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if(_iconImage != null)
        {
            _iconImage.raycastTarget = true;
        }
        ReturnPosition();
    }
    private void MoveToPointer(PointerEventData eventData)
    {
        if(_rootCanvasRect == null)
        {
            return;
        }
        if(RectTransformUtility.ScreenPointToWorldPointInRectangle(
            _rootCanvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector3 worldPoint))
        {
            transform.position = worldPoint;
        }
    }
    private void ReturnPosition()
    {
        if(_originalParent == null)
        {
            return;
        }
        transform.SetParent(_originalParent, true);
        transform.SetSiblingIndex(_originalSiblingIndex);
        transform.position = _originalWorldPosition;
    }
}
