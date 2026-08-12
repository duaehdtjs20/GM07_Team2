using GM07.Map;

namespace GM07.Order
{
    public class OrderData
    {
        public Seat Seat;
        public float OrderRequestTime;
        public Customer Customer;
        public Recipe Recipe;
        public EOrderState State = EOrderState.Waiting;
        public float CookStartTime;
        public EQuality Quality = EQuality.Normal;
    }
}