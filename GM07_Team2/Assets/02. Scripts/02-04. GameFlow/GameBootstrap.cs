using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField]
    private GameFlowManager _gameFlowManager;
    [SerializeField]
    private Restaurant _restaurant;

    private void Start()
    {
        if(SaveManager.Instance == null ||
            CurrencyManager.Instance == null ||
            RecipeManager.Instance == null ||
            _gameFlowManager == null ||
            _restaurant == null)
        {
            Debug.LogWarning("초기화실패");
            return;
        }

        if(GameSession.StartMode == EGameStartMode.NewGame)
        {
            InitNewGame();
        }
        else
        {
            InitSaveData(SaveManager.Instance.Load());
        }
    }

    private void InitNewGame()
    {
        CurrencyManager.Instance.InitNewGame();
        RecipeManager.Instance.InitNewGame();
        _gameFlowManager.InitNewGame();
        //_restaurant.InitNewGame();
    }

    private void InitSaveData(SaveData saveData)
    {
        if(saveData== null)
        {
            InitNewGame();
            return;
        }

        CurrencyManager.Instance.InitMoney(saveData.Money);
        RecipeManager.Instance.InitSaveData(saveData.Recipes);
        _gameFlowManager.InitDay(saveData.Day);
        //_restaurant
    }
}
