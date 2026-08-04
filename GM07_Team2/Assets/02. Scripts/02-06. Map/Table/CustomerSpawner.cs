using System.Collections;
using UnityEngine;

namespace GM07.Map
{
    public sealed class CustomerSpawner : MonoBehaviour
    {
        [SerializeField] 
        private TableRegistry _registry;
        [SerializeField] 
        private GameObject _customerPrefab;
        [SerializeField, Min(0.1f)]
        private float _spawnInterval;

        private IEnumerator Start()
        {
            if (_registry == null)
            {
                _registry = FindFirstObjectByType<TableRegistry>();
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
            if (_registry == null || !_registry.TryUseSeat(out Table table, out Seat seat) || _customerPrefab == null)
            {
                return false;
            }
            GameObject customer = Instantiate(_customerPrefab, transform.position, Quaternion.identity);
            // test
            customer.name = $"Customer_T{table.TableId:00}_S{seat.SeatId:00}";
            customer.transform.position = seat.Anchor.position;
            customer.transform.rotation = seat.Anchor.rotation;
            return true;
        }
    }
}
