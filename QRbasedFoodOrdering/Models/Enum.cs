namespace QRbasedFoodOrdering.Models
{

    public enum OrderStatus
    {
        Pending,
        Comfirmed,
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
        Reserved
    }
}
