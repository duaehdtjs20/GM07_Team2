using System;

using UnityEngine;

[Serializable]
public class RecipeSaveData
{
    [SerializeField]
    private int _recipeId;
    
    [SerializeField]
    private bool _unlocked;

    public int RecipeId => _recipeId;
   
    public bool Unlocked => _unlocked;

    public RecipeSaveData(int recipeId, bool unlocked)
    {
        _recipeId = recipeId;
        _unlocked = unlocked;
    }
}
