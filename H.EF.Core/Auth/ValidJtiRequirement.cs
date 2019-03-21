using Microsoft.AspNetCore.Authorization;

namespace H.EF.Core.Auth
{
    /// <summary>
    /// JWT验证
    /// </summary>
    public class ValidJtiRequirement : IAuthorizationRequirement
    {
    }
}
