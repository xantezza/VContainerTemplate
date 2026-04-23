using VContainerTemplate.Infrastructure.Services.Log;

namespace VContainerTemplate.Utilities.Assertions
{
    public static class AssertionUtils
    {
        public static T AssertNotNull<T>(this T obj) where T : class
        {
            if (obj == null)
            {
                Log.Error($"Object of type {typeof(T)} is null");
            }
            return obj;
        }
    }
}