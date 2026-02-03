namespace backend.Service.Common;

public interface ICurrentUserService
{
    int? GetUserId();
    string? GetUserRole();
    bool IsAdmin();
    bool IsNutricionista();
}
