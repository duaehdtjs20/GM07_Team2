using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace GM07.Map
{
    public class TableManager : MonoBehaviour
    {
        [SerializeField]
        private TableRegistry _tableRegistry;
        [SerializeField]
        private Table _tablePrefab;
        [SerializeField]
        private List<Transform> _tableSpawnPointList;

        private readonly List<Table> _tableList = new();
        private int _nextTableId = 1;

        public Table AddTable()
        {
            if(_tableList.Count >= _tableSpawnPointList.Count)
            {
                return null;
            }

            Transform spawnPoint = _tableSpawnPointList[_tableList.Count];

            Table table = Instantiate(_tablePrefab, spawnPoint.position, spawnPoint.rotation, transform);

            int tableId = GetNextTableId();
            table.Initialize(tableId);

            if(!_tableRegistry.Register(table))
            {
                Destroy(table.gameObject);
                return null;
            }

            _tableList.Add(table);
            return table;
        }

        private int GetNextTableId()
        {
            int tableId = _nextTableId;
            _nextTableId++;

            return tableId;
        }
    }
}
