namespace Paradigm.Common
{
    using StructureMap;
    
    using Paradigm.Contract.Interface;

    public class ContainerRegistry : Registry
    {
        public ContainerRegistry()
        {
            For<ICryptoService>().Use<CryptoHelper>();
        }
    }
}