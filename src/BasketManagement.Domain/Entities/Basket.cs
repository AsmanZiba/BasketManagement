using BasketManagement.Domain.Enums;
using BasketManagement.Domain.Events;
using BasketManagement.Domain.Interfaces;

namespace BasketManagement.Domain.Entities;

public class Basket : IDomainEntity
{
    private readonly List<BasketItem> _items = new();
    private readonly List<IDomainEvent> _domainEvents = new();

    public long Id { get; private set; }
    public long UserId { get; private set; }
    public BasketStatus Status { get; private set; }
    public IReadOnlyList<BasketItem> Items => _items.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastUpdatedAt { get; private set; }
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Basket() { }

    public Basket(long userId)
    {
        UserId = userId;
        Status = BasketStatus.Active;
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>افزودن کالا به سبد با اعتبارسنجی</summary>
    public void AddItem(long productId, int quantity, decimal unitPrice)
    {
        EnsureActive();
        ValidateQuantity(quantity);

        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing != null)
        {
            // به‌روزرسانی تعداد
            UpdateQuantityInternal(existing, quantity, unitPrice);
        }
        else
        {
            var newItem = new BasketItem(productId, quantity, unitPrice);
            ValidateTotalPrice(newItem);
            _items.Add(newItem);
        }

        LastUpdatedAt = DateTime.UtcNow;
        _domainEvents.Add(new BasketItemAddedDomainEvent(Id, UserId, productId, quantity));
    }

    /// <summary>به‌روزرسانی تعداد یک کالا</summary>
    public void UpdateQuantity(long productId, int newQuantity, decimal unitPrice)
    {
        EnsureActive();
        ValidateQuantity(newQuantity);

        var item = _items.FirstOrDefault(i => i.ProductId == productId)
            ?? throw new InvalidOperationException("کالا در سبد وجود ندارد.");

        // محاسبه تغییرات و اعتبارسنجی مجموع قیمت
        var oldTotal = item.Quantity * item.UnitPrice;
        var newTotal = newQuantity * unitPrice;
        var diff = newTotal - oldTotal;
        if (TotalPrice() + diff > 50_000_000)
            throw new InvalidOperationException("مجموع قیمت سبد نباید از ۵۰,۰۰۰,۰۰۰ ریال بیشتر شود.");

        item.UpdateQuantity(newQuantity);
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>حذف یک کالا از سبد</summary>
    public void RemoveItem(long productId)
    {
        EnsureActive();
        var item = _items.FirstOrDefault(i => i.ProductId == productId)
            ?? throw new InvalidOperationException("کالا در سبد وجود ندارد.");
        _items.Remove(item);
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>خالی کردن کامل سبد</summary>
    public void Clear()
    {
        EnsureActive();
        _items.Clear();
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>منقضی کردن سبد و ایجاد رویداد</summary>
    public void Expire()
    {
        if (Status == BasketStatus.Expired) return;
        Status = BasketStatus.Expired;
        LastUpdatedAt = DateTime.UtcNow;
        _domainEvents.Add(new BasketExpiredEvent(Id, UserId));
    }

    private void EnsureActive()
    {
        if (Status == BasketStatus.Expired)
            throw new InvalidOperationException("سبد خرید منقضی شده است.");
    }

    private void ValidateQuantity(int quantity)
    {
        if (quantity < 1 || quantity > 10)
            throw new ArgumentOutOfRangeException(nameof(quantity), "تعداد باید بین ۱ تا ۱۰ باشد.");
    }

    private void ValidateTotalPrice(BasketItem? newItem = null)
    {
        var total = TotalPrice() + (newItem?.Quantity * newItem?.UnitPrice ?? 0);
        if (total > 50_000_000)
            throw new InvalidOperationException("مجموع قیمت سبد نباید از ۵۰,۰۰۰,۰۰۰ ریال بیشتر شود.");
    }

    private decimal TotalPrice() => _items.Sum(i => i.Quantity * i.UnitPrice);

    private void UpdateQuantityInternal(BasketItem item, int newQuantity, decimal newUnitPrice)
    {
        // در این پیاده‌سازی فرض می‌کنیم قیمت واحد ثابت می‌ماند
        // ولی در صورت نیاز می‌توان UnitPrice را نیز به‌روز کرد
        var oldTotal = item.Quantity * item.UnitPrice;
        var newTotal = newQuantity * item.UnitPrice;
        if (TotalPrice() - oldTotal + newTotal > 50_000_000)
            throw new InvalidOperationException("مجموع قیمت سبد نباید از ۵۰,۰۰۰,۰۰۰ ریال بیشتر شود.");
        item.UpdateQuantity(newQuantity);
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}