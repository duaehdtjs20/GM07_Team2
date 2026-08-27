using GM07.Order;
using System;
using UnityEngine;

public abstract class UI_MiniGameBase : MonoBehaviour
{
    public abstract void Open(OrderData order, Action<EQuality> onCompleted);

    protected Color GetGradeColor(EMenuGrade grade)
    {
        switch (grade)
        {
            case EMenuGrade.Low:
                return new Color(0.3f, 0.7f, 0.2f);
            case EMenuGrade.Mid:
                return new Color(1f, 0.85f, 0.3f);
            case EMenuGrade.High:
                return new Color(0.9f, 0.25f, 0.25f);
            default:
                return Color.white;
        }
    }
}
