namespace Paradigm.Service
{
    using StructureMap;

    using Paradigm.Common;
    using Paradigm.Service.Security;
    using Paradigm.Contract.Interface;
    using Paradigm.Service.Repository;

    public class ContainerRegistry : Registry
    {
        public ContainerRegistry()
        {
            For<ICryptoService>().Use<CryptoHelper>();
            For<ITokenProviderService<Token>>().Use<TokenProviderService>();
            For<ISecurityClaimsInspector>().Use<SecurityClaimsInspector>().Singleton();

            // Application - Role
            For<IProviderRepository>().Add<ProviderRepository>();
            For<IRoleRepository>().Add<RoleRepository>();
            For<IRoleSecurityClaimRepository>().Add<RoleSecurityClaimRepository>();
            For<ISecurityClaimRepository>().Add<SecurityClaimRepository>();
            For<IUserRepository>().Add<UserRepository>();
            For<ILocalAuthenticationService>().Add<UserRepository>();
            For<IUserRoleRepository>().Add<UserRoleRepository>();
        }
    }
}