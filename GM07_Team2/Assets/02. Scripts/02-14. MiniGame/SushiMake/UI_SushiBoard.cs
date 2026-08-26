using UnityEngine;
using UnityEngine.EventSystems;

public class UI_SushiBoard : MonoBehaviour,IDropHandler
{
    [SerializeField]
    private UI_SushiMiniGame _miniGame;

    public void OnDrop(PointerEventData eventData)
    {
        if(_miniGame == null)
        {
            return;
        }
        if(eventData.pointerDrag.TryGetComponent(out UI_IngredientDrag ingredient))
        {
            _miniGame.OnIngredientDrop(ingredient);
        }
    }
}
