namespace SharedKernel.Constant.Roles;
public static class SystemRoles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string Individual = "Individual";
    public const string Corporate = "Corporate";
    public const string Agent = "Agent";
    public const string FoDo = "FoDo";

    public const int SuperAdminLevel = 999;

    public const int AdminLevel = 500;
    public const int AgentLevel = 300;
    public const int FoDoLevel = 300;

    public const int CustomerLevel = 200;
    public const int CorporateLevel = 200;


    public static List<string> GetDefaultAllRoles()
    {
        return new List<string>
            {
                SuperAdmin,
                Admin,
                Individual,
                Corporate,
                Agent,
                FoDo
            };
    }

    public static List<string> GetAllDefaultRolesExceptSuperAdminAndCustomer()
    {
        return new List<string>
            {
                Agent,
                Admin,
                FoDo,
            };
    }

    public static HashSet<string> GetNotDeletableRoles() => new HashSet<string>
            {
                SuperAdmin,
                Corporate,
                Individual,
                Agent,
                Admin,
                FoDo
            };
}

