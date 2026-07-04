using BasketManagement.Application.Common;

namespace BasketManagement.Application.Basket.Commands.ExpireBaskets;

public record ExpireBasketsCommand : ICommand<ServiceResult>;