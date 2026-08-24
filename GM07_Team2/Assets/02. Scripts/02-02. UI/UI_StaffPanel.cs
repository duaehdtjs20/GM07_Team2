using System.Collections.Generic;
using UnityEngine;

public class UI_StaffPanel : MonoBehaviour
{
    [SerializeField]
    private Restaurant _restaurant;
    [Header("Staff List")]
    [SerializeField]
    private Transform _listRoot;
    [SerializeField]
    private UI_StaffView _staffViewPrefab;
    [SerializeField]
    private UI_StaffDetailView _staffDetailView;

    private List<UI_StaffView> _staffViews = new();

    private void OnEnable()
    {
        if (_restaurant == null)
        {
            return;
        }
        _restaurant.OnRestaurantChanged += RefreshUI;
        RefreshUI();
    }
    private void OnDisable()
    {
        if(_restaurant == null)
        {
            return;
        }
        _restaurant.OnRestaurantChanged -= RefreshUI;
    }
    private void RefreshUI()
    {
        ClearStaffViews();

        int count = _restaurant.StaffCount;
        for (int i = 0; i < count; i++)
        {
            if (!_restaurant.TryGetStaffIndex(i, out Staff staff))
            {
                continue;
            }
            UI_StaffView view = Instantiate(_staffViewPrefab, _listRoot);
            view.Bind(staff, _staffDetailView);
            _staffViews.Add(view);
        }
        if(_staffViews.Count > 0)
        {
            _staffViews[0].OnClick();
        }
    }
    private void ClearStaffViews()
    {
        foreach(UI_StaffView view in _staffViews)
        {
            if(view != null)
            {
                Destroy(view.gameObject);
            }
        }
        _staffViews.Clear();
    }
}
