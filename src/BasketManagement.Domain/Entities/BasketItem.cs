namespace BasketManagement.Domain.Entities;

public class BasketItem
{
    public long Id { get; private set; }
    public long ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public long BasketId { get; private set; }
    public Basket Basket { get; private set; } = null!;

    private BasketItem() { }

    internal BasketItem(long productId, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    internal void UpdateQuantity(int newQuantity) => Quantity = newQuantity;
}