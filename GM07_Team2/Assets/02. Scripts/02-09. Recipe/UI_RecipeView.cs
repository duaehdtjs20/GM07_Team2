using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class UI_RecipeView : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private TMP_Text _gradeText;
    [SerializeField] private TMP_Text _unlockedText;
    [SerializeField] private Button _unlockButton;

    private RecipeData _data;
    public void Bind(RecipeData data)
    {
        if (data == null)
        {
            return;
        }

        _data = data;
        _unlockButton.onClick.AddListener(Unlock);
    }

    // 호출 순서 명시하기 위한 메서드
    private void Unlock()
    {
        _data.Unlock();
        Draw();
    }
    public void Draw()
    {
        if (_data == null)
        {
            return;
        }

        if(_icon != null)
        {
            _icon.sprite = _data.Icon;
        }
        if(_nameText != null)
        {
            _nameText.text = _data.Name;
        }
        if(_costText != null)
        {
            _costText.text =  "Cost:" + _data.Cost.ToString();
        }
        if(_priceText != null)
        {
            _priceText.text = "Price:" + _data.Price.ToString();
        }
        if(_gradeText != null)
        {
            _gradeText.text = "Grade:" + _data.Grade.ToString();
        }
        if(_unlockedText != null)
        {
            _unlockedText.text = "Unlocked:"+_data.Unlocked.ToString();
        }
    }
}
