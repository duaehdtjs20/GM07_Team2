using System;
using System.Collections;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField]
    private float _openDuration;

    private Coroutine _openCoroutine;

    public EGameState GameState { get; private set; }
    public float OpenDuration => _openDuration;
    public float RemainingTime { get; private set; }
    public int CurrentDay { get; private set; }

    public Action<EGameState> OnGameStateChanged;
    public Action<float> OnRemainingTimeChanged;
    public Action<int> OnDayChanged;

    private void Start()
    {
        CurrentDay = 1;
        RemainingTime = _openDuration;
        GameState = EGameState.Preparing;

        OnDayChanged?.Invoke(CurrentDay);
        OnRemainingTimeChanged?.Invoke(RemainingTime);
        OnGameStateChanged?.Invoke(GameState);
    }
    private void OnDisable()
    {
        StopOpenCo();
    }
    private void StartOpen()
    {
        if(_openCoroutine != null)
        {
            return;
        }

        RemainingTime = _openDuration;
        SetGameState(EGameState.Open);

        _openCoroutine = StartCoroutine(OpenCo());
    }

    private IEnumerator OpenCo()
    {
        while (RemainingTime > 0f)
        {
            RemainingTime -= Time.deltaTime;
            RemainingTime = Mathf.Max(0, RemainingTime);

            OnRemainingTimeChanged?.Invoke(RemainingTime);

            yield return null;
        }

        _openCoroutine = null;

        SetGameState(EGameState.ClosingWait);
        //남은 손님 기다리기
        SetGameState(EGameState.Close);
    }

    private void StopOpenCo()
    {
        if(_openCoroutine == null)
        {
            return;
        }
        StopCoroutine(_openCoroutine);
        _openCoroutine = null;
    }

    private void SetGameState(EGameState newState)
    {
        if(GameState == newState)
        {
            return;
        }

        GameState = newState;
        OnGameStateChanged?.Invoke(GameState);
    }

    public void SetOpenDuration(float openDuration)
    {
        if (openDuration <= 0f)
        {
            return;
        }

        _openDuration = openDuration;

        if (GameState != EGameState.Preparing)
        {
            return;
        }

        RemainingTime = _openDuration;
        OnRemainingTimeChanged?.Invoke(RemainingTime);
    }

    public void OnClickOpen()
    {
        if (GameState != EGameState.Preparing)
        {
            return;
        }

        StartOpen();
    }

    public void OnClickNextDay()
    {
        if(GameState != EGameState.Close)
        {
            return;
        }

        CurrentDay++;
        RemainingTime = _openDuration;
        OnDayChanged?.Invoke(CurrentDay);
        OnRemainingTimeChanged?.Invoke(RemainingTime);

        SetGameState(EGameState.Preparing);
    }
}
