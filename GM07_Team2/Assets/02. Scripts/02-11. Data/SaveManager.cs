using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

using UnityEngine;

public class SaveManager : MonoBehaviourSingleton<SaveManager>
{
    private string _path = "SaveData.json";
    public void Save()
    {
        var restaurant = FindFirstObjectByType<Restaurant>();
        var flow = FindFirstObjectByType<GameFlowManager>();
        if (restaurant == null)
        {
            Debug.LogWarning("Restaurant를 찾지 못함");
            return;
        }
        if (flow == null)
        {
            Debug.LogWarning("GameFlowManager를 찾지 못함");
            return;
        }
        if (RecipeManager.Instance == null)
        {
            Debug.LogWarning("RecipeManager를 찾지 못함");
            return;
        }
        if (CurrencyManager.Instance == null)
        {
            Debug.LogWarning("CurrencyManager를 찾지 못함");
            return;
        }
        SaveData saveData = new SaveData
        (
            RecipeManager.Instance.Recipes,
            restaurant.Staffs,
            restaurant.Upgrade,
            CurrencyManager.Instance.Money,
            flow.CurrentDay
        );

        string text = JsonUtility.ToJson(saveData);
        Debug.Log(text);
        File.WriteAllText(Application.persistentDataPath + _path, text);
    }
    public SaveData Load()
    {
        string text = File.ReadAllText(Application.persistentDataPath + _path);
        SaveData saveData = JsonUtility.FromJson<SaveData>(text);
        return saveData;
    }
}
