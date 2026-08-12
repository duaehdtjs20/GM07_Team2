using TMPro;
using UnityEngine;

public class UI_StaffView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _name;
    [SerializeField]
    private TMP_Text _level;

    private Staff _staff;
    private UI_StaffDetailView _staffDetailView;
    public void Bind(Staff staff, UI_StaffDetailView staffDetailView)
    {
        if(staff == null || staffDetailView == null)
        {
            return;
        }
        _staff = staff;
        _staffDetailView = staffDetailView;

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
    }
    public void OnClick()
    {
        if(_staff == null || _staffDetailView == null)
        {
            return;
        }
        _staffDetailView.Bind(_staff, Draw);
    }
}
