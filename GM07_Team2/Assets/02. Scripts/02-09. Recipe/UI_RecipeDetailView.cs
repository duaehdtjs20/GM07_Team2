using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class UI_RecipeDetailView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _name;
    [SerializeField]
    private Image _image;
    [SerializeField]
    private TMP_Text _price;
    [SerializeField]
    private TMP_Text _cost;
    [SerializeField]
    private TMP_Text _rank;
    [SerializeField]
    private Button _unlockButton;

    private Recipe _recipe;
    private Action _onUnlocked;

    private void Awake()
    {
        if (_unlockButton != null)
        {
            _unlockButton.onClick.AddListener(Unlock);
        }
    }
    public void Bind(Recipe recipe, Action onUnlocked)
    {
        if (recipe == null)
        {
            return;
        }

        _recipe = recipe;
        _onUnlocked = onUnlocked;

        Draw();
    }
    private void Draw()
    {
        if (_recipe == null)
        {
            return;
        }

        if (_image != null)
        {
            _image.sprite = _recipe.Data.Icon;
        }
        if (_name != null)
        {
            _name.text = _recipe.Data.Name;
        }
        if (_price != null)
        {
            _price.text = _recipe.Data.Price.ToString();
        }
        if(_cost != null)
        {
            _cost.text = _recipe.Data.Cost.ToString();
        }
        if( _rank != null)
        {
            _rank.text = _recipe.Data.MenuGrade.ToString();
        }
        if( _unlockButton != null)
        {
            _unlockButton.gameObject.SetActive(!_recipe.Unlocked);
        }
    }
    private void Unlock()
    {
        if(_recipe == null)
        {
            return;
        }

        if (_recipe.Unlock())
        {
            Draw();
            _onUnlocked?.Invoke();
        }
        else
        {
            if(_unlockButton.TryGetComponent(out ButtonFailEffect effect))
            {
                effect.Play();
            }
            return;
        }
    }
}
