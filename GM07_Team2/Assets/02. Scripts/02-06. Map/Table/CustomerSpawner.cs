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
        [SerializeField, Min(0.1f)]
        private float _spawnInterval;
        [Header("Test")]
        [SerializeField, Min(0.1f)]
        private float _customerStayDuration = 3f;

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


        // test
        public bool TrySpawn()
        {
            if (_tableManager == null || !_tableManager.TryUseSeat(out Table table, out Seat seat) || _customerPrefab == null)
            {
                return false;
            }
            GameObject customer = Instantiate(_customerPrefab, transform.position, Quaternion.identity);
            customer.name = $"Customer_T{table.TableId:00}_S{seat.SeatId:00}";
            customer.transform.position = seat.Anchor.position;
            customer.transform.rotation = seat.Anchor.rotation;

            StartCoroutine(StayAndRelease(customer, table, seat, _customerStayDuration));

            return true;
        }

        private IEnumerator StayAndRelease(GameObject customer, Table table, Seat seat, float stayDuration)
        {
            yield return new WaitForSeconds(stayDuration);

            if (_tableManager != null)
            {
                _tableManager.ReleaseSeat(table, seat);
            }

            if (customer != null)
            {
                Destroy(customer);
            }
        }
    }
}
