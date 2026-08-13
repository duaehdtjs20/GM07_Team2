using UnityEngine;

[CreateAssetMenu(fileName = "RecipeData", menuName = "Recipe/RecipeData")]
public class RecipeData : ScriptableObject
{
    [field: SerializeField]
    public int RecipeId { get; private set; }
    [field: SerializeField]
    public Sprite Icon { get; private set; }
    [field: SerializeField]
    public Sprite IngredientIcon { get; private set; }
    [field: SerializeField]
    public string Name { get; private set; } = "레시피 이름";
    [field: SerializeField]
    public int Cost { get; private set; } = 0; // 레시피 제작 비용
    [field: SerializeField]
    public int Price { get; private set; } = 0; // 판매 가격
    [field: SerializeField]
    public float CookingTime { get; private set; } = 1.0f; // 조리 시간
    [field: SerializeField]
    public EMenuGrade MenuGrade { get; private set; } = EMenuGrade.Low; // 메뉴 고정 등급 (하/중/상)
}