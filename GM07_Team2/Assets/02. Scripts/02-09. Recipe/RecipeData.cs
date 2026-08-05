using System;

using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "RecipeData", menuName = "Recipe/RecipeData")]
public class RecipeData : ScriptableObject
{
    [field: SerializeField]
    public Sprite Icon { get; private set; }

    [field: SerializeField]
    public string Name { get; private set; } = "레시피 이름";

    [field: SerializeField]
    public int Cost { get; private set; } = 0; // 레시피 제작 비용

    [field: SerializeField]
    public int Price { get; private set; } = 0; // 판매 가격

    [field: SerializeField]
    public ERecipeGrade Grade { get; private set; } = ERecipeGrade.Normal; // 레시피 등급

    [field: SerializeField]
    public bool Unlocked { get; private set; } = false; // 해금 여부

    [field: SerializeField]
    public float CookingTime { get; private set; } = 1.0f; // 요리 하는 데 걸리는 시간
    public void Unlock()
    {
        Grade = (ERecipeGrade)UnityEngine.Random.Range(0, (int)ERecipeGrade.Size);
        Unlocked = true;
    }
}
