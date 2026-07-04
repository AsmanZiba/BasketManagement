using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketManagement.Application.Basket.DTOs;

public static class BasketMapper
{
    public static BasketDTO MapToDTO(Domain.Entities.Basket basket) => new()
    {
        Id = basket.Id,
        UserId = basket.UserId,
        Status = basket.Status.ToString(),
        Items = basket.Items.Select(i => new BasketItemDTO
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            TotalPrice = i.Quantity * i.UnitPrice
        }).ToList(),
        TotalPrice = basket.Items.Sum(i => i.Quantity * i.UnitPrice),
        CreatedAt = basket.CreatedAt,
        LastUpdatedAt = basket.LastUpdatedAt
    };
}
