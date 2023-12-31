namespace Paradigm.Contract.Interface
{
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Paradigm.Contract.Model;

    public interface IVerificationProvider
    {
        string Key { get; }
        bool CanHandle(IUser user, string code);
        Task<string> IssueCode(IUser user);
        Task<bool> RedeemCode(IUser user, string code);
    }
}