namespace FoodMatch.Level.Mechanics.Orders
{
    public class OrderModel
    {
        public string OrderID { get; set; }
        public int TargetAmount { get; set; }
        public int CurrentAmount { get; set; }

        public OrderModel(string orderID, int targetAmount)
        {
            OrderID = orderID;
            TargetAmount = targetAmount;
            CurrentAmount = 0;
        }

        public void IncreaseAmount(int amount)
        {
            CurrentAmount += amount;
        }

        public bool IsCompleted()
        {
            return CurrentAmount >= TargetAmount;
        }
    }
}