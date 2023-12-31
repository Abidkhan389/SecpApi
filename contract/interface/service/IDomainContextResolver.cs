namespace Paradigm.Contract.Interface
{
    public interface IDomainContextResolver
    {
        IDomainContext Resolve(bool cache = true);
    }
}