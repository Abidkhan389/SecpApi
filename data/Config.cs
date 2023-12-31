namespace Paradigm.Data
{
    public class Config
    {
        public Config() 
        {

        }
        
        public const string DbConnectionKey = @"data:connectionString";
        public const string RedisConnectionKey = @"data:redisConnectionString";
        public string ConnectionString { get; set; }
        public string RedisConnectionString { get; set; }
        public string SchemaName { get; set; }
    }
}
