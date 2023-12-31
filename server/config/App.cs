namespace Paradigm.Server
{
    public class AppConfig
    {
        public AppConfig()
        {
            
        }
        
        public Config Server { get; set; }
        public Data.Config Data { get; set; }
        public Service.Config Service { get; set; }
    }
}
