namespace Paradigm.Service.Model
{
    using Paradigm.Contract.Model;

    public class KeyValue : IKeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}