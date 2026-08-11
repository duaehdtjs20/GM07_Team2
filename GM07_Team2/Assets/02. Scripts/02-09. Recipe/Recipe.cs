using System;
using UnityEngine;

[Serializable]
public class Recipe
{
    private RecipeData _data;
    private ERecipeGrade _grade = ERecipeGrade.Normal;
    private bool _unlocked = false;

    public RecipeData Data => _data;
    public int RecipeId => Data.RecipeId;
    public ERecipeGrade Grade => _grade;
    public bool Unlocked => _unlocked;

    public Recipe(RecipeData data)
    {
        _data = data;
    }

    public Recipe(RecipeSaveData saveData)
    {
        if (RecipeManager.Instance.TryGetRecipeId(saveData.RecipeId, out RecipeData data))
        {
            _data = data;
            _grade = saveData.Grade;
            _unlocked = saveData.Unlocked;
        }
    }

    public bool Unlock()
    {
        if (CurrencyManager.Instance == null || RecipeManager.Instance == null)
        {
            return false;
        }

        if (!RecipeManager.Instance.CanUnlock(this))
        {
            return false;
        }

        if (CurrencyManager.Instance.TrySpendMoney(_data.Cost, ECurrencyTransactionType.OtherExpense))
        {
            _grade = (ERecipeGrade)UnityEngine.Random.Range(0, (int)ERecipeGrade.Size);
            _unlocked = true;
            return true;
        }
        else
        {
            return false;
        }
    }


    // 게임 시작 시 기본 제공되는 레시피를 위한 강제 해금 (돈 소모 없음)
    public void ForceUnlock()
    {
        _grade = (ERecipeGrade)UnityEngine.Random.Range(0, (int)ERecipeGrade.Size);
        _unlocked = true;
    }



    public void ApplySaveData(RecipeSaveData data)
    {
        _grade = data.Grade;
        _unlocked = data.Unlocked;
    }

    public RecipeSaveData Save()
    {
        return new RecipeSaveData(RecipeId, Grade, Unlocked);
    }
}