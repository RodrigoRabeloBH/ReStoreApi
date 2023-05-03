namespace ReStore.Domain.Entities.OrderAggregate
{
    public enum OrderStatus
    {
        Pending,
        PaymentReceived,
        PaymentFailed
    }
}
