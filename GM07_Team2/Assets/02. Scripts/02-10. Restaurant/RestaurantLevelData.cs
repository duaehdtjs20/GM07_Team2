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
    [SerializeField, Min(1)]
    private int _openingTime; //해당 레벨에서 영업 가능한 시간
    [Header("Unlock Objects")]
    [SerializeField]
    private List<GameObject> _unlockObjectList = new();
    [Header("Add Objects")]
    [SerializeField]
    private List<GameObject> _addObjectList = new();

    public int TableCount => _tableCount;
    public int ChefCount => _tableCount;
    public int Rent => _rent;
    public int UpgradeCost => _upgradeCost;
    public int OpeningTime => _openingTime;
    public IReadOnlyList<GameObject> UnlockObjectList => _unlockObjectList;
    public IReadOnlyList<GameObject> AddObjectList => _addObjectList;
}
