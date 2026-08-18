using System;
using UnityEngine;

[Serializable]
public class StaffLevelData
{
    [SerializeField]
    private float _cookSpeed = 1f;
    [SerializeField]
    [Range(0f,1f)]
    private float _qualityBonus;
    [SerializeField]
    private int _wage;
    [SerializeField]
    private int _upgradeCost;

    public float CookSpeed => _cookSpeed;
    public float QualityBonus => _qualityBonus;
    public int Wage => _wage;
    public int UpgradeCost => _upgradeCost;
}
