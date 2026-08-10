using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UI_RecipePanel : MonoBehaviour
{
    [SerializeField]
    private UI_RecipeView _recipeViewPrefab;
    [SerializeField]
    private Transform _ListRoot;
    [SerializeField]
    private UI_RecipeDetailView _recipeDetailView;

    private List<UI_RecipeView> _recipeViews;
    private void Awake()
    {
        _recipeViews = new List<UI_RecipeView>();
    }
    private void Start()
    {
        Init();
    }
    private void Init()
    {
        if (RecipeManager.Instance == null)
        {
            return;
        }

        int count = RecipeManager.Instance.Count;

        for (int i = 0; i < count; i++)
        {
            if (!RecipeManager.Instance.TryGetRecipeIndex(i, out Recipe recipe))
            {
                continue;
            }
            UI_RecipeView view = Instantiate(_recipeViewPrefab, _ListRoot);
            view.Bind(recipe, _recipeDetailView);
            view.Draw();
            _recipeViews.Add(view);
        }
    }
    private void OnClickExit()
    {
        gameObject.SetActive(false);
    }
}
