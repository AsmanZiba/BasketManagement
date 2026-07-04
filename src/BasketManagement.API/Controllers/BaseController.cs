using Microsoft.AspNetCore.Mvc;
using BasketManagement.Application.Common;

namespace BasketManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    private ICommandDispatcher? _commandDispatcher;
    private IQueryDispatcher? _queryDispatcher;

    protected ICommandDispatcher CommandDispatcher =>
        _commandDispatcher ??= HttpContext.RequestServices.GetRequiredService<ICommandDispatcher>();

    protected IQueryDispatcher QueryDispatcher =>
        _queryDispatcher ??= HttpContext.RequestServices.GetRequiredService<IQueryDispatcher>();
}