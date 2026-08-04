using System.Collections.Generic;
using UnityEngine;

namespace GM07.Map
{
    public sealed class TableRegistry : MonoBehaviour
    {
        private readonly Dictionary<int, Table> _tableDictionary = new();
        public int TableCount => _tableDictionary.Count;

        public bool Register(Table table)
        {
            if (table == null || _tableDictionary.ContainsKey(table.TableId))
            {
                return false;
            }
            _tableDictionary.Add(table.TableId, table);
            return true;
        }

        public bool TryUseSeat(out Table selectedTable, out Seat selectedSeat)
        {
            int maxRemainingSeats = 0;
            List<Table> candidateTableList = new();

            foreach (Table table in _tableDictionary.Values)
            {
                int remainingSeats = table.RemainingSeatsCount;
                if (remainingSeats == 0)
                {
                    continue;
                }
                if (remainingSeats > maxRemainingSeats)
                {
                    maxRemainingSeats = remainingSeats;
                    candidateTableList.Clear();
                    candidateTableList.Add(table);
                    continue;
                }

                if (remainingSeats == maxRemainingSeats)
                {
                    candidateTableList.Add(table);
                }
            }

            if (candidateTableList.Count == 0)
            {
                selectedSeat = null;
                selectedTable = null;
                return false;
            }

            int randomTableIndex = Random.Range(0, candidateTableList.Count);
            selectedTable = candidateTableList[randomTableIndex];
            if (selectedTable.TryRandomSeat(out selectedSeat))
            {
                return true;
            }

            selectedSeat = null;
            selectedTable = null;
            return false;
        }
    }
}
