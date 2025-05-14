using Base.DataAccess.Interfaces;
using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text;
using System.Reflection.Metadata;

namespace Base.DataAccess.Services
{
    public class DatabaseServiceWithDecorator : IDataBaseService
    {
        private readonly IDataBaseService _inner;
        private readonly ILogger<DatabaseServiceWithDecorator> _logger;
        private readonly IMemoryCache _cache;

        public DatabaseServiceWithDecorator(IDataBaseService inner, ILogger<DatabaseServiceWithDecorator> logger, IMemoryCache cache)
        {
            _inner = inner;
            _logger = logger;
            _cache = cache;
        }
        public async Task<DataSet> ExcuteStoreAsync(string StoreName, dynamic Params)
        {
            string cacheKey = GenerateCacheKey(StoreName, Params);

            if (_cache.TryGetValue(cacheKey, out DataSet cachedResult))
            {
                _logger.LogInformation($"[CACHE HIT] {StoreName} with key {cacheKey}");
                return cachedResult;
            }

            try
            {
                _logger.LogInformation($"[EXEC SP] {StoreName} with params {JsonSerializer.Serialize(Params)}");

                DataSet result = await _inner.ExcuteStoreAsync(StoreName, Params);

                _cache.Set(cacheKey, result, TimeSpan.FromSeconds(60));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error executing stored procedure {StoreName}");
                throw;
            }
        }

        private string GenerateCacheKey(string procedureName, dynamic parameters)
        {
            var json = JsonSerializer.Serialize(parameters);
            using var md5 = MD5.Create();
            var hash = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(json))).Replace("-", "");
            return $"{procedureName}_{hash}";
        }
    }
}
