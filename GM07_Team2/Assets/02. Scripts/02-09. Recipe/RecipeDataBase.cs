using System;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "RecipeDataBase", menuName = "Recipe/RecipeDataBase")]
public class RecipeDataBase : ScriptableObject
{
    [SerializeField]
    private List<RecipeData> _recipeDatas = new List<RecipeData>();
    public IReadOnlyList<RecipeData> RecipeDatas => _recipeDatas;
    public int Count => _recipeDatas.Count;
}
