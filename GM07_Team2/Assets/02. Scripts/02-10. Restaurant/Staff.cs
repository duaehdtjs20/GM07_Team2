using System;

using UnityEngine;

[Serializable]
public class Staff
{
    [SerializeField]
    private string _name;
    [SerializeField]
    private int _upgrade;
    [SerializeField]
    private EStaffState _state;

    [NonSerialized]
    private StaffUpgradeDataBase _upgradeDataBase;

    public string Name => _name;
    public float CookSpeed => CurrentLevelData.CookSpeed;
    public int Upgrade => _upgrade;
    public EStaffState State => _state;
    public int Wage => CurrentLevelData.Wage;
    public int MaxLevel => _upgradeDataBase.MaxLevel;
    public bool IsMexLevel => _upgrade >= MaxLevel;
    public int UpgradeCost => NextLevelData.UpgradeCost;
    public StaffLevelData CurrentLevelData
    {
        get
        {
            if(_upgradeDataBase.TryGetLevelData(_upgrade, out StaffLevelData data))
            {
                return data;
            }
            return null;
        }
    }
    public StaffLevelData NextLevelData
    {
        get
        {
            if (IsMexLevel)
            {
                return null;
            }
            if(_upgradeDataBase.TryGetLevelData(_upgrade+1,out StaffLevelData data))
            {
                return data;
            }
            return null;
        }
    }
    public Staff(string name, int upgrade, StaffUpgradeDataBase upgradeDataBase)
    {
        _name = name;
        _upgrade = upgrade;
        _upgradeDataBase = upgradeDataBase;
        _state = EStaffState.Idle;
    }
    public void Bind(StaffUpgradeDataBase upgradeDataBase)
    {
        _upgradeDataBase = upgradeDataBase;
        if(_upgradeDataBase != null)
        {
            _upgrade = Mathf.Clamp(_upgrade,1,Math.Max(1,_upgradeDataBase.MaxLevel));
        }
    }
    public bool TryUpgrade()
    {
        if(IsMexLevel || NextLevelData == null || CurrencyManager.Instance == null)
        {
            return false;
        }
        if (!CurrencyManager.Instance.TrySpendMoney(UpgradeCost, ECurrencyTransactionType.OtherExpense))
        {
            return false;
        }
        _upgrade++;
        return true;
    }
    public void SetState(EStaffState state)
    {
        _state = state;
    }
    public StaffSaveData CreateSaveData()
    {
        return new StaffSaveData(_name, _upgrade);
    }
}

[Serializable]
public class StaffSaveData
{
    [SerializeField]
    private string _name;
    [SerializeField]
    private int _upgrade;
    public string Name => _name;
    public int Upgrade => _upgrade;
    public StaffSaveData(string name, int upgrade)
    {
        _name = name;
        _upgrade = upgrade;
    }
    
}
