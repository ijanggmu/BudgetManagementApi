namespace SharedKernel.Constant.Roles;
public static class SystemRoles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string FoDo = "FoDo";

    public const int SuperAdminLevel = 999;
    public const int AdminLevel = 500;
    public const int FoDoLevel = 300;

    public static List<string> GetDefaultAllRoles()
    {
        return new List<string>
            {
                SuperAdmin,
                Admin,
                FoDo
            };
    }

    public static List<string> GetAllDefaultRolesExceptSuperAdmin()
    {
        return new List<string>
            {
                Admin,
                FoDo
            };
    }

    public static HashSet<string> GetNotDeletableRoles() => new HashSet<string>
            {
                SuperAdmin,
                Admin,
                FoDo
            };
}

