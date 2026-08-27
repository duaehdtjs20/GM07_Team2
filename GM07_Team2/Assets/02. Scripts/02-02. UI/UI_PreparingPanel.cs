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
        OpenPanel(_recipePanel);
    }
    private void OpenStaffPanel()
    {
        OpenPanel(_staffPanel);
    }
    private void OpenStorePanel()
    {
        OpenPanel(_storePanel);
    }
    private void CloseRecipePanel()
    {
        ClosePanel(_recipePanel);
    }
    private void CloseStaffPanel()
    {
        ClosePanel(_staffPanel);
    }
    private void CloseStorePanel()
    {
        ClosePanel(_storePanel);
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
    private void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
        if (panel.TryGetComponent<OpenUpgradePanelEffect>(out OpenUpgradePanelEffect effect))
        {
            effect.Play();
        }
    }
    private void ClosePanel(GameObject panel)
    {
        if (panel.TryGetComponent<CloseUpgradePanelEffect>(out CloseUpgradePanelEffect effect))
        {
            effect.Play();
        }
    }
}
