using System;

using UnityEngine;

public class RecipeManager : MonoBehaviourSingleton<RecipeManager>
{
    [SerializeField]
    private RecipeDataBase _dataBase;

    public int Count => _dataBase.Count;

    public bool TryGetRecipeIndex(int index, out RecipeData recipe)
    {
        // index 범위 제한
        if (index < 0 || index >= _dataBase.Count)
        {
            recipe = null;
            return false;
        }

        recipe = _dataBase.RecipeDatas[index];
        return recipe != null;
    }
}
