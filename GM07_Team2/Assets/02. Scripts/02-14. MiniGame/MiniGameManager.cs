using GM07.Order;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : MonoBehaviour 
{
    [SerializeField]
    private List<UI_MiniGameBase> _miniGames = new();

    private readonly List<UI_MiniGameBase> _playableMiniGames = new();
    private UI_MiniGameBase _lastMiniGame;
    private void Start()
    {
        CloseAllMiniGames();
    }
    public void PlayRandomGame(OrderData order, Action<EQuality> onCompleted)
    {
        if(order == null)
        {
            return;
        }

        _playableMiniGames.Clear();
        foreach(UI_MiniGameBase miniGame in _miniGames)
        {
            if(miniGame != _lastMiniGame)
            {
                _playableMiniGames.Add(miniGame);
            }
        }

        if(_playableMiniGames.Count == 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, _miniGames.Count);
            _miniGames[randomIndex].Open(order, onCompleted);
            return;
        }
        else
        {
            int randomIndex = UnityEngine.Random.Range(0, _playableMiniGames.Count);
            _lastMiniGame = _playableMiniGames[randomIndex];
            _lastMiniGame.Open(order, onCompleted);
        }
    }
    private void CloseAllMiniGames()
    {
        foreach(UI_MiniGameBase miniGameBase in _miniGames)
        {
            if(miniGameBase != null)
            {
                miniGameBase.gameObject.SetActive(false);
            }
        }
    }
}
