namespace QRbasedFoodOrdering.Models
{

    public enum OrderStatus
    {
        Pending,
        Comfirmed,
        Preparing,
        Completed,
        BillRequested,
        Cancelled
    }
    public enum OrderDetailStatus
    {
        Pending,
        Comfirmed,
        Preparing,
        Served,
        Cancelled

    }
    public enum  TableStatus
    {
        Available,
        Occupied,
        Preparing,
        Served
    }
}
