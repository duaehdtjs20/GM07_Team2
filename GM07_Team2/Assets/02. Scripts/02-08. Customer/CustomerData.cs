using System;

using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "CustomerBaseData", menuName = "Customer/CustomerData")]
public class CustomerData : ScriptableObject
{
    [SerializeField]
    private float _speed = 5.0f;
    [SerializeField]
    private float _eatTime = 3.0f;
    [SerializeField]
    private float _waitTime = 10.0f;

    public float Speed => _speed;
    public float EatTime => _eatTime;
    public float WaitTime => _waitTime;
}
