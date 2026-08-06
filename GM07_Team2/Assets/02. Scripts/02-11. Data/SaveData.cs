using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class SaveData
{
    [SerializeField]
    private List<RecipeSaveData> _recipes = new List<RecipeSaveData>();
    [SerializeField]
    private List<Staff> _staffs = new List<Staff>();
    [SerializeField]
    private int _restaurantUpgrade;
    [SerializeField]
    private int _money;
    [SerializeField]
    private int _day;

    public IReadOnlyList<RecipeSaveData> Recipes => _recipes;
    public IReadOnlyList<Staff> Staffs => _staffs;
    public int RestaurantUpgrade => _restaurantUpgrade;
    public int Money => _money;
    public int Day => _day;

    public SaveData(IReadOnlyList<Recipe> recipes, IReadOnlyList<Staff> staffs, int restaurantUpgrade, int money, int day)
    {
        foreach (var recipe in recipes)
        {
            _recipes.Add(new RecipeSaveData(recipe.RecipeId, recipe.Grade, recipe.Unlocked));
        }
        foreach (var staff in staffs)
        {
            _staffs.Add(new Staff(staff.Name, staff.CookSpeed, staff.Upgrade));
        }
        _restaurantUpgrade = restaurantUpgrade;
        _money = money;
        _day = day;
    }
}
