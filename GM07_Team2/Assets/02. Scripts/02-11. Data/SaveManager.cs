using System;
using System.IO;

using UnityEngine;
using UnityEngine.Audio;

public class SaveManager : MonoBehaviourSingleton<SaveManager>
{
    private string _path = "SaveData.json";
    private string _savePath => Path.Combine(Application.persistentDataPath, _path);
    private string _optionPath => Path.Combine(Application.persistentDataPath, "OptionData.json");
    public bool HasSaveData()
    {
        return File.Exists(_savePath);
    }
    public bool HasOptionData()
    {
        return File.Exists(_optionPath);
    }
    public string SavePath => _savePath;
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
            restaurant.Level,
            CurrencyManager.Instance.Money,
            flow.CurrentDay
        );

        try
        {
            string json = JsonUtility.ToJson(saveData, true);
            string temPath = _savePath + ".tmp";

            File.WriteAllText(temPath, json);
            File.Delete(_savePath);
            File.Move(temPath, _savePath);

            Debug.Log("게임 저장 완료");
        }
        catch(Exception ex)
        {
            Debug.LogWarning($"게임 저장 실패{ex}");
        }
    }
    public SaveData Load()
    {
        if (!HasSaveData())
        {
            return null;
        }

        try
        {
            string text = File.ReadAllText(_savePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(text);
            return saveData;
        }
        catch
        {
            Debug.LogWarning("게임 불러오기 실패");
            return null;
        }
    }
    public void SaveOption()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager를 찾지 못함");
            return;
        }

        AudioMixer mixer = AudioManager.Instance.Mixer;

        float masterVolume = 0.0f;
        float bgmVolume = 0.0f;
        float sfxVolume = 0.0f;

        mixer.GetFloat("Master", out masterVolume);
        mixer.GetFloat("BGM", out bgmVolume);
        mixer.GetFloat("SFX", out sfxVolume);

        OptionData optionData = new OptionData(masterVolume, bgmVolume, sfxVolume);

        try
        {
            string json = JsonUtility.ToJson(optionData, true);
            string temPath = _optionPath + ".tmp";

            File.WriteAllText(temPath, json);
            File.Delete(_optionPath);
            File.Move(temPath, _optionPath);

            Debug.Log("옵션 저장 완료");
        }
        catch(Exception ex)
        {
            Debug.LogWarning($"게임 저장 실패{ex}");
        }
    }
    public OptionData LoadOption()
    {
        if (!HasOptionData())
        {
            return null;
        }

        try
        {
            string text = File.ReadAllText(_optionPath);
            OptionData optionData = JsonUtility.FromJson<OptionData>(text);
            return optionData;
        }
        catch
        {
            Debug.LogWarning("게임 불러오기 실패");
            return null;
        }
    }
    public bool DeleteSaveData()
    {
        try
        {
            if (File.Exists(_savePath)) File.Delete(_savePath);

            string tempPath = _savePath + ".tmp";
            if (File.Exists(tempPath)) File.Delete(tempPath);

            Debug.Log($"저장 데이터 삭제 완료: {_savePath}");
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"저장 데이터 삭제 실패: {exception.Message}");
            return false;
        }
    }
}
