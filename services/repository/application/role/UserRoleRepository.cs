namespace Paradigm.Service.Repository
{
    using Paradigm.Data;    
    using Paradigm.Data.Model;
    using Paradigm.Contract.Interface;

    public interface IUserRoleRepository : IEntityRepository<UserRole> { }
    public class UserRoleRepository : EntityRepository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(DbContextBase context) : base(context) { }
    }
}