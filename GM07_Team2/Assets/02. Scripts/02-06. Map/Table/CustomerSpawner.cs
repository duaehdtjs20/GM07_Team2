using System.Collections;
using UnityEngine;

namespace GM07.Map
{
    public sealed class CustomerSpawner : MonoBehaviour
    {
        [SerializeField] 
        private TableManager _tableManager;
        [SerializeField]
        private GameFlowManager _gameFlowManager;
        [SerializeField] 
        private GameObject _customerPrefab;
        [SerializeField]
        private Transform _spawnPoint;

        [SerializeField, Min(0.1f)]
        private float _spawnInterval;

        private Coroutine _spawnCoroutine;

        private void OnEnable()
        {
            if (_gameFlowManager != null)
            {
                _gameFlowManager.OnGameStateChanged += OnGameStateChanged;

                OnGameStateChanged(_gameFlowManager.GameState);
            }
        }

        private void OnDisable()
        {
            if (_gameFlowManager != null)
            {
                _gameFlowManager.OnGameStateChanged -= OnGameStateChanged;
            }

            StopSpawn();
        }

        private void OnGameStateChanged(EGameState gameState)
        {
            if (gameState == EGameState.Open)
            {
                StartSpawn();
                return;
            }

            StopSpawn();
        }

        private void StartSpawn()
        {
            if (_spawnCoroutine != null)
            {
                return;
            }

            _spawnCoroutine = StartCoroutine(StartSpawnCo());
        }

        private void StopSpawn()
        {
            if (_spawnCoroutine == null)
            {
                return;
            }

            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }

        private IEnumerator StartSpawnCo()
        {
            if (_tableManager == null)
            {
                _tableManager = FindFirstObjectByType<TableManager>();
            }
            var wait = new WaitForSeconds(_spawnInterval);
            while (true)
            {
                yield return wait;
                TrySpawn();
            }
        }

        public bool TrySpawn()
        {
            if (_tableManager == null || !_tableManager.TryUseSeat(out Table table, out Seat seat) || _customerPrefab == null)
            {
                return false;
            }
            GameObject customer = Instantiate(_customerPrefab, _spawnPoint.position, _spawnPoint.rotation);
            if(customer.TryGetComponent<Customer>(out Customer customerComponent))
            {
                customerComponent.Init(table, seat);
                return true;
            }
            else
            {
                Destroy(customer);
                _tableManager.ReleaseSeat(table, seat);
                return false;
            }
        }
    }
}
