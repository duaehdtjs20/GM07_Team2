using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_FishChoice : MonoBehaviour
{
    [SerializeField]
    private List<Image> _imageList;

    private RecipeData _recipe;
    public void Init(RecipeData recipe)
    {
        _recipe = recipe;
        RefreshImage();
    }
    private void RefreshImage()
    {
        if (_imageList.Count<=0 || _recipe == null)
        {
            return;
        }

        foreach(Image image in _imageList)
        {
            image.sprite = _recipe.IngredientIcon;
        }
    }
}
