using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RestaurantPanel : MonoBehaviour
{
    [Header("Restaurant")]
    [SerializeField]
    private Restaurant _restaurant;
    [Header("Update Button")]
    [SerializeField]
    private Button _upgradeButton;
    [Header("MaxPanel")]
    [SerializeField]
    private GameObject _maxPanel;
    [Header("Current")]
    [SerializeField]
    private TMP_Text _currentLevel;
    [SerializeField]
    private TMP_Text _currentDetails;
    [Header("Next")]
    [SerializeField]
    private TMP_Text _nextLevel;
    [SerializeField]
    private TMP_Text _nextDetails;

    private void OnEnable()
    {
        if (_restaurant == null)
        {
            return;
        }

        _restaurant.OnRestaurantChanged += RefreshUI;
        _upgradeButton?.onClick.AddListener(OnClickUpgrade);
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnMoneyChanged += OnMoneyChanged;
        }
        RefreshUI();
    }

    private void OnDisable()
    {
        if (_restaurant != null)
        {
            _restaurant.OnRestaurantChanged -= RefreshUI;
        }

        _upgradeButton?.onClick.RemoveListener(OnClickUpgrade);
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnMoneyChanged -= OnMoneyChanged;
        }
    }

    public void RefreshUI()
    {
        if (_restaurant == null)
        {
            return;
        }

        int level = _restaurant.Level;

        if (_currentLevel != null)
        {
            _currentLevel.text = $"Lv. {level}";
        }

        if (_currentDetails != null)
        {
            _currentDetails.text =
                $"{_restaurant.TableCount}\n\n" +
                $"{_restaurant.ChefCount}\n\n" +
                $"{_restaurant.Rent:N0}";
        }

        bool isMaxLevel = _restaurant.IsMaxLevel;
        _maxPanel?.SetActive(isMaxLevel);

        if (_upgradeButton != null)
        {
            _upgradeButton.gameObject.SetActive(!isMaxLevel);
            /*_upgradeButton.interactable =
                !isMaxLevel && CurrencyManager.Instance != null &&
                CurrencyManager.Instance.Money >= _restaurant.UpgradeCost;*/
        }

        if (isMaxLevel)
        {
            return;
        }

        int nextLevel = level + 1;
        if (_nextLevel != null)
        {
            _nextLevel.text = $"Lv. {nextLevel}";
        }

        if (_nextDetails != null)
        {
            _nextDetails.text =
                $"{_restaurant.GetTableCount(nextLevel)}\n\n" +
                $"{_restaurant.GetChefCount(nextLevel)}\n\n" +
                $"{_restaurant.GetRent(nextLevel):N0}\n\n\n" +
                $"{_restaurant.UpgradeCost:N0}";
        }
    }

    private void OnClickUpgrade()
    {
        if (!_restaurant.TryUpgrade())
        {
            if (_upgradeButton.TryGetComponent(out ButtonFailEffect effect))
            {
                effect.Play();
            }
            return;
        }
        this.gameObject.SetActive(false);
        //RefreshUI();
    }

    private void OnMoneyChanged(int money)
    {
        RefreshUI();
    }
}

