using System;
using System.Collections;
using System.Collections.Generic;

using GM07.Order;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using Random = UnityEngine.Random;

public class UI_IceBreakGame : UI_MiniGameBase
{
    // UI 갱신 이벤트
    public event Action OnChanged;

    // 미니게임 결과 UI
    [Header("Result UI")]
    [SerializeField]
    private UI_MiniGameResult _resultUI;

    // 충격 UI
    [Header("Impact UI")]
    [SerializeField]
    private Image _impactImage;
    // 충격 퍼센트 텍스트
    [SerializeField]
    private TMP_Text _impactPercent;

    // 정보 표시 UI
    [Header("Info UI")]
    [SerializeField]
    private TMP_Text _countText;
    [SerializeField]
    private Image _hammerBtnImg;
    [SerializeField]
    private Image _awlBtnImg;

    // 생선 이미지
    [SerializeField]
    private Image _fishImage;

    // 플레이하는 타일 오브젝트들
    [SerializeField]
    private GameObject[] _lines;

    // 재료 위치를 설정하기 위한 UI
    [Header("Ingredient")]
    [SerializeField]
    private RectTransform _fish;
    [SerializeField]
    private RectTransform _rice;
    [SerializeField]
    private RectTransform _wasabi;

    // 현재 얼음을 부수는 타입
    [SerializeField]
    private EBreakType _breakType;
    // 게임 클리어 창 표시 시간
    [SerializeField]
    private float _completeDuration;

    // 현재 충격 게이지
    private int _impactGage;

    //주문서
    [Header("Order")]
    [SerializeField]
    private TMP_Text _menuText;
    [SerializeField]
    private Image _orderIcon;

    // UI 갱신 및 이벤트 기능 연결을 위한 배열
    private UI_IceBlock[,] _blockViews;
    // 각 위치 별 블럭의 상태를 보관
    private EIceBlockState[,] _blockStates;
    // 각 재료가 포함되어 있는 칸의 정보
    private EIngredientType?[,] _ingredients;
    // 마우스가 위치하는 현 좌표
    private bool[,] _mouseHovers;
    // 최대 XY 크기
    private int _sizeX;
    private int _sizeY;
    // 인덱스 단위로 4방향 탐색을 위한 읽기전용 배열
    private readonly int[] _directionX = { 1, -1, 0, 0 };
    private readonly int[] _directionY = { 0, 0, 1, -1 };
    // 찾은 재료의 최대 개수
    private int _maxCount = 0;

    // 주문 정보 및 외부 로직 연결을 위한 필드
    private OrderData _order;
    private Action<EQuality> _onCompleted;
    private Coroutine _completeCoroutine;

    // indexer를 사용한 프로퍼티 (2차원 배열을 읽기 전용으로 가져오기 위함)
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
        _mouseHovers = new bool[_sizeX, _sizeY];

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

                // Ingredient
                _ingredients[x, y] = null;

