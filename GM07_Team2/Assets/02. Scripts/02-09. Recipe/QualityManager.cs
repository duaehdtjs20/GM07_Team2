using GM07.Order;
using UnityEngine;

public class QualityManager : MonoBehaviourSingleton<QualityManager>
{
    [SerializeField]
    private float[] _baseWeights = new float[4] { 10f, 50f, 30f, 10f }; // Fail, Normal, Good, Great 순서

    [SerializeField]
    private float[] _priceMultipliers = new float[4] { 1.0f, 1.1f, 1.25f, 1.5f }; // Fail, Normal, Good, Great 순서

    // 요리사 등급 시스템 연동 전까지는 고정값 사용
    private int _chefLevel = 0;

    // 조리 완료 시점에 호출 — 가중치 기반으로 완성도 하나를 뽑아 반환
    public EQuality RollQuality()
    {
        float[] weights = GetWeights(_chefLevel);

        float total = 0f;
        foreach (float w in weights)
        {
            total += w;
        }

        float rand = Random.Range(0f, total);
        float cumulative = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            cumulative += weights[i];
            if (rand <= cumulative)
            {
                return (EQuality)i;
            }
        }
        return EQuality.Normal;
    }

    // 완성도에 해당하는 판매가 배율 반환
    public float GetPriceMultiplier(EQuality quality)
    {
        int index = (int)quality;
        if (index < 0 || index >= _priceMultipliers.Length)
        {
            return 1.0f;
        }
        return _priceMultipliers[index];
    }

    // TODO: 요리사 등급 시스템 연동 시, chefLevel에 따라 상위 단계(Good/Great) 가중치를 높이도록 여기를 확장
    private float[] GetWeights(int chefLevel)
    {
        return _baseWeights;
    }
}