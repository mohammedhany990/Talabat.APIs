using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;

namespace Talabat.Repository
{
    public class BasketReository : IBasketRepository
    {
        private IDatabase _database;
        public BasketReository(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }


        public async Task<CustomerBasket?> GetBasketAsync(string BasketId)
        {
            var Basket = await _database.StringGetAsync(BasketId);
            return Basket.IsNull ? null : JsonSerializer.Deserialize<CustomerBasket>(Basket);
        }


        public async Task<bool> DeleteBasketAsync(string BasketId)
        {
            return await _database.KeyDeleteAsync(BasketId);
        }

        
        public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket Basket)
        {
            var JsonBasket = JsonSerializer.Serialize(Basket);
            var CreatedOrUpdated = await _database.StringSetAsync(Basket.Id, JsonBasket, TimeSpan.FromDays(2));
            if (!CreatedOrUpdated)
            {
                return null;
            }
            return await GetBasketAsync(Basket.Id);
        }
    }
}
