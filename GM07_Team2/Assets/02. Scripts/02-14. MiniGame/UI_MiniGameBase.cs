using GM07.Order;
using System;
using UnityEngine;

public abstract class UI_MiniGameBase : MonoBehaviour
{
    public abstract void Open(OrderData order, Action<EQuality> onCompleted);
}
