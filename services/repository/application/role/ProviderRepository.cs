namespace Paradigm.Service.Repository
{
    using Paradigm.Data;    
    using Paradigm.Data.Model;
    using Paradigm.Contract.Interface;

    public interface IProviderRepository : IEntityRepository<Provider> { }
    public class ProviderRepository : EntityRepository<Provider>, IProviderRepository
    {
        public ProviderRepository(DbContextBase context) : base(context) { }
    }
}