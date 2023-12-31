namespace Paradigm.Contract.Interface
{
    using Paradigm.Contract.Model;

    public interface IDeviceProfiler
    {
        string DeriveFingerprint(IUser user);
    }
}