using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RecipePanel : MonoBehaviour
{
    [Header("Recipe List")]
    [SerializeField]
    private UI_RecipeView _recipeViewPrefab;
    [SerializeField]
    private Transform _ListRoot;
    [SerializeField]
    private UI_RecipeDetailView _recipeDetailView;
    [Header("Page")]
    [SerializeField]
    private Button _previousButton;
    [SerializeField]
    private Button _nextButton;
    [SerializeField]
    private TMP_Text _pageText;

    private List<UI_RecipeView> _recipeViews = new();
    private int _currentPage;
    private const int ItemInPage = 6;
    private int PageCount
    {
        get
        {
            return Mathf.CeilToInt(RecipeManager.Instance.Count / (float)ItemInPage);
        }
    }
    private void Awake()
    {
        _previousButton?.onClick.AddListener(OnClickPreviousPage);
        _nextButton?.onClick.AddListener(OnClickNextPage);
    }
    private void OnEnable()
    {
        _currentPage = Mathf.Clamp(_currentPage,0,PageCount-1);
        RefreshPage();
    }
    private void OnDestroy()
    {
        _previousButton?.onClick.RemoveListener(OnClickPreviousPage);
        _nextButton?.onClick.RemoveListener(OnClickNextPage);
    }
    public void RefreshPage()
    {
        ClearRecipeView();

        int startIndex = _currentPage * ItemInPage;
        int endIndex = Mathf.Min(startIndex + ItemInPage, RecipeManager.Instance.Count);

        for(int i = startIndex; i < endIndex; i++)
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

        if(_previousButton != null)
        {
            _previousButton.interactable = _currentPage > 0;
        }
        if(_nextButton != null)
        {
            _nextButton.interactable = _currentPage < PageCount - 1;
        }
        if(_pageText != null)
        {
            _pageText.text = (_currentPage + 1).ToString();
        }
    }
    private void ClearRecipeView()
    {
        foreach(UI_RecipeView view in _recipeViews)
        {
            Destroy(view.gameObject);
        }
        _recipeViews.Clear();
    }
    private void OnClickPreviousPage()
    {
        if (_currentPage <= 0)
        {
            return;
        }
        _currentPage--;
        RefreshPage();
    }
    private void OnClickNextPage()
    {
        if(_currentPage >= PageCount - 1)
        {
            return;
        }
        _currentPage++;
        RefreshPage();
    }
}
