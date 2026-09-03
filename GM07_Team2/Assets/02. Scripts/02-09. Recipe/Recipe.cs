using System;
using UnityEngine;

[Serializable]
public class Recipe
{
    private RecipeData _data;
    
    private bool _unlocked = false;

    public RecipeData Data => _data;
    public int RecipeId => Data.RecipeId;
    
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
           
            _unlocked = true;
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool CanUnlock()
    {
        if (RecipeManager.Instance == null || CurrencyManager.Instance == null)
        {
            return false;
        }
        return RecipeManager.Instance.CanUnlock(this, false) && CurrencyManager.Instance.CanSpendMoney(_data.Cost);
    }
    // 게임 시작 시 기본 제공되는 레시피를 위한 강제 해금 (돈 소모 없음)
    public void ForceUnlock(bool isNewGame)
    {
        if (isNewGame)
        {
           
        }
        _unlocked = true;
    }

    public void ApplySaveData(RecipeSaveData data)
    {
      
        _unlocked = data.Unlocked;
    }
}