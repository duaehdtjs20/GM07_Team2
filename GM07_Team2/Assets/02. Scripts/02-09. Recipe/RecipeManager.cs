using System;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviourSingleton<RecipeManager>
{
    [SerializeField]
    private RecipeDataBase _dataBase;
    private List<Recipe> _recipes;
    public IReadOnlyList<Recipe> Recipes => _recipes;
    public int Count => _recipes.Count;

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    public void InitNewGame()
    {
        Init();
        DefaultUnlock(true);
    }

    public void InitSaveData(IReadOnlyList<RecipeSaveData> saveRecipes)
    {
        Init();
        if (saveRecipes.Count == 0)
        {
            return;
        }
        foreach (RecipeSaveData saveData in saveRecipes)
        {
            for (int i = 0; i < _recipes.Count; i++)
            {
                if (_recipes[i].RecipeId != saveData.RecipeId)
                {
                    continue;
                }
                _recipes[i].ApplySaveData(saveData);
                break;
            }
        }
        DefaultUnlock(false);
    }
    private void DefaultUnlock(bool isNewGame)
    {
        // 기본 제공 레시피: 리스트 0번(하등급) 자동 해금
        if (_recipes == null || _recipes.Count == 0)
        {
            return;
        }
        _recipes[0].ForceUnlock(isNewGame);
    }

    private void Init()
    {
        _recipes = new List<Recipe>();
        if (_dataBase != null)
        {
            foreach (RecipeData data in _dataBase.RecipeDatas)
            {
                Recipe newRecipe = new Recipe(data);
                _recipes.Add(newRecipe);
            }
        }
    }

    public bool TryGetRecipeIndex(int index, out Recipe recipe)
    {
        if (index < 0 || index >= _recipes.Count)
        {
            recipe = null;
            return false;
        }
        recipe = _recipes[index];
        return recipe != null;
    }

    public bool TryGetRecipeId(int recipeId, out RecipeData recipeData)
    {
        foreach (RecipeData data in _dataBase.RecipeDatas)
        {
            if (data.RecipeId == recipeId)
            {
                recipeData = data;
                return true;
            }
        }
        recipeData = null;
        return false;
    }

    // 순차 해금 규칙: Low는 항상 해금 가능,
    // Mid는 Low 전체가 Unlocked여야 가능, High는 Mid 전체가 Unlocked여야 가능
    public bool CanUnlock(Recipe recipe)
    {
        if (recipe == null || recipe.Data == null)
        {
            return false;
        }

        EMenuGrade grade = recipe.Data.MenuGrade;
        if (grade == EMenuGrade.Low)
        {
            return true;
        }

        EMenuGrade previousGrade = grade - 1;
        foreach (Recipe r in _recipes)
        {
            if (r.Data.MenuGrade == previousGrade && !r.Unlocked)
            {
                UI_ToastMessage.Instance?.Show("이전 등급의 레시피를 모두 해금해야합니다");
                return false;
            }
        }
        return true;
    }
}