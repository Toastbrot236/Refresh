using Refresh.Database.Models.Users;

namespace Refresh.Core.Authentication.Permission;

[AttributeUsage(AttributeTargets.Method)]
public class MinimumRoleAttribute : Attribute // TODO: DocAttribute
{
    public GameUserRole MinimumRole { get; init; }

    public MinimumRoleAttribute(GameUserRole minimumRole)
    {
        this.MinimumRole = minimumRole;
    }
}