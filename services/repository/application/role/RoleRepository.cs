namespace Paradigm.Service.Repository
{
    using Paradigm.Data;    
    using Paradigm.Data.Model;
    using Paradigm.Contract.Interface;

    public interface IRoleRepository : IEntityRepository<Role> { }
    public class RoleRepository : EntityRepository<Role>, IRoleRepository
    {
        public RoleRepository(DbContextBase context) : base(context) { }
    }
}