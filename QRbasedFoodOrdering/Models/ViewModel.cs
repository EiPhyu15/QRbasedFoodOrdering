namespace QRbasedFoodOrdering.Models
{
    public class DailySalesViewModel
    {
        public DateTime Date { get; set; }
        public double TotalSales { get; set; }
        public int TotalOrders { get; set; }
    }
    public class  WeeklySalesViewModel
    {
        public int Year { get; set; }
        public int Week { get; set; }
        public double TotalSales { get; set; }
        public int TotalOrders { get; set; }
    }

}
