using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RestaurantLevelData
{
    [SerializeField, Min(1)]
    private int _tableCount;
    [SerializeField, Min(0)]
    private int _rent;
    [SerializeField, Min(0)]
    private int _upgradeCost; //해당 레벨로 업그레이드 할 때 필요한 비용
    [Header("Unlock Objects")]
    [SerializeField]
    private List<GameObject> _unlockObjectList;

    public int TableCount => _tableCount;
    public int ChefCount => _tableCount;
    public int Rent => _rent;
    public int UpgradeCost => _upgradeCost;
    public IReadOnlyList<GameObject> UnlockObjectList => _unlockObjectList;
}
