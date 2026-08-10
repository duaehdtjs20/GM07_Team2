using UnityEngine;
using UnityEngine.UI;

public class UI_PreparingPanel : MonoBehaviour
{
    [Header("Recipe")]
    [SerializeField]
    private Button _recipeButton;
    [SerializeField]
    private GameObject _recipePanel;
    [SerializeField]
    private Button _recipeExitButton;
    [Header("Staff")]
    [SerializeField]
    private Button _staffButton;
    [SerializeField]
    private GameObject _staffPanel;
    [SerializeField]
    private Button _staffExitButton;
    [Header("Store")]
    [SerializeField]
    private Button _storeButton;
    [SerializeField]
    private GameObject _storePanel;
    [SerializeField]
    private Button _storeExitButton;

    private void Awake()
    {
        _recipeButton?.onClick.AddListener(OpenRecipePanel);
        _staffButton?.onClick.AddListener(OpenStaffPanel);
        _storeButton?.onClick.AddListener(OpenStorePanel);

        _recipeExitButton?.onClick.AddListener(CloseRecipePanel);
        _staffExitButton?.onClick.AddListener(CloseStaffPanel);
        _storeExitButton?.onClick.AddListener(CloseStorePanel);

        CloseAllPanels();
    }

    private void OnEnable()
    {
        CloseAllPanels();
    }

    private void OnDisable()
    {
        CloseAllPanels();
    }

    private void OnDestroy()
    {
        _recipeButton?.onClick.RemoveListener(OpenRecipePanel);
        _staffButton?.onClick.RemoveListener(OpenStaffPanel);
        _storeButton?.onClick.RemoveListener(OpenStorePanel);

        _recipeExitButton?.onClick.RemoveListener(CloseRecipePanel);
        _staffExitButton?.onClick.RemoveListener(CloseStaffPanel);
        _storeExitButton?.onClick.RemoveListener(CloseStorePanel);
    }

    private void OpenRecipePanel()
    {
        SetPanelActive(_recipePanel, true);
    }

    private void OpenStaffPanel()
    {
        SetPanelActive(_staffPanel, true);
    }

    private void OpenStorePanel()
    {
        SetPanelActive(_storePanel, true);
    }

    private void CloseRecipePanel()
    {
        SetPanelActive(_recipePanel, false);
    }

    private void CloseStaffPanel()
    {
        SetPanelActive(_staffPanel, false);
    }

    private void CloseStorePanel()
    {
        SetPanelActive(_storePanel, false);
    }

    private void CloseAllPanels()
    {
        SetPanelActive(_recipePanel, false);
        SetPanelActive(_staffPanel, false);
        SetPanelActive(_storePanel, false);
    }

    private static void SetPanelActive(GameObject panel, bool isActive)
    {
        if (panel != null)
        {
            panel.SetActive(isActive);
        }
    }
}
