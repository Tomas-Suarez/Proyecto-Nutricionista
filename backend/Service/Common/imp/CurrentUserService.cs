using System.Security.Claims;
using static backend.Enum.ERol;

namespace backend.Service.Common.imp;


public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? GetUserId()
    {
        var id = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return id != null ? int.Parse(id) : null;
    }

    public string? GetUserRole()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
    }

    public bool IsAdmin()
    {
        return GetUserRole() == nameof(Admin);
    }

    public bool IsNutricionista()
    {
        return GetUserRole() == nameof(Nutricionista);
    }
}
