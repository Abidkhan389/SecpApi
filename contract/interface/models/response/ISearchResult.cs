namespace Paradigm.Contract.Model
{
    using System.Collections.Generic;
   
    public interface ISearchResult<TOut>
    {
        IEnumerable<TOut> Items { get; }
        int Page { get; }
        int PageSize { get; }
        long Total { get; }
    }
}
