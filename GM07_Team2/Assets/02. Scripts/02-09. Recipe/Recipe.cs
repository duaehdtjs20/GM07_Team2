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
        if(RecipeManager.Instance.TryGetRecipeId(saveData.RecipeId, out RecipeData data))
        {
            _data = data;
            _grade = saveData.Grade;
            _unlocked = saveData.Unlocked;
        }
    }
    public void Unlock()
    {
        _grade = (ERecipeGrade)UnityEngine.Random.Range(0, (int)ERecipeGrade.Size);
        _unlocked = true;
    }
    public RecipeSaveData Save()
    {
        return new RecipeSaveData(RecipeId, Grade, Unlocked);
    }
}
