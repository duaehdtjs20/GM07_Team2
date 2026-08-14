using GM07.Order;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SushiDropGame : MonoBehaviour
{
    private enum EDropState
    {
        None,
        Moving,
        Falling,
        Completed,
    }
    [Header("Play Area")]
    [SerializeField]
    private RectTransform _playArea;
    [SerializeField]
    private RectTransform _spawnPoint;
    [SerializeField]
    private RectTransform _floor;
    [SerializeField]
    private UI_IngredientDrop _ingredientPrefab;
    [Header("Ingredient Sprites")]
    [SerializeField]
    private Sprite _riceSprite;
    [SerializeField]
    private Sprite _wasabiSprite;
    [Header("Input")]
    [SerializeField]
    private Button _dropButton;
    [Header("Movement")]
    [SerializeField]
    private float _horizontalSpeed;
    [SerializeField]
    private float _fallSpeed;
    [SerializeField]
    private float _minimumOverlapRatio;
    [Header("Timer")]
    [SerializeField]
    private TMP_Text _timer;
    [SerializeField]
    private float _timeLimit;
    [SerializeField]
    private float _completeDuration;
    [Header("Quality")]
    [Range(0f, 1f)]
    [SerializeField]
    private float _greatOverlapRatio;
    [Range(0f, 1f)]
    [SerializeField]
    private float _goodOverlapRatio;
    [Header("Result Setting")]
    [SerializeField]
    private UI_MiniGameResult _resultUI;

    private List<Sprite> _ingredientSprites = new();
    private OrderData _order;
    private UI_IngredientDrop _currentIngredient;
    private Action<EQuality> _onCompleted;
    private EDropState _state;
    private Coroutine _gameCoroutine;
    private Coroutine _completeCoroutine;
    private int _ingredientIndex;
    private int _horizontalDirection = 1;

    private float _remainingTime;
    private float _totalOverlapRatio;
}
