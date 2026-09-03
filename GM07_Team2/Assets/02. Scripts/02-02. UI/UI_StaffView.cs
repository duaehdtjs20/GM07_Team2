using System;
using TMPro;
using UnityEngine;

public class UI_StaffView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _name;
    [SerializeField]
    private TMP_Text _level;
    [SerializeField]
    private GameObject _alarmImage;

    private Staff _staff;
    private UI_StaffDetailView _staffDetailView;

    private Action _onStaffUpgraded;
    public void Bind(Staff staff, UI_StaffDetailView staffDetailView, Action onStaffUpgraded)
    {
        if(staff == null || staffDetailView == null)
        {
            return;
        }
        _staff = staff;
        _staffDetailView = staffDetailView;
        _onStaffUpgraded = onStaffUpgraded;

        Draw();
    }
    public void Draw()
    {
        if (_staff == null)
        {
            return;
        }
        if (_name != null)
        {
            _name.text = _staff.Name;
        }
        if(_level != null)
        {
            _level.text = $"Lv.{_staff.Upgrade}";
        }
        if(_alarmImage != null)
        {
            _alarmImage.SetActive(_staff.CanUpgrade());
        }
    }
    public void OnClick()
    {
        if(_staff == null || _staffDetailView == null)
        {
            return;
        }
        _staffDetailView.Bind(_staff, OnUpgraded);
    }
    private void OnUpgraded()
    {
        Draw();
        _onStaffUpgraded?.Invoke();
    }
}
