using System;
using System.Collections.Generic;

using GM07.Map;

using UnityEngine;

public class Restaurant : MonoBehaviour
{
    private List<Staff> _staffs = new List<Staff>();
    private int _level = 1;
    private int _upgradeCost = 10; // 임시 업그레이드 비용

    public IReadOnlyList<Staff> Staffs => _staffs;
    public int Level => _level;

    private void Start()
    {
        TableManager tableManager = FindFirstObjectByType<TableManager>();
        if(tableManager != null)
        {
            for (int i = 0; i < tableManager.TableCount; i++)
            {
                _staffs.Add(new Staff($"직원{i}", 1.1f, 1));
            }
        }
    }
    public Staff GetStaff(int index)
    {
        if (index < 0 || index >= _staffs.Count)
        {
            return null;
        }
        return _staffs[index];
    }
    public void Upgrade()
    {
        if (CurrencyManager.Instance == null)
        {
            return;
        }
        if(CurrencyManager.Instance.TrySpendMoney(_upgradeCost, ECurrencyTransactionType.Sale))
        {
            _level++;
        }
    }
}
