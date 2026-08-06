using System;
using System.Collections.Generic;

using GM07.Map;

using UnityEngine;

public class Restaurant : MonoBehaviour
{
    private List<Staff> _staffs = new List<Staff>();
    private int _upgrade = 1;

    public int Upgrade => _upgrade;

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
}
