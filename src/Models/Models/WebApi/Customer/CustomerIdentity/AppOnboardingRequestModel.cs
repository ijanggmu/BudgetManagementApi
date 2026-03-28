namespace Models.BeemaEdgeApi.Customer.CustomerIdentity;

/// <summary>Update whether the current user has finished the in-app onboarding tour.</summary>
public class SetAppOnboardingCompletedRequestModel
{
    /// <summary>When true, the user will not be prompted for the tour again; false resets (e.g. "Restart tour").</summary>
    public bool Completed { get; set; }
}
