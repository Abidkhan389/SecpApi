namespace Paradigm.Service.Repository
{
    using Paradigm.Data;    
    using Paradigm.Data.Model;
    using Paradigm.Contract.Interface;

    public interface IRoleSecurityClaimRepository : IEntityRepository<RoleSecurityClaim> { }
    public class RoleSecurityClaimRepository : EntityRepository<RoleSecurityClaim>, IRoleSecurityClaimRepository
    {
        public RoleSecurityClaimRepository(DbContextBase context) : base(context) { }
    }
}