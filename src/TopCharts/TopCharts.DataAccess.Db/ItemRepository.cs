using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using TopCharts.DataAccess.Abstractions;
using TopCharts.Domain.Model;
using TopCharts.Domain.Model.Api;

namespace TopCharts.DataAccess.Db
{
    public class ItemRepository:MongoDbRepositoryBase<Item>, IItemRepository
    {
        public ItemRepository(MongoDbContext mongoDbContext) : base(mongoDbContext)
        {
        }
        public async Task SaveAsync(Item item, CancellationToken cancellationToken)
        {
            var site = item.Site;
            var id = item.Data.Id;
            await MongoDbContext.Items.ReplaceOneAsync(x => x.Site == site && x.Data.Id == id, item, new ReplaceOptions { IsUpsert = true }, cancellationToken);
        }

        public async Task<List<Item>> GetAsync(Site site, DateTime @from, DateTime to, CancellationToken cancellationToken)
        {
            var fromTs = ((DateTimeOffset) from).ToUnixTimeSeconds();
            var toTs = ((DateTimeOffset) to).ToUnixTimeSeconds();
            return await MongoDbContext.Items
                .Find(x => x.Site == site && x.Data.Date >= fromTs && x.Data.Date < toTs).ToListAsync(cancellationToken);
        }

        protected override void InitAction(MongoDbContext mongoDbContext)
        {
            var options = new CreateIndexOptions<Item>
            {
                Unique = false
            };
            var indexKeysDefinition = Builders<Item>.IndexKeys.Ascending(x => x.Site).Ascending(x=>x.Data.Date);
            mongoDbContext.Items.Indexes.CreateOne(new CreateIndexModel<Item>(indexKeysDefinition, options));
            
            options = new CreateIndexOptions<Item>
            {
                Unique = true
            };
            indexKeysDefinition = Builders<Item>.IndexKeys.Ascending(x => x.Site).Ascending(x=>x.Data.Id);
            mongoDbContext.Items.Indexes.CreateOne(new CreateIndexModel<Item>(indexKeysDefinition, options));
        }
    }
}