                // hover
                _mouseHovers[x, y] = false;
            }
        }

        // fish 설정
        SetIngredient(3, 2, EIngredientType.Fish, _fish);
        // rice 설정
        SetIngredient(2, 2, EIngredientType.Rice, _rice);
        // wasabi 설정
        SetIngredient(1, 1, EIngredientType.Wasabi, _wasabi);

        OnChanged += RefreshImpact;
        OnChanged += RefreshCount;
        OnChanged += RefreshBreakArea;
        OnChanged += RefreshSelectButton;

        OnChanged?.Invoke();
    }
    private void OnDisable()
    {
        OnChanged -= RefreshImpact;
        OnChanged -= RefreshCount;
        OnChanged -= RefreshBreakArea;
        OnChanged -= RefreshSelectButton;
    }

    public override void Open(OrderData order, Action<EQuality> onCompleted)
    {
        _order = order;
        _onCompleted = onCompleted;
        _impactGage = 0;
        _maxCount = 0;
        _breakType = EBreakType.Hammer;
        _fishImage.sprite = _order.Recipe.Data.IngredientIcon;
        _menuText.text = _order.Recipe.Data.Name;
        _orderIcon.sprite = _order.Recipe.Data.Icon;
        gameObject.SetActive(true);
    }
    public void BreakBlock(int x, int y)
    {
        // 이미 파괴 된 칸 클릭 이거나 미니게임 종료됨
        if (_blockStates[x, y] == EIceBlockState.Breaked || _completeCoroutine != null)
        {
            return;
        }

        // 클릭한 블록 파괴
        if (_blockStates[x, y] == EIceBlockState.Intact)
        {
            // 온전한 상태면 충격량 10
            _impactGage += 10;
        }
        else
        {
            // 이미 일부 파괴된 상태면 충격량 5
            _impactGage += 5;
        }
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
                // 이미 완전히 부서진 블럭인 경우 건너뛰기
                if (_blockStates[nx, ny] == EIceBlockState.Breaked)
                {
                    continue;
                }

                // 온전한 상태면 일부 파괴
                if (_blockStates[nx, ny] == EIceBlockState.Intact)
                {
                    _blockStates[nx, ny] = EIceBlockState.Cracked;
                    _impactGage += 5;
                }
                // 이미 일부 파괴된 상태면 완전 파괴
                else
                {
                    _blockStates[nx, ny] = EIceBlockState.Breaked;
                    _impactGage += 5;
                }
            }
        }
        OnChanged?.Invoke();
        CheckAllFind();
        CheckImpact();
    }
    public void SwitchHammer()
    {
        _breakType = EBreakType.Hammer;
        OnChanged?.Invoke();
    }
    public void SwitchAwl()
    {
        _breakType = EBreakType.Awl;
        OnChanged?.Invoke();
    }
    public void EnterBlock(int x, int y)
    {
        _mouseHovers[x, y] = true;
        OnChanged?.Invoke();
    }
    public void ExitBlock(int x, int y)
    {
        _mouseHovers[x, y] = false;
        OnChanged?.Invoke();
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
        _maxCount = Mathf.Max(_maxCount, count);
        if(_maxCount >= 3)
        {
            Result();
        }
    }
    private void Finish(EQuality result)
    {
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
    private void RefreshCount()
    {
        _countText.text = _maxCount.ToString() + "/3";
    }
    private void RefreshImpact()
    {
        _impactImage.fillAmount = (float)_impactGage / 200.0f;
        _impactPercent.text = Mathf.CeilToInt(_impactImage.fillAmount * 100f).ToString();
    }
    private void CheckImpact()
    {
        // 최대 충격량에 도달하지 않으면 반환
        if (_impactGage < 200)
        {
            return;
        }
        // 모든 재료를 찾은 경우
        if (_completeCoroutine != null)
        {
            return;
        }
        Result();
    }
    private void RefreshBreakArea()
    {
        int x = -1;
        int y = -1;
        // 모든 블럭 색상 복구
        for (int i = 0; i < _sizeX; i++)
        {
            for (int j = 0; j < _sizeY; j++)
            {
                if (_blockStates[i, j] == EIceBlockState.Breaked)
                {
                    continue;
                }

                _blockViews[i, j].Image.color = new Color(1.0f, 1.0f, 1.0f, _blockViews[i, j].Image.color.a);

                // 현재 블록에 마우스가 위치하면 좌표 저장
                if (_mouseHovers[i, j])
                {
                    x = i;
                    y = j;
                }
            }
        }
        // 마우스가 올라가 있는 좌표가 없는 경우
        if (x == -1 || y == -1)
        {
            return;
        }

        // 마우스 좌표 블럭 색상 변경
        _blockViews[x, y].Image.color = new Color(1.0f, 0.4f, 0.4f, _blockViews[x, y].Image.color.a);

        // Awl 타입인 경우 반환
        if (_breakType == EBreakType.Awl)
        {
            return;
        }

        // 해머 범위 색상 변경
        for (int i = 0; i < 4; i++)
        {
            int nextX = x + _directionX[i];
            int nextY = y + _directionY[i];
            if (nextX < 0 || nextY < 0 || nextX >= _sizeX || nextY >= _sizeY)
            {
                continue;
            }
            if (_blockStates[nextX, nextY] == EIceBlockState.Breaked)
            {
                continue;
            }
            _blockViews[nextX, nextY].Image.color = new Color(1.0f, 0.7f, 0.7f, _blockViews[nextX, nextY].Image.color.a);
        }
    }
    private void RefreshSelectButton()
    {
        if (_breakType == EBreakType.Hammer)
        {
            _hammerBtnImg.color = Color.orange;
            _awlBtnImg.color = Color.black;
        }
        else
        {
            _hammerBtnImg.color = Color.black;
            _awlBtnImg.color = Color.orange;
        }
    }
    private void Result()
    {
        int _totalCount = _maxCount + (((int)GetStaffQualityBonus()) / 10);

        // 찾은 재료 개수 별 결과 호출
        if (_totalCount == 0)
        {
            _completeCoroutine = StartCoroutine(CompleteCo(EQuality.Fail, _totalCount));
        }
        else if (_totalCount == 1)
        {
            _completeCoroutine = StartCoroutine(CompleteCo(EQuality.Normal, _totalCount));
        }
        else if (_totalCount == 2)
        {
            _completeCoroutine = StartCoroutine(CompleteCo(EQuality.Good, _totalCount));
        }
        else
        {
            _completeCoroutine = StartCoroutine(CompleteCo(EQuality.Great, _totalCount));
        }
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
}
