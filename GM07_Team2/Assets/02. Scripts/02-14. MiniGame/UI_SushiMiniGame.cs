using GM07.Order;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SushiMiniGame : UI_MiniGameBase
{
    private enum EStep
    {
        Rice,
        Wasabi,
        Fish,
        Completed
    }
    [Header("Order")]
    [SerializeField]
    private GameObject _orderSheet;
    [SerializeField]
    private TMP_Text _nemuName;
    [SerializeField]
    private Image _orderIcon;
    [SerializeField]
    private Image _ingredientIcon;
    [Header("Timer")]
    [SerializeField]
    private TMP_Text _timer;
    [SerializeField]
    private float _timeLimit;
    [SerializeField]
    private float _completeDuration;
    [Header("Step Images")]
    [SerializeField]
    private GameObject _lineImage;
    [SerializeField]
    private GameObject _riceImage;
    [SerializeField]
    private GameObject _wasabiImage;
    [SerializeField]
    private Image _sushiImage;
    [Header("Fish Choice")]
    [SerializeField]
    private List<UI_IngredientDrag> _fishChoiceList = new();
    [Header("Quality")]
    [SerializeField]
    [Range(0f, 1f)]
    private float _greatRemainRatio;
    [SerializeField]
    [Range(0f, 1f)]
    private float _goodRemainRatio;
    [Header("Result Setting")]
    [SerializeField]
    private UI_MiniGameResult _resultUI;

    private OrderData _order;
    private EStep _currentStep;
    private float _remainingTime;
    private Coroutine _timerCoroutine;
    private Coroutine _completeCoroutine;
    private Action<EQuality> _onCompleted;

    public override void Open(OrderData order, Action<EQuality> onCompleted)
    {
        _order = order;
        _onCompleted = onCompleted;
        _currentStep = EStep.Rice;
        _remainingTime = _timeLimit;
        _nemuName.text = order.Recipe.Data.Name;
        _orderIcon.sprite = order.Recipe.Data.Icon;
        _ingredientIcon.sprite = order.Recipe.Data.IngredientIcon;
        _lineImage.SetActive(true);
        _riceImage.SetActive(false);
        _wasabiImage.SetActive(false);
        _sushiImage.gameObject.SetActive(false);
        CreateFishChoice();
        gameObject.SetActive(true);
        _timerCoroutine = StartCoroutine(TimerCo());
    }
    public void OnIngredientDrop(UI_IngredientDrag ingredient)
    {
        switch (_currentStep)
        {
            case EStep.Rice:
                RiceStep(ingredient);
                break;
            case EStep.Wasabi:
                WasabiStep(ingredient);
                break;
            case EStep.Fish:
                FishStep(ingredient);
                break;
            default:
                break;
        }
    }
    private void RiceStep(UI_IngredientDrag ingredient)
    {
        if(ingredient.IngredientType != EIngredientType.Rice)
        {
            _completeCoroutine = StartCoroutine(CompleteCo(EQuality.Fail));
            return;
        }
        if(_riceImage != null)
        {
            _riceImage.SetActive(true);
        }
        _currentStep = EStep.Wasabi;
    }
    private void WasabiStep(UI_IngredientDrag ingredient)
    {
        if (ingredient.IngredientType != EIngredientType.Wasabi)
        {
            _completeCoroutine = StartCoroutine(CompleteCo(EQuality.Fail));
            return;
        }
        if (_wasabiImage != null)
        {
            _wasabiImage.SetActive(true);
        }
        _currentStep = EStep.Fish;
    }
    private void FishStep(UI_IngredientDrag ingredient)
    {
        if (ingredient.IngredientType != EIngredientType.Fish)
        {
            _completeCoroutine = StartCoroutine(CompleteCo(EQuality.Fail));
            return;
        }
        if(ingredient.RecipeData.RecipeId != _order.Recipe.RecipeId)
        {
            _completeCoroutine = StartCoroutine(CompleteCo(EQuality.Fail));
            return;
        }
        _currentStep = EStep.Completed;
        if(_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }
        if(_sushiImage != null)
        {
            _sushiImage.sprite = _order.Recipe.Data.Icon;
            _sushiImage.gameObject.SetActive(true);
        }
        EQuality quality = CalculateQuality(out float totalScore);
        _completeCoroutine = StartCoroutine(CompleteCo(quality, totalScore));
    }
    private void CreateFishChoice()
    {
        RecipeData correctData = _order.Recipe.Data;
        List<RecipeData> candidates = RecipeManager.Instance.Recipes
            .Where(recipe =>
                   recipe.Data != null &&
                   recipe.Data.IngredientIcon != null &&
                   recipe.RecipeId != correctData.RecipeId)
            .Select(recipe => recipe.Data)
            .OrderBy(_ => UnityEngine.Random.value)
            .Take(3)
            .ToList();
        candidates.Add(correctData);
        candidates = candidates.OrderBy(_ => UnityEngine.Random.value).ToList();

        for (int i = 0; i < _fishChoiceList.Count; i++)
        {
            bool hasChoice = i < candidates.Count;

            _fishChoiceList[i].gameObject.SetActive(hasChoice);

            if (hasChoice)
            {
                _fishChoiceList[i].InitFish(candidates[i]);
            }
        }
    }
    private IEnumerator TimerCo()
    {
        while(_remainingTime > 0)
        {
            _remainingTime -= Time.deltaTime;
            _timer.text = Mathf.CeilToInt(_remainingTime).ToString();
            yield return null;
        }
        _timerCoroutine = null;
        _completeCoroutine = StartCoroutine(CompleteCo(EQuality.Fail));
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
    private EQuality CalculateQuality(out float score)
    {
        float remainRatio = Mathf.Clamp01(_remainingTime / _timeLimit);
        float staffBonusScore = GetStaffQualityBonus();
        float totalScore = remainRatio + staffBonusScore;
        if(totalScore >= _greatRemainRatio)
        {
            score = totalScore;
            return EQuality.Great;
        }
        if(totalScore >= _goodRemainRatio)
        {
            score = totalScore;
            return EQuality.Good;
        }
        score = totalScore;
        return EQuality.Normal;
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
