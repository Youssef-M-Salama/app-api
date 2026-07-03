using App.Core.ServiceContracts;
using StackExchange.Redis;
using System.Text.Json;

namespace App.Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _db;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public CacheService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var value = await _db.StringGetAsync(key);

                if (value.IsNullOrEmpty)
                    return default;

                return JsonSerializer.Deserialize<T>(value.ToString(), _jsonOptions);
            }
            catch
            {
                return default;
            }
        }

        public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            try
            {
                var json = JsonSerializer.Serialize(value, _jsonOptions);

                return await _db.StringSetAsync(
                    key,
                    json,
                    expiry ?? TimeSpan.FromMinutes(5) 
                );
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveAsync(string key)
        {
            try
            {
                return await _db.KeyDeleteAsync(key);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveByPrefixAsync(string prefixKey)
        {
            try
            {
                var endpoints = _db.Multiplexer.GetEndPoints();
                foreach (var endpoint in endpoints)
                {
                    var server = _db.Multiplexer.GetServer(endpoint);
                    var keys = server.Keys(database: _db.Database, pattern: prefixKey + "*");
                    
                    var keysArray = keys.ToArray();
                    if (keysArray.Any())
                    {
                        await _db.KeyDeleteAsync(keysArray);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                return await _db.KeyExistsAsync(key);
            }
            catch
            {
                return false;
            }
        }
    }
}
