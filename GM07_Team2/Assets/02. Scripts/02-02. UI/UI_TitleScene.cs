using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_TitleScene : MonoBehaviour
{
    [SerializeField]
    private Button _newGameButton;
    [SerializeField]
    private Button _continueButton;
    [SerializeField]
    private Button _exitButton;

    private EffectBase _titleEffect;
    private void Awake()
    {
        _newGameButton?.onClick.AddListener(OnClickNewGame);
        _continueButton?.onClick.AddListener(OnClickContinue);
        _exitButton?.onClick.AddListener(OnClickExit);
        _titleEffect = GetComponent<EffectBase>();
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
        GameSession.StartMode = EGameStartMode.NewGame;
        SceneManager.LoadScene(ESceneName.Maptest.ToString());
    }
    private void OnClickContinue()
    {
        GameSession.StartMode = EGameStartMode.Continue;
        SceneManager.LoadScene(ESceneName.Maptest.ToString());
    }

    private void OnClickExit()
    {
        Application.Quit();
    }
}
