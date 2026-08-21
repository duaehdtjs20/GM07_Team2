using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using GM07.Order;

using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

public class UI_IceBreakGame : UI_MiniGameBase
{
    public event Action OnChanged;
    [SerializeField]
    private UI_MiniGameResult _resultUI;
    [SerializeField]
    private GameObject[] _lines;

    [SerializeField]
    private RectTransform _fish;
    [SerializeField]
    private RectTransform _rice;
    [SerializeField]
    private RectTransform _wasabi;

    [SerializeField]
    private EBreakType _breakType;
    [SerializeField]
    private float _completeDuration;

    private UI_IceBlock[,] _blockViews;
    private EIceBlockState[,] _blockStates;
    private EIngredientType?[,] _ingredients;
    private int _sizeX;
    private int _sizeY;
    private readonly int[] _directionX = { 1, -1, 0, 0 };
    private readonly int[] _directionY = { 0, 0, 1, -1 };

    private OrderData _order;
    private Action<EQuality> _onCompleted;
    private Coroutine _timerCoroutine;
    private Coroutine _completeCoroutine;

    public EIceBlockState this[int y, int x] => _blockStates[y, x];

    private void Awake()
    {
        // null reference exception
        if (_lines == null || _lines.Length <= 0)
        {
            return;
        }
        // 크기 지정
        _sizeY = _lines.Length;
        _sizeX = _lines[0].transform.childCount;

        // 배열 크기 초기화
        _blockViews = new UI_IceBlock[_sizeX, _sizeY];
        _blockStates = new EIceBlockState[_sizeX, _sizeY];
        _ingredients = new EIngredientType?[_sizeX, _sizeY];

        // Block 연결
        for (int y = 0; y < _sizeY; y++)
        {
            for (int x = 0; x < _sizeX; x++)
            {
                Transform block = _lines[y].transform.GetChild(x);
                block.name = $"block ({x}, {y})";
                if (block.TryGetComponent(out UI_IceBlock view))
                {
                    view.Bind(this, x, y);
                    _blockViews[x, y] = view;
                }
            }
        }
    }
    private void OnEnable()
    {
        // 새로 활성화 시 필드 값 초기화
        for (int x = 0; x < _sizeX; x++)
        {
            for (int y = 0; y < _sizeY; y++)
            {
                // Block
                _blockStates[x, y] = EIceBlockState.Intact;
                _blockViews[x, y].Refresh();

                // Ingredient
                _ingredients[x, y] = null;
            }
        }

        // fish 설정
        SetIngredient(3, 2, EIngredientType.Fish, _fish);
        // rice 설정
        SetIngredient(2, 2, EIngredientType.Rice, _rice);
        // wasabi 설정
        SetIngredient(1, 1, EIngredientType.Wasabi, _wasabi);
    }

