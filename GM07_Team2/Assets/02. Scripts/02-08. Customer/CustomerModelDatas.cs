using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "CustomerModelData", menuName = "Customer/CustomerModelDatas")]
public class CustomerModelDatas : ScriptableObject
{
    [SerializeField] private List<GameObject> _models;
    public IReadOnlyList<GameObject> Models => _models;
    public int Count => _models.Count;
}
