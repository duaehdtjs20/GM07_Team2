using GM07.Order;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_SushiSliceGame : UI_MiniGameBase
{
    private enum ESlice
    {
        None,
        Playing,
        Completed,
    }
    [Header("PlayerArea")]
    [SerializeField]
    private RectTransform _playArea;
    [SerializeField]
    private UI_SliceObject _sliceObjectPrefab;
    [Header("Order")]
    [SerializeField]
    private TMP_Text _menuName;
    [SerializeField]
    private Image _orderIcon;
    [SerializeField]
    private Image _ingredientIcon;
    [Header("Timer")]
    [SerializeField]
    private float _completeDuration;
    [Header("Ingredient")]
    [SerializeField]
    private Sprite _riceSprite;
    [SerializeField]
    private Sprite _wasabiSprite;
    [SerializeField]
    private List<Sprite> _junkSprites = new();
    [Header("Spawn Setting")]
    [SerializeField]
    private int _requiredSetCount = 2;
    [SerializeField]
    private int _distractorCount = 8;
    [SerializeField]
    private float _spawnInterval = 0.55f;
    [SerializeField]
    private float _minimumThrowSpeed = 550f;
    [SerializeField]
    private float _maximumThrowSpeed = 750f;
    [SerializeField]
    private float _maximumHorizontalSpeed = 180f;
    [SerializeField]
    private float _gravity = 900f;
    [SerializeField]
    private float _minimumRotationSpeed = -180f;
    [SerializeField]
    private float _maximumRotationSpeed = 180f;
    [SerializeField]
    private float _spawnBottomOffset = 70f;
    [Header("Score")]
    [SerializeField]
    private int _correctSliceScore = 20;
    [SerializeField]
    private int _wrongToppingPenalty = 10;
    [SerializeField]
    private int _junkPenalty = 15;
    [Header("Slice")]
    [SerializeField]
    private Image _sliceLinePrefab;
    [SerializeField]
    private float _sliceLineWidth;
    [SerializeField]
    private float _maximumSliceLineLength;
    [SerializeField]
    private float _minimumSliceDistance;
    [Header("Quality")]
    [SerializeField]
    private int _greatScore;
    [SerializeField]
    private int _goodScore;
    [SerializeField]
    private int _normalScore;
    [Header("Result")]
    [SerializeField]
    private UI_MiniGameResult _resultUI;

    private OrderData _order;
    private Action<EQuality> _onCompleted;
    private ESlice _state = ESlice.None;

    private readonly List<ESliceObjectType> _spawnPlan = new();
    private readonly List<UI_SliceObject> _activeObjects = new();
    private readonly List<UI_SliceObject> _sliceCheckBuffer = new();
    private readonly List<Sprite> _wrongToppingSprites = new();

    private Canvas _rootCanvas;
    private Camera _canvasCamera;

    private Coroutine _spawnCoroutine;
    private Coroutine _completeCoroutine;

    private int _spawnIndex;
    private int _score;
    private bool _isDragging;
    private bool _allObjectsSpawned;
    private Vector2 _sliceStartPosition;
    private readonly List<Image> _sliceLines = new();
    private Vector2 _previousPointerPosition;
    private float _currentSliceLineLength;

    private void Update()
    {
        if(_state != ESlice.Playing)
        {
            return;
        }
        UpdateSwipeInput();
    }
    private void OnDisable()
    {
        if(_spawnCoroutine!= null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
        if( _completeCoroutine!= null)
        {
            StopCoroutine(_completeCoroutine);
            _completeCoroutine = null;
        }
        ClearObjects();
        _isDragging = false;
        _state = ESlice.None;
    }
    public override void Open(OrderData order, Action<EQuality> onCompleted)
    {
        if (order == null || order.Recipe == null || order.Recipe.Data == null)
        {
            onCompleted?.Invoke(EQuality.Fail);
            return;
        }

        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
        if (_completeCoroutine != null)
        {
            StopCoroutine(_completeCoroutine);
            _completeCoroutine = null;
        }
        ClearObjects();
        _order = order;
        _onCompleted = onCompleted;
        _menuName.text = order.Recipe.Data.Name;
        _orderIcon.sprite = order.Recipe.Data.Icon;
        _ingredientIcon.sprite = order.Recipe.Data.IngredientIcon;
        _score = 0;
        _spawnIndex = 0;
        _isDragging = false;
        _allObjectsSpawned = false;
        _state = ESlice.Playing;
        BuildWrongToppingList();
        BuildSpawnPlan();
        if(_resultUI != null)
        {
            _resultUI.gameObject.SetActive(false);
        }
        gameObject.SetActive(true);
        _spawnCoroutine = StartCoroutine(SpawnCoroutine());
    }
    private IEnumerator SpawnCoroutine()
    {
        while(_spawnIndex<_spawnPlan.Count && _state == ESlice.Playing)
        {
            SpawnObject(_spawnPlan[_spawnIndex]);
            _spawnIndex++;
            yield return new WaitForSeconds(_spawnInterval);
        }
        _spawnCoroutine = null;
        _allObjectsSpawned = true;
        TryCompleteGame();
    }
    private void TryCompleteGame()
    {
        if (!_allObjectsSpawned || _activeObjects.Count > 0)
        {
            return;
        }
        CompleteGame();
    }
    private void BuildWrongToppingList()
    {
        _wrongToppingSprites.Clear();
        if(RecipeManager.Instance == null)
        {
            return;
        }
        foreach(Recipe recipe in RecipeManager.Instance.Recipes)
        {
            if(recipe.RecipeId == _order.Recipe.RecipeId)
            {
                continue;
            }
            if (_wrongToppingSprites.Contains(recipe.Data.IngredientIcon))
            {
                continue;
            }
            _wrongToppingSprites.Add(recipe.Data.IngredientIcon);
        }
    }
    private void BuildSpawnPlan()
    {
        _spawnPlan.Clear();
        for(int i= 0;i<_requiredSetCount;i++)
        {
            _spawnPlan.Add(ESliceObjectType.Rice);
            _spawnPlan.Add(ESliceObjectType.Wasabi);
            _spawnPlan.Add(ESliceObjectType.Fish);
        }
        for(int i = 0; i < _distractorCount; i++)
        {
            _spawnPlan.Add(UnityEngine.Random.value < 0.5f ? ESliceObjectType.WrongFish : ESliceObjectType.Junk);
        }
        ShuffleSpawnPlan();
    }
    private void ShuffleSpawnPlan()
    {
        for(int i=_spawnPlan.Count-1;i>0;i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            ESliceObjectType tmp = _spawnPlan[i];
            _spawnPlan[i] = _spawnPlan[randomIndex];
            _spawnPlan[randomIndex] = tmp;
        }
    }
    private void SpawnObject(ESliceObjectType objectType)
    {
        Sprite sprite = GetSprite(objectType);
        UI_SliceObject sliceObject = Instantiate(_sliceObjectPrefab, _playArea);
        Vector2 velocity = new Vector2(UnityEngine.Random.Range(-_maximumHorizontalSpeed, _maximumHorizontalSpeed), UnityEngine.Random.Range(_minimumThrowSpeed, _maximumThrowSpeed));
        float rotationSpeed = UnityEngine.Random.Range(_minimumRotationSpeed, _maximumRotationSpeed);
        float playAreaBottom = -_playArea.rect.height * 0.5f;
        float despawnY = playAreaBottom - _spawnBottomOffset;

        sliceObject.Init(this, objectType, sprite, velocity, _gravity, despawnY, rotationSpeed);

        RectTransform objectRect = sliceObject.RectTransform;
        float halfWidth = objectRect.rect.width * 0.5f;
        float halfHeight = objectRect.rect.height * 0.5f;
        float minimumX = -_playArea.rect.width * 0.5f + halfWidth;
        float maximumX = _playArea.rect.width * 0.5f - halfWidth;
        float spawnX = UnityEngine.Random.Range(minimumX, maximumX);
        float spawnY = playAreaBottom + halfHeight;

        objectRect.anchoredPosition = new Vector2(spawnX, spawnY);
        _activeObjects.Add(sliceObject);
    }
    private Sprite GetSprite(ESliceObjectType type)
    {
        switch (type)
        {
            case ESliceObjectType.Rice:
                return _riceSprite;
            case ESliceObjectType.Wasabi:
                return _wasabiSprite;
            case ESliceObjectType.Fish:
                return _order.Recipe.Data.IngredientIcon;
            case ESliceObjectType.WrongFish:
                return GetRandomSprite(_wrongToppingSprites);
            case ESliceObjectType.Junk:
                return GetRandomSprite(_junkSprites);
        }
        return null;
    }
    private Sprite GetRandomSprite(List<Sprite> sprites)
    {
        int randomIndex = UnityEngine.Random.Range(0, sprites.Count);
        return sprites[randomIndex];
    }
    public void OnObjectSliced(UI_SliceObject sliceObject)
    {
        if (_state != ESlice.Playing || sliceObject == null)
        {
            return;
        }
        _activeObjects.Remove(sliceObject);
        switch (sliceObject.SliceObjectType)
        {
            case ESliceObjectType.Rice:
            case ESliceObjectType.Wasabi:
            case ESliceObjectType.Fish:
                _score += _correctSliceScore;
                break;
            case ESliceObjectType.WrongFish:
                _score -= _wrongToppingPenalty;
                break;
            case ESliceObjectType.Junk:
                _score -= _junkPenalty;
                break;
        }
        _score = Mathf.Min(_score, 100);
        TryCompleteGame();
    }
    public void OnobjectMissed(UI_SliceObject sliceObject)
    {
        if (_state != ESlice.Playing || sliceObject == null)
        {
            return;
        }
        _activeObjects.Remove(sliceObject);
        TryCompleteGame();
    }
    private void UpdateSwipeInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClearSliceLine();
            _isDragging = true;
            _sliceStartPosition = Input.mousePosition;
            _previousPointerPosition = Input.mousePosition;
        }
        if (_isDragging && Input.GetMouseButton(0))
        {
            Vector2 currentPosition = Input.mousePosition;
            AddSliceLine(_previousPointerPosition, currentPosition);
            _previousPointerPosition = currentPosition;
            if (Vector2.Distance(_sliceStartPosition, currentPosition) >= _minimumSliceDistance)
            {
                CheckSliceBetween(_sliceStartPosition, currentPosition);
                _sliceStartPosition = currentPosition;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            ClearSliceLine();
        }
    }
    private void CheckSliceBetween(Vector2 start, Vector2 end)
    {
        _sliceCheckBuffer.Clear();
        _sliceCheckBuffer.AddRange(_activeObjects);
        foreach (UI_SliceObject sliceObject in _sliceCheckBuffer)
        {
            if (sliceObject == null ||!sliceObject.CanSlice)
            {
                continue;
            }
            Rect screenRect = GetScreenRect(sliceObject.RectTransform);
            if (LineIntersectsRect(start,end,screenRect))
            {
                sliceObject.Slice();
            }
        }
    }
    private Rect GetScreenRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        Vector2 bottomLeft = RectTransformUtility.WorldToScreenPoint(_canvasCamera, corners[0]);
        Vector2 topRight = RectTransformUtility.WorldToScreenPoint(_canvasCamera, corners[2]);
        return Rect.MinMaxRect(
            Mathf.Min(bottomLeft.x, topRight.x),
            Mathf.Min(bottomLeft.y, topRight.y),
            Mathf.Max(bottomLeft.x, topRight.x),
            Mathf.Max(bottomLeft.y, topRight.y));
    }
    private bool LineIntersectsRect(Vector2 start, Vector2 end, Rect rect)
    {
        if(rect.Contains(start) || rect.Contains(end))
        {
            return true;
        }
        Vector2 bottomLeft = new Vector2(rect.xMin, rect.yMin);
        Vector2 bottomRight = new Vector2(rect.xMax, rect.yMin);
        Vector2 topRight = new Vector2(rect.xMax, rect.yMax);
        Vector2 topLeft = new Vector2(rect.xMin, rect.yMax);

        return LineSegmentsIntersect(start, end, bottomLeft, bottomRight) ||
               LineSegmentsIntersect(start, end, bottomRight, topRight) ||
               LineSegmentsIntersect(start, end, topRight, topLeft) ||
               LineSegmentsIntersect(start, end, topLeft, bottomLeft);
    }
    private bool LineSegmentsIntersect(Vector2 a,Vector2 b, Vector2 c,Vector2 d)
    {
        float denominator = (b.x - a.x) * (d.y - c.y) - (b.y - a.y) * (d.x - c.x);
        if (Mathf.Approximately(denominator,0f))
        {
            return false;
        }
        float first = ((c.x - a.x) * (d.y - c.y) - (c.y - a.y) * (d.x - c.x)) / denominator;
        float second = ((c.x - a.x) * (b.y - a.y) - (c.y - a.y) * (b.x - a.x)) / denominator;
        return first >= 0f && first <= 1f && second >= 0f && second <= 1f;
    }
    private void AddSliceLine(Vector2 screenStart, Vector2 screenEnd)
    {
        if (_sliceLinePrefab == null)
        {
            return;
        }
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_playArea, screenStart, _canvasCamera, out Vector2 localStart);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_playArea, screenEnd, _canvasCamera, out Vector2 localEnd);
        Vector2 direction = localEnd - localStart;
        float length = direction.magnitude;
        if (length < 1f)
        {
            return;
        }
        Image line = Instantiate(_sliceLinePrefab, _playArea);
        RectTransform lineRect = line.rectTransform;
        line.raycastTarget = false;
        lineRect.anchorMin = lineRect.anchorMax = new Vector2(0.5f, 0.5f);
        lineRect.pivot = new Vector2(0.5f, 0.5f);
        lineRect.anchoredPosition = (localStart + localEnd) * 0.5f;
        lineRect.sizeDelta = new Vector2(length, _sliceLineWidth);
        lineRect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        lineRect.SetAsLastSibling();
        _sliceLines.Add(line);
        _currentSliceLineLength += length;
        while (_currentSliceLineLength > _maximumSliceLineLength && _sliceLines.Count > 0)
        {
            Image oldestLine = _sliceLines[0];
            _sliceLines.RemoveAt(0);
            if (oldestLine != null)
            {
                _currentSliceLineLength -= oldestLine.rectTransform.sizeDelta.x;
                Destroy(oldestLine.gameObject);
            }
        }
    }
    private void ClearSliceLine()
    {
        foreach (Image line in _sliceLines)
        {
            Destroy(line.gameObject);
        }
        _sliceLines.Clear();
        _currentSliceLineLength = 0f;
    }
    private void ClearObjects()
    {
        foreach (UI_SliceObject sliceObject in _activeObjects)
        {
            if (sliceObject != null)
            {
                Destroy(sliceObject.gameObject);
            }
        }
        _activeObjects.Clear();
        _sliceCheckBuffer.Clear();
    }
    private void CompleteGame()
    {
        if (_state == ESlice.Completed)
        {
            return;
        }
        _state = ESlice.Completed;
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
        ClearSliceLine();
        float baseScore = Mathf.Clamp01(_score / 100f);
        float staffBonus = GetStaffQualityBonus();
        float totalScore = _score + staffBonus;
        EQuality quality = CalculateQuality(totalScore);
        _completeCoroutine = StartCoroutine(CompleteCoroutine(quality, baseScore, staffBonus));
    }
    private float GetStaffQualityBonus()
    {
        if (_order == null || _order.Staff == null)
        {
            return 0f;
        }
        return _order.Staff.QualityBonus;
    }
    private EQuality CalculateQuality(float totalScore)
    {
        if (totalScore >= _greatScore)
        {
            return EQuality.Great;
        }
        else if (totalScore >= _goodScore)
        {
            return EQuality.Good;
        }
        else if (totalScore >= _normalScore)
        {
            return EQuality.Normal;
        }
        return EQuality.Fail;
    }

    private IEnumerator CompleteCoroutine(EQuality quality, float score, float staffBonus)
    {
        if (_resultUI != null)
        {
            _resultUI.ApplyResult(quality, score, staffBonus);
            _resultUI.gameObject.SetActive(true);
        }
        yield return new WaitForSecondsRealtime(_completeDuration);
        if (_resultUI != null)
        {
            _resultUI.gameObject.SetActive(false);
        }
        _completeCoroutine = null;
        Action<EQuality> callback = _onCompleted;
        _onCompleted = null;
        _order = null;
        ClearObjects();
        callback?.Invoke(quality);
        gameObject.SetActive(false);
    }
}
public enum ESliceObjectType
{
    Rice,
    Wasabi,
    Fish,
    WrongFish,
    Junk,
}
