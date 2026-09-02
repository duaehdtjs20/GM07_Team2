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
    [SerializeField]
    private Button _settingButton;
    [Header("Setting Panel")]
    [SerializeField]
    private GameObject _settingPanel;
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
        _settingButton?.onClick.AddListener(OnClickSetting);
        _titleEffect = GetComponent<EffectBase>();
        _loadingPanel?.SetActive(false);
    }
    private void Start()
    {
        RefreshButton();
        AudioManager.Instance?.PlayBGM(EAudioType.Title);
        _titleEffect?.Play();
        _settingPanel?.SetActive(false);
    }
    private void OnDestroy()
    {
        _newGameButton?.onClick.RemoveListener(OnClickNewGame);
        _continueButton?.onClick.RemoveListener(OnClickContinue);
        _exitButton?.onClick.RemoveListener(OnClickExit);
        _settingButton?.onClick.RemoveListener(OnClickSetting);
    }

    private void RefreshButton()
    {
        if (_continueButton != null)
        {
            _continueButton.interactable = SaveManager.Instance!=null && SaveManager.Instance.HasSaveData();
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
        _loadingPanel?.SetActive(true);
        GameSession.StartMode = startMode;
        StartCoroutine(LoadMainSceneCo());
    }

    private IEnumerator LoadMainSceneCo()
    {
        _isLoading = true;
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
    private void OnClickSetting()
    {
        _settingPanel?.SetActive(true);
    }
}
