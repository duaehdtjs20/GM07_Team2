using System;

using UnityEngine;

[Serializable]
public class RecipeSaveData
{
    [SerializeField]
    private int _recipeId;
    [SerializeField]
    private ERecipeGrade _grade;
    [SerializeField]
    private bool _unlocked;

    public int RecipeId => _recipeId;
    public ERecipeGrade Grade => _grade;
    public bool Unlocked => _unlocked;

    public RecipeSaveData(int recipeId, ERecipeGrade grade, bool unlocked)
    {
        _recipeId = recipeId;
        _grade = grade;
        _unlocked = unlocked;
    }
}
