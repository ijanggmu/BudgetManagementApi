using System;

namespace BeemaEdgeApi.Filters.AuthorizationFilters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class SkipIndividualAuthorizationAttribute : Attribute
{
}

