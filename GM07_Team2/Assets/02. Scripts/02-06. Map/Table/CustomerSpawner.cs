using System.Collections;
using UnityEngine;

namespace GM07.Map
{
    public sealed class CustomerSpawner : MonoBehaviour
    {
        [SerializeField] 
        private TableManager _tableManager;
        [SerializeField] 
        private GameObject _customerPrefab;
        [SerializeField]
        private Transform _spawnPoint;

        [SerializeField, Min(0.1f)]
        private float _spawnInterval;

        private IEnumerator Start()
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
                return false;
            }
        }
    }
}
