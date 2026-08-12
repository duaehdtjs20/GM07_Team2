using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StaffDetailView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _name;
    [SerializeField]
    private Button _upgradeButton;
    [SerializeField]
    private GameObject _maxImage;
    [Header("Current")]
    [SerializeField]
    private TMP_Text _currentLevel;
    [SerializeField]
    private TMP_Text _currentDetails;
    [Header("Next")]
    [SerializeField]
    private TMP_Text _nextLevel;
    [SerializeField]
    private TMP_Text _nextDetails;

    private Staff _staff;
    private Action _onStaffUpgrade;
    private void Awake()
    {
        if(_upgradeButton != null)
        {
            _upgradeButton.onClick.AddListener(OnClickUpgrade);
        }
    }
    public void Bind(Staff staff, Action onUpgrade)
    {
        if(staff == null)
        {
            return;
        }
        _staff = staff;
        _onStaffUpgrade = onUpgrade;

        Draw();
    }
    private void Draw()
    { 
        if(_staff == null)
        {
            return;
        }
        if (_staff.IsMexLevel)
        {
            _maxImage?.SetActive(true);
        }
        else
        {
            _maxImage?.SetActive(false);
        }
        if (_name != null)
        {
            _name.text = _staff.Name;
        }
        if(_currentLevel != null)
        {
            _currentLevel.text = $"Lv. {_staff.Upgrade}";
        }
        if(_currentDetails != null)
        {
            _currentDetails.text = $"{_staff.CookSpeed}\n\n" + $"{_staff.Wage:N0}";
        }

        StaffLevelData nextData = _staff.NextLevelData;

        if(_nextLevel != null)
        {
            if (_staff.IsMexLevel)
            {
                _nextLevel.text = string.Empty;
            }
            else
            {
                _nextLevel.text = $"Lv.{_staff.Upgrade + 1}";
            }
        }
        if (_nextDetails != null)
        {
            if (_staff.IsMexLevel)
            {
                _nextDetails.text = string.Empty;
            }
            else
            {
                _nextDetails.text =
                $"{nextData.CookSpeed}\n\n" +
                $"{nextData.Wage:N0}\n\n" +
                $"{nextData.UpgradeCost:N0}";
            }
        }
    }
    private void OnClickUpgrade()
    {
        if(_staff == null)
        {
            return;
        }
        if (!_staff.TryUpgrade())
        {
            return;
        }
        Draw();
        _onStaffUpgrade?.Invoke();
    }
}
