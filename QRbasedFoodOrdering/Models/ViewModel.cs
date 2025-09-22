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
    public class PopularFoodReportViewModel
    {
        public DateTime StartDate { get; set; }  
        public DateTime EndDate { get; set; }     
        public int FoodItemId { get; set; }
        public string FoodItemName { get; set; }
        public int TotalQuantity { get; set; }
    }

   


    }
