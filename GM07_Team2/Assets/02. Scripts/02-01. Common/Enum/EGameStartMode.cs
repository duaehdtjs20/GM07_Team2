using UnityEngine;

public enum EGameStartMode
{
    NewGame,
    Continue,
}

public static class GameSession
{
    public static EGameStartMode StartMode { get; set; } = EGameStartMode.NewGame;
}