    private void SetIngredient(int row, int col, EIngredientType ingredientType, RectTransform rect)
    {
        // 최대 범위 제한
        int maxX = _sizeX - row + 1;
        int maxY = _sizeY - col + 1;

        while (true)
        {
            // 제한된 범위 내에서 랜덤 좌표
            int randX = Random.Range(0, maxX);
            int randY = Random.Range(0, maxY);

            // 나온 좌표에 재료를 넣어도 되는지 확인(중복 체크)
            bool able = true;
            for (int r = randX; r < randX + row; r++)
            {
                for (int c = randY; c < randY + col; c++)
                {
                    able &= _ingredients[r, c] == null;
                }
            }
            // 해당 위치에 넣어도 되는 경우
            if (able)
            {
                // 적용된 칸에 타입 변경
                for (int r = randX; r < randX + row; r++)
                {
                    for (int c = randY; c < randY + col; c++)
                    {
                        _ingredients[r, c] = ingredientType;
                    }
                }
                // 이미지 위치 조정
                rect.anchoredPosition = new Vector2((randX + (row / 2.0f)) * 150.0f, -(randY + (col / 2.0f)) * 150.0f);
                break;
            }
        }
    }
    private void CheckAllFind()
    {
        int count = 0;
        bool[,] visited = new bool[_sizeX, _sizeY];
        Queue<(int x, int y)> queue = new Queue<(int x, int y)>();
        for (int startX = 0; startX < _sizeX; startX++)
        {
            for (int startY = 0;  startY < _sizeY; startY++)
            {
                // 이미 방문한 좌표거나 | 해당 칸의 블럭이 깨지지 않았거나 | 해당 칸에 재료가 없는 경우
                if (visited[startX, startY] || _blockStates[startX, startY] != EIceBlockState.Breaked || _ingredients[startX, startY] == null)
                {
                    continue;
                }
                Debug.Log($"{startX} {startY}");
                // bfs search
                EIngredientType? ingredient = _ingredients[startX, startY];
                queue.Clear();
                visited[startX, startY] = true;
                queue.Enqueue((startX, startY));
                bool flag = true;
                while (queue.Count > 0)
                {
                    var cur = queue.Dequeue();
                    int currentX = cur.x;
                    int currentY = cur.y;

                    // 4방향 탐색
                    for (int i = 0; i < 4; i++)
                    {
                        int nextX = currentX + _directionX[i];
                        int nextY = currentY + _directionY[i];

                        // index out of range
                        if (nextX < 0 || nextY < 0 || nextX >= _sizeX || nextY >= _sizeY)
                        {
                            continue;
                        }
                        // 이미 방문한 적 있는 칸이거나 다른 재료인 경우 건너뛰기
                        if (visited[nextX, nextY] || _ingredients[nextX, nextY] != ingredient)
                        {
                            continue;
                        }
                        // Block이 깨져있는 경우 방문 예약
                        if (_blockStates[nextX, nextY] == EIceBlockState.Breaked)
                        {
                            visited[nextX, nextY] = true;
                            queue.Enqueue((nextX, nextY));
                        }
                        // 아직 깨지지 않았으면 해당 재료를 아직 완전히 찾지 못함
                        else
                        {
                            flag = false;
                        }
                    }
                }
                if (flag)
                {
                    count++;
                }
            }
        }
        Debug.Log($"COUNT : {count}");
        if (count >= 3)
        {
            _completeCoroutine = StartCoroutine(CompleteCo(EQuality.Great));
        }
    }
    public void BreakBlock(int x, int y)
    {
        // 이미 파괴 된 칸 클릭
        if (_blockStates[x, y] == EIceBlockState.Breaked)
        {
            return;
        }

        // 클릭한 블록 파괴
        _blockStates[x, y] = EIceBlockState.Breaked;

        // BreakType이 Hammer인 경우
        if (_breakType == EBreakType.Hammer)
        {
            // 클릭한 칸 주변 칸에 충격 전달
            for (int i = 0; i < 4; i++)
            {
                int nx = x + _directionX[i];
                int ny = y + _directionY[i];

                // index out of range
                if (nx < 0 ||  ny < 0 || nx >= _sizeX || ny >= _sizeY)
                {
                    continue;
                }

                // 온전한 상태면 일부 파괴
                if (_blockStates[nx, ny] == EIceBlockState.Intact)
                {
                    _blockStates[nx, ny] = EIceBlockState.Cracked;
                }
                // 그 외의 경우 완전 파괴
                else
                {
                    _blockStates[nx, ny] = EIceBlockState.Breaked;
                }
            }
        }
        OnChanged?.Invoke();
        CheckAllFind();
    }
    public override void Open(OrderData order, Action<EQuality> onCompleted)
    {
        _order = order;
        _onCompleted = onCompleted;
        gameObject.SetActive(true);
    }
    private IEnumerator CompleteCo(EQuality quality, float score = 0)
    {
        if(_resultUI != null)
        {
            _resultUI.ApplyResult(quality, score, GetStaffQualityBonus());
            _resultUI.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(_completeDuration);
        if (_resultUI != null)
        {
            _resultUI.gameObject.SetActive(false);
        }
        _completeCoroutine = null;
        Finish(quality);
    }
    private void Finish(EQuality result)
    {
        if(_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }
        Action<EQuality> callback = _onCompleted;
        _onCompleted = null;
        gameObject.SetActive(false);
        callback?.Invoke(result);
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
}
