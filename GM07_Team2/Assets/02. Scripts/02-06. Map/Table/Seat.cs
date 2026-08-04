using System;
using UnityEngine;

namespace GM07.Map
{
    [Serializable]
    public sealed class Seat
    {
        [SerializeField]
        private int seatId;
        [SerializeField]
        private Transform anchor;
        [SerializeField]
        private bool isUsing;

        public int SeatId => seatId;
        public Transform Anchor => anchor;
        public bool IsUsing => isUsing;
        public Seat(int seatId, Transform anchor)
        {
            this.seatId = seatId;
            this.anchor = anchor;
            isUsing = false;
        }

        public bool TryUse()
        {
            if (isUsing || anchor == null)
            {
                return false;
            }
            isUsing = true;
            return true;
        }

        public void TryRelease()
        {
            if (!isUsing)
            {
                return;
            }
            isUsing = false;
        }
    }
}
