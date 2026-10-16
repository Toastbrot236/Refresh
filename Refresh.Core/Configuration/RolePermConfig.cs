using System.Diagnostics.CodeAnalysis;
using Bunkum.Core.Configuration;
using Microsoft.CSharp.RuntimeBinder;

namespace Refresh.Core.Configuration;

public class RolePermConfig : Config
{
    public override int CurrentConfigVersion => 1;
    public override int Version { get; set; } = 0;
    
    protected override void Migrate(int oldVer, dynamic oldConfig)
    {
        
    }

    /// <summary>
    /// Role-specific permissions for new users.
    /// </summary>
    public RolePermissions NewUserPermissions = new();
    /// <summary>
    /// Role-specific permissions for normal, not-new users.
    /// </summary>
    public RolePermissions NormalUserPermissions = new();
    /// <summary>
    /// Role-specific permissions for trusted users and above.
    /// </summary>
    public RolePermissions TrustedUserPermissions = new();

    /// <summary>
    /// The minimum age (time after registration) an account must have before it automatically gets promoted from
    /// NewUser to User.
    /// We do this by using NewUserJob to compare the account's age, and if it is old enough, we update their role.
    /// Currently, the only difference between new users and regular users is that we apply NewUserPermissions
    /// instead of NormalUserPermissions, which you can freely configure in this config.
    /// </summary>
    public int HoursUntilNewAccountNoLongerNew { get; set; } = 24 * 7; // TODO should we think of a better name?
}