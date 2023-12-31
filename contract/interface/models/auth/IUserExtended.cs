namespace Paradigm.Contract.Model
{
    using System;
    using System.Collections.Generic;

    public interface IUserExtended : IUser
    {
        IEnumerable<string> Claims { get; }
        IEnumerable<string> Roles { get; }
    }
}