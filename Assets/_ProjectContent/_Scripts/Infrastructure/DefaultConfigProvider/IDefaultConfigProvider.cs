using Newtonsoft.Json.Linq;

namespace VContainerTemplate.Infrastructure.Providers.DefaultConfigProvider
{
    public interface IDefaultConfigProvider
    {
        public JToken CachedConfig { get; }
    }
}