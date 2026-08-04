using System;
using UnityEngine;

[Serializable]
public sealed class CustomerSpawnPeriod
{
    [SerializeField, Range(0f, 1f)]
    private float _startTime;
    [SerializeField, Range(0f, 1f)]
    private float _endTime;

    [SerializeField]
    private float _minInterval;
    [SerializeField]
    private float _maxInterval;

    private float StartTime => _startTime;
    private float EndTime => _endTime;
    private float MinInterval => _minInterval;
    private float MaxInterval => _maxInterval;

    public bool IsInPeriod(float hour)
    {
        return hour >= StartTime && hour <= EndTime;
    }
    public float GetRandomInterval()
    {
        return UnityEngine.Random.Range(MinInterval, MaxInterval);
    }
}
