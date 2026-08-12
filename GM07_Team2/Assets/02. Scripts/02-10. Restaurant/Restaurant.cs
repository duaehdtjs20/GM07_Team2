using System;
using System.Collections.Generic;
using GM07.Map;
using UnityEngine;

public class Restaurant : MonoBehaviour
{
    [SerializeField]
    private TableManager _tableManager;
    [Header("Level Settings")]
    [SerializeField]
    private List<RestaurantLevelData> _levelDataList = new();
    [Header("Staff Settings")]
    [SerializeField]
    private StaffUpgradeDataBase _staffUpgradeDataBase;

    private readonly List<Staff> _staffs = new();
    private int _level = 1;

    public IReadOnlyList<Staff> Staffs => _staffs;
    public int StaffCount => _staffs.Count;
    public int Level => _level;
    public int MaxLevel => _levelDataList.Count;
    public bool IsMaxLevel => _level >= _levelDataList.Count;
    public int TableCount => GetLevelData(_level).TableCount;
    public int ChefCount => GetLevelData(_level).ChefCount;
    public int Rent => GetLevelData(_level).Rent;
    public int UpgradeCost => IsMaxLevel? 0 : GetLevelData(_level + 1).UpgradeCost;
    public int TotalWage
    {
        get
        {
            int totalWage = 0;
            foreach (Staff staff in Staffs)
            {
                totalWage += staff.Wage;
            }
            return totalWage;
        }
    }

    public event Action OnRestaurantChanged;

    public void InitNewGame()
    {
        ApplyLevel(1);
    }

    public void InitSaveData(int level, IReadOnlyList<StaffSaveData> savedStaffs)
    {
        _level = Mathf.Clamp(level, 1, MaxLevel);
        _staffs.Clear();

        if (savedStaffs != null)
        {
            foreach (StaffSaveData staff in savedStaffs)
            {
                if (staff != null)
                {
                    _staffs.Add(new Staff(staff.Name,staff.Upgrade ,_staffUpgradeDataBase));
                }
            }
        }

        SyncRestaurantState();
    }
    public int GetTableCount(int level)
    {
        return GetLevelData(level)?.TableCount ?? 0;
    }
    public int GetChefCount(int level)
    {
        return GetLevelData(level)?.ChefCount ?? 0;
    }
    public int GetRent(int level)
    {
        return GetLevelData(level)?.Rent ?? 0;
    }
    public bool TryUpgrade()
    {
        if (IsMaxLevel || CurrencyManager.Instance == null)
        {
            return false;
        }

        if (!CurrencyManager.Instance.TrySpendMoney(UpgradeCost,ECurrencyTransactionType.OtherExpense))
        {
            return false;
        }

        ApplyLevel(_level + 1);
        return true;
    }
    private void ApplyLevel(int level)
    {
        _level = Mathf.Clamp(level, 1, MaxLevel);
        SyncRestaurantState();
    }

    private void SyncRestaurantState()
    {
        ApplyMapUnlockState();
        _tableManager?.SetTableCount(TableCount);
        SyncChefCount(ChefCount);
        OnRestaurantChanged?.Invoke();
    }

    private void ApplyMapUnlockState()
    {
        for(int i=0;i<_levelDataList.Count;i++)
        {
            RestaurantLevelData levelData = _levelDataList[i];

            if(levelData == null)
            {
                continue;
            }

            int targetLevel = i + 1;
            bool isUnlocked = _level >= targetLevel;

            foreach(GameObject target in levelData.UnlockObjectList)
            {
                if (target == null)
                {
                    continue;
                }
                target.SetActive(!isUnlocked);
            }
        }
    }

    private void SyncChefCount(int targetCount)
    {
        if(_staffUpgradeDataBase == null)
        {
            return;
        }
        while (_staffs.Count < targetCount)
        {
            _staffs.Add(new Staff($"요리사 {_staffs.Count + 1}", 1, _staffUpgradeDataBase));
        }
        while(_staffs.Count > targetCount)
        {
            _staffs.RemoveAt(_staffs.Count - 1);
        }
    }

    private RestaurantLevelData GetLevelData(int level)
    {
        if (_levelDataList == null || _levelDataList.Count == 0)
        {
            return null;
        }
        return _levelDataList[Mathf.Clamp(level - 1, 0, _levelDataList.Count - 1)];
    }

    public bool TryGetStaffIndex(int index, out Staff staff)
    {
        if(index<0|| index >= _staffs.Count)
        {
            staff = null;
            return false;
        }

        staff = _staffs[index];
        return staff != null;
    }
}
