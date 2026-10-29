using Newtonsoft.Json.Linq;

namespace VContainerTemplate.Infrastructure.DefaultConfigProvider
{
    public interface IDefaultConfigProvider
    {
        public JToken CachedConfig { get; }
    }
}