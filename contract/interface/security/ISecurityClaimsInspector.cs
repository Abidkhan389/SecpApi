namespace Paradigm.Contract.Interface
{
    using System.Security.Claims;
    
    using Paradigm.Contract.Constant;
    
    public interface ISecurityClaimsInspector
    {
        bool Satisifies(ClaimsPrincipal principal, ClaimRequirementType requirementType, string claimType, params object[] values);
    }
}