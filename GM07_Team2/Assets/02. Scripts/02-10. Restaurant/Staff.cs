using System;

using UnityEngine;

[Serializable]
public class Staff
{
    [SerializeField]
    private string _name;
    [SerializeField]
    private float _cookSpeed;
    [SerializeField]
    private int _upgrade;
    [SerializeField]
    private EStaffState _state;

    public string Name => _name;
    public float CookSpeed => _cookSpeed;
    public int Upgrade => _upgrade;
    public EStaffState State => _state;

    public Staff(string name, float cookSpeed, int upgrade)
    {
        _name = name;
        _cookSpeed = cookSpeed;
        _upgrade = upgrade;
        _state = EStaffState.Idle;
    }
    public void SetState(EStaffState state)
    {
        _state = state;
    }
}
