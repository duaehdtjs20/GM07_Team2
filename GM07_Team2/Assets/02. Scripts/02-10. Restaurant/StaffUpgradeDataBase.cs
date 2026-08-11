using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="StaffUpgradeData",menuName ="Staff/StaffUpgradeData")]
public class StaffUpgradeDataBase : ScriptableObject
{
    [SerializeField]
    private List<StaffLevelData> _levelDataList = new();

    public int MaxLevel => _levelDataList.Count;

    public bool TryGetLevelData(int level, out StaffLevelData levelData)
    {
        int index = level - 1;
        if(_levelDataList == null || index<0 || index >= _levelDataList.Count)
        {
            levelData = null;
            return false;
        }

        levelData = _levelDataList[index];
        return true;
    }
}
