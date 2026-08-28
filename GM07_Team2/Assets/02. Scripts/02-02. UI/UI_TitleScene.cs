using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_TitleScene : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField]
    private Button _newGameButton;
    [SerializeField]
    private Button _continueButton;
    [SerializeField]
    private Button _exitButton;
    [Header("Loading")]
    [SerializeField]
    private GameObject _loadingPanel;

    private EffectBase _titleEffect;
    private bool _isLoading;
    private void Awake()
    {
        _newGameButton?.onClick.AddListener(OnClickNewGame);
        _continueButton?.onClick.AddListener(OnClickContinue);
        _exitButton?.onClick.AddListener(OnClickExit);
        _titleEffect = GetComponent<EffectBase>();
        _loadingPanel?.SetActive(false);
        RefreshButton();
    }
    private void Start()
    {
        _titleEffect?.Play();
    }
    private void OnDestroy()
    {
        _newGameButton?.onClick.RemoveListener(OnClickNewGame);
        _continueButton?.onClick.RemoveListener(OnClickContinue);
        _exitButton?.onClick.RemoveListener(OnClickExit);
    }

    private void RefreshButton()
    {
        if (_continueButton != null)
        {
            _continueButton.interactable = SaveManager.Instance.HasSaveData();
        }
    }

    private void OnClickNewGame()
    {
        StartGame(EGameStartMode.NewGame);
    }
    private void OnClickContinue()
    {
        StartGame(EGameStartMode.Continue);
    }

    private void StartGame(EGameStartMode startMode)
    {
        if (_isLoading) 
        { 
            return; 
        }
        GameSession.StartMode = startMode;
        StartCoroutine(LoadMainSceneCo());
    }

    private IEnumerator LoadMainSceneCo()
    {
        _isLoading = true;
        _loadingPanel?.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(ESceneName.Maptest.ToString());
        while (operation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
        }
        yield return null;
        operation.allowSceneActivation = true;
        while(!operation.isDone)
        {
            yield return null;
        }
    }

    private void OnClickExit()
    {
        Application.Quit();
    }
}
