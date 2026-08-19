using GM07.Order;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : MonoBehaviour 
{
    [SerializeField]
    private List<UI_MiniGameBase> _miniGames = new();

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
        int randomIndex = UnityEngine.Random.Range(0, _miniGames.Count);
        _miniGames[randomIndex].Open(order, onCompleted);
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
