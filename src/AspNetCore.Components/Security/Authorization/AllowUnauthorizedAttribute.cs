namespace AspNetCore.Components.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AllowUnauthorizedAttribute : Attribute
{
}
