using System;
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
    private GameObject _alarmImage;
    [SerializeField]
    private RecipeUnlockEffect _unlockEffect;

    private Recipe _recipe;
    private UI_RecipeDetailView _recipeDetailView;

    private Action _onRecipeUnlocked;
    public void Bind(Recipe recipe, UI_RecipeDetailView recipeDetailView, Action onRecipeUnlocked)
    {
        if (recipe == null || recipeDetailView == null)
        {
            return;
        }
        _recipe = recipe;
        _recipeDetailView = recipeDetailView;
        _onRecipeUnlocked = onRecipeUnlocked;
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
        if (_alarmImage != null)
        {
            if(_recipe.Unlocked)
            {
                _alarmImage.SetActive(false);
            }
            else
            {
                _alarmImage.SetActive(_recipe.CanUnlock());
            }
        }
    }
    public void OnClick()
    {
        if(_recipeDetailView == null || _recipe  == null)
        {
            return;
        }

        _recipeDetailView.Bind(_recipe, OnUnlocked);
    }
    private void OnUnlocked()
    {
        _unlockEffect.Play(() =>
        {
            Draw();
            _onRecipeUnlocked?.Invoke();
        });
    }
}
