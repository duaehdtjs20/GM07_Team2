using GM07.Order;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SushiDropGame : UI_MiniGameBase
{
    private enum EDropState
    {
        None,
        Moving,
        Falling,
        Completed,
    }
    [Header("Order")]
    [SerializeField]
    private TMP_Text _nemuName;
    [SerializeField]
    private Image _ingredientIcon;
    [Header("Play Area")]
    [SerializeField]
    private RectTransform _playArea;
    [SerializeField]
    private float _spawnPointY;
    [SerializeField]
    private RectTransform _floor;
    [Header("Ingredient Object")]
    [SerializeField]
    private GameObject _ingredientLine;
    [SerializeField]
    private GameObject _riceObject;
    [SerializeField]
    private GameObject _wasabiObject;
    [SerializeField]
    private GameObject _fishObject;
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
    private float _greatScore;
    [Range(0f, 1f)]
    [SerializeField]
    private float _goodScore;
    [Header("Result Setting")]
    [SerializeField]
    private UI_MiniGameResult _resultUI;

    private readonly List<GameObject> _ingredientObjects = new();
    private readonly List<RectTransform> _stackedIngredients = new();

    private OrderData _order;
    private Action<EQuality> _onCompleted;
    private GameObject _currentObject;
    private RectTransform _currentIngredient;

    private EDropState _state;
    private Coroutine _completeCoroutine;
    private int _ingredientIndex;
    private int _horizontalDirection = 1;
    private float _remainingTime;
    private float _totalAlignmentScore = 0;

    #region Init
    private void OnEnable()
    {
        if (_dropButton != null)
        {
            _dropButton.onClick.AddListener(DropCurrentIngredient);
        }
        if (_completeCoroutine != null)
        {
            StopCoroutine(_completeCoroutine);
            _completeCoroutine = null;
        }
    }
    private void OnDisable()
    {
        if (_dropButton != null)
        {
            _dropButton.onClick.RemoveListener(DropCurrentIngredient);
        }
        if (_completeCoroutine != null)
        {
            StopCoroutine(_completeCoroutine);
            _completeCoroutine = null;
        }
    }
    #endregion
    private void Update()
    {
        if (_state == EDropState.None || _state == EDropState.Completed)
        {
            return;
        }

        UpdateTimer();

        if (_state == EDropState.Moving)
        {
            MoveIngredient();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                DropCurrentIngredient();
            }
        }
        else if (_state == EDropState.Falling)
        {
            FallIngredient();
        }
    }
    public override void Open(OrderData order, Action<EQuality> onCompleted)
    {
        if (order == null || order.Recipe == null || order.Recipe.Data == null)
        {
            onCompleted?.Invoke(EQuality.Fail);
            return;
        }
        ClearGame();
        _order = order;
        _onCompleted = onCompleted;
        _nemuName.text = order.Recipe.Data.Name;
        _ingredientIcon.sprite = order.Recipe.Data.IngredientIcon;
        _remainingTime = _timeLimit;
        _ingredientIndex = 0;
        _horizontalDirection = 1;
        _state = EDropState.None;

        RefeshFishSprite();
        CreateIngredientOrder();

        gameObject.SetActive(true);
        if (_dropButton != null)
        {
            _dropButton.interactable = true;
        }

        RefreshTimer();
        SpawnNextIngredient();
    }
    private void CreateIngredientOrder()
    {
        _ingredientObjects.Clear();
        _ingredientObjects.Add(_riceObject);
        _ingredientObjects.Add(_wasabiObject);
        _ingredientObjects.Add(_fishObject);
    }
    private void RefeshFishSprite()
    {
        if (_order == null)
        {
            return;
        }
        if (_fishObject.TryGetComponent<Image>(out Image fishImage))
        {
            fishImage.sprite = _order.Recipe.Data.IngredientIcon;
        }
    }
    private void MoveIngredient()
    {
        if (_currentIngredient == null)
        {
            return;
        }
        Vector2 position = _currentIngredient.anchoredPosition;
        position.x += _horizontalDirection * _horizontalSpeed * Time.deltaTime;
        float halfWidth = _currentIngredient.rect.width * 0.5f;
        float leftLimit = -_playArea.rect.width * 0.5f + halfWidth;
        float rightLimit = _playArea.rect.width * 0.5f - halfWidth;
        if (position.x <= leftLimit)
        {
            position.x = leftLimit;
            _horizontalDirection = 1;
        } else if (position.x >= rightLimit)
        {
            position.x = rightLimit;
            _horizontalDirection = -1;
        }
        _currentIngredient.anchoredPosition = position;
    }
    private void DropCurrentIngredient()
    {
        if (_state != EDropState.Moving || _currentIngredient == null)
        {
            return;
        }

        _state = EDropState.Falling;
    }
    private void FallIngredient()
    {
        if (_currentIngredient == null)
        {
            return;
        }
        Vector2 position = _currentIngredient.anchoredPosition;
        position.y -= _fallSpeed * Time.deltaTime;
        _currentIngredient.anchoredPosition = position;
        if (_stackedIngredients.Count == 0)
        {
            CheckFloorLanding();
            if (_ingredientLine != null)
            {
                _ingredientLine.SetActive(false);
            }
        }
        else
        {
            CheckIngredientLanding();
        }
        CheckOutPlayerArea();
    }
    private void CheckFloorLanding()
    {
        float landingY = CalculateLandingY(_currentIngredient, _floor);
        if (_currentIngredient.anchoredPosition.y > landingY)
        {
            return;
        }
        float overlapRatio = CalculateHorizontalOverlap(_currentIngredient, _floor);
        if (overlapRatio < _minimumOverlapRatio)
        {
            return;
        }
        SetLandingPosition(landingY);
        float alignmetnScore = CalculateCenterAlignment(_currentIngredient, _floor);
        SettleCurrentIngredient(alignmetnScore);
    }
    private void CheckIngredientLanding()
    {
        RectTransform previousIngredient = _stackedIngredients[_stackedIngredients.Count - 1];
        float landingY = CalculateLandingY(_currentIngredient, previousIngredient);
        if (_currentIngredient.anchoredPosition.y > landingY)
        {
            return;
        }
        float overlapRatio = CalculateHorizontalOverlap(_currentIngredient, previousIngredient);
        if (overlapRatio < _minimumOverlapRatio)
        {
            return;
        }
        SetLandingPosition(landingY);
        float alignmetnScore = CalculateCenterAlignment(_currentIngredient, previousIngredient);
        SettleCurrentIngredient(alignmetnScore);
    }
    private void SetLandingPosition(float landingY)
    {
        Vector2 position = _currentIngredient.anchoredPosition;
        position.y = landingY;
        _currentIngredient.anchoredPosition = position;
    }
    private void SettleCurrentIngredient(float score)
    {
        _totalAlignmentScore += score;
        _stackedIngredients.Add(_currentIngredient);
        _currentIngredient = null;
        _currentObject = null;
        _ingredientIndex++;
        if (_ingredientIndex >= _ingredientObjects.Count)
        {
            EQuality quality = CalculateQuality(out float totalScore);
            CompleteGame(quality, totalScore);
            return;
        }
        SpawnNextIngredient();
    }
    private void CheckOutPlayerArea()
    {
        if(_currentIngredient == null)
        {
            return;
        }
        float failY = -_playArea.rect.height * 0.5f - _currentIngredient.rect.height;
        if(_currentIngredient.anchoredPosition.y < failY)
        {
            CompleteGame(EQuality.Fail);
        }
    }
    private void SpawnNextIngredient()
    {
        if (_ingredientIndex < 0 || _ingredientIndex >= _ingredientObjects.Count)
        {
            EQuality quality = CalculateQuality(out float totalScore);
            CompleteGame(quality, totalScore);
            return;
        }
        GameObject nextIngredient = _ingredientObjects[_ingredientIndex];
        if (!nextIngredient.TryGetComponent(out RectTransform nextRect))
        {
            CompleteGame(EQuality.Fail);
            return;
        }
        _currentObject = nextIngredient;
        _currentIngredient = nextRect;

        float halfWidth = _currentIngredient.rect.width * 0.5f;
        float minX = -_playArea.rect.width * 0.5f + halfWidth;
        float maxX = _playArea.rect.width * 0.5f - halfWidth;
        float randomX = UnityEngine.Random.Range(minX, maxX);
        _currentIngredient.anchoredPosition = new Vector2(randomX, _spawnPointY);
        _currentObject.SetActive(true);
        _horizontalDirection = UnityEngine.Random.value < 0.5f ? 1 : -1;
        _state = EDropState.Moving;
    }
    #region Timer
    private void UpdateTimer()
    {
        _remainingTime -= Time.deltaTime;
        RefreshTimer();
        if(_remainingTime <= 0)
        {
            CompleteGame(EQuality.Fail);
        }
    }
    private void RefreshTimer()
    {
        if (_timer != null)
        {
            _timer.text = Mathf.CeilToInt(_remainingTime).ToString();
        }
    }
    #endregion
    private void CompleteGame(EQuality quality, float score = 0)
    {
        _state = EDropState.Completed;
        if (_dropButton != null)
        {
            _dropButton.interactable = false;
        }
        if(_completeCoroutine != null)
        {
            StopCoroutine(_completeCoroutine);
            _completeCoroutine = null;
        }
        _completeCoroutine = StartCoroutine(CompleteCoroutine(quality, score, GetStaffQualityBonus()));
    }
    private IEnumerator CompleteCoroutine(EQuality quality, float score, float staffBonus)
    {
        if (_resultUI != null)
        {
            _resultUI.ApplyResult(quality, score, staffBonus);
            _resultUI.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(_completeDuration);
        if(_resultUI != null)
        {
            _resultUI.gameObject.SetActive(false);
        }
        _completeCoroutine = null;
        Action<EQuality> callback = _onCompleted;
        _onCompleted = null;
        callback?.Invoke(quality);
        gameObject.SetActive(false);
    }
    private void ClearGame()
    {
        if (_completeCoroutine != null)
        {
            StopCoroutine(_completeCoroutine);
            _completeCoroutine = null;
        }
        _state = EDropState.None;
        _currentIngredient = null;
        _currentObject = null;
        _totalAlignmentScore = 0f;
        _stackedIngredients.Clear();
        _ingredientObjects.Clear();

        if (_ingredientLine != null)
        {
            _ingredientLine.SetActive(true);
        }

        ResetIngredient(_riceObject);
        ResetIngredient(_wasabiObject);
        ResetIngredient(_fishObject);

        if (_resultUI != null)
        {
            _resultUI.gameObject.SetActive(false);
        }
    }
    private void ResetIngredient(GameObject ingredient)
    {
        if (ingredient == null)
        {
            return;
        }
        ingredient.SetActive(false);
        if (ingredient.TryGetComponent(out RectTransform rect))
        {
            rect.anchoredPosition = new Vector2(0, _spawnPointY);
        }
    }
    #region Calculate
    private float CalculateHorizontalOverlap(RectTransform ingredient, RectTransform floor)
    {
        float ingredientLeft = ingredient.anchoredPosition.x - ingredient.rect.width / 2;
        float ingredientRight = ingredient.anchoredPosition.x + ingredient.rect.width / 2;
        float floorLeft = floor.anchoredPosition.x - floor.rect.width / 2;
        float floorRight = floor.anchoredPosition.x + floor.rect.width / 2;
        float overlap = Mathf.Min(ingredientRight, floorRight) - Mathf.Max(ingredientLeft, floorLeft);
        float comparisonWidth = Mathf.Min(ingredient.rect.width, floor.rect.width);
        if (comparisonWidth <= 0)
        {
            return 0;
        }
        return Mathf.Clamp01(overlap / comparisonWidth);
    }
    private EQuality CalculateQuality(out float score)
    {
        int alignmentCount = _stackedIngredients.Count;
        if(alignmentCount <= 0)
        {
            score = 0;
            return EQuality.Normal;
        }
        float averageScore = _totalAlignmentScore/alignmentCount;
        float staffBonusScore = GetStaffQualityBonus();
        float totalScore = averageScore + staffBonusScore;
        if (totalScore >= _greatScore)
        {
            score = totalScore;
            return EQuality.Great;
        }
        if (totalScore >= _goodScore)
        {
            score = totalScore;
            return EQuality.Good;
        }
        score = totalScore;
        return EQuality.Normal;
    }
    private float CalculateLandingY(RectTransform current, RectTransform previous)
    {
        float previoutMiddleY = previous.anchoredPosition.y + (0.5f - previous.pivot.y) * previous.rect.height;
        return previoutMiddleY + current.pivot.y * current.rect.height;
    }
    private float CalculateCenterAlignment(RectTransform current, RectTransform previous)
    {
        float centerDistance = Mathf.Abs(current.anchoredPosition.x-previous.anchoredPosition.x);
        float alignmentScore = 1 - Mathf.Clamp01(centerDistance / 100);
        return alignmentScore;
    }
    private float GetStaffQualityBonus()
    {
        if (_order == null ||
        _order.Staff == null)
        {
            return 0f;
        }

        return _order.Staff.QualityBonus;
    }
    #endregion
}

