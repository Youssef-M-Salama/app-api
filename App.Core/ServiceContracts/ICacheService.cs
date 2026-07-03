using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.ServiceContracts
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);

        Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null);

        Task<bool> RemoveAsync(string key);

        Task<bool> RemoveByPrefixAsync(string prefixKey);

        Task<bool> ExistsAsync(string key);
    }
}
