using Newtonsoft.Json;

namespace VContainerTemplate.Infrastructure.Saving
{
    public interface IDataSaveable<out TSave> where TSave : class
    {
        SaveKey SaveKey { get; }

        [JsonProperty] TSave SaveData { get; }
    }
}