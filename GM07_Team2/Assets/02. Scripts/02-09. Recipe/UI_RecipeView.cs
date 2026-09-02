using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RecipeView : MonoBehaviour
{
    [SerializeField]
    private Image _icon;
    [SerializeField]
    private TMP_Text _nameText;
    [SerializeField]
    private TMP_Text _stateText;
    [SerializeField]
    private GameObject _unlockImage;
    [SerializeField]
    private RecipeUnlockEffect _unlockEffect;

    private Recipe _recipe;
    private UI_RecipeDetailView _recipeDetailView;
    public void Bind(Recipe recipe, UI_RecipeDetailView recipeDetailView)
    {
        if (recipe == null || recipeDetailView == null)
        {
            return;
        }

        _recipe = recipe;
        _recipeDetailView = recipeDetailView;
    }
    public void Draw()
    {
        if (_recipe == null)
        {
            return;
        }

        if(_icon != null)
        {
            _icon.sprite = _recipe.Data.Icon;
        }
        if(_nameText != null)
        {
            _nameText.text = _recipe.Data.Name;
        }
        if(_stateText != null)
        {
            _stateText.text = (_recipe.Unlocked) ? "보유중" : "잠김";
        }
        if( _unlockImage != null)
        {
            _unlockImage.SetActive(!_recipe.Unlocked);
        }
    }
    public void OnClick()
    {
        if(_recipeDetailView == null || _recipe  == null)
        {
            return;
        }

        _recipeDetailView.Bind(_recipe, Draw);
    }
}
