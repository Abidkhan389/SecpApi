namespace Paradigm.Service.Repository
{
    using Paradigm.Data;    
    using Paradigm.Data.Model;
    using Paradigm.Contract.Interface;

    public interface ISecurityClaimRepository : IEntityRepository<SecurityClaim> { }
    public class SecurityClaimRepository : EntityRepository<SecurityClaim>, ISecurityClaimRepository
    {
        public SecurityClaimRepository(DbContextBase context) : base(context) { }
    }
}