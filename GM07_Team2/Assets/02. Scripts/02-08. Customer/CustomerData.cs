using System;

using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "CustomerBaseData", menuName = "Customer/CustomerData")]
public class CustomerData : ScriptableObject
{
    [SerializeField] private float _speed = 5.0f;
}
