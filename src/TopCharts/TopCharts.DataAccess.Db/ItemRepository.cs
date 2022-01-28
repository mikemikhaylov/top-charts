using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using TopCharts.DataAccess.Abstractions;
using TopCharts.Domain.Model.Api;

namespace TopCharts.DataAccess.Db
{
    public class ItemRepository:MongoDbRepositoryBase<Item>, IItemRepository
    {
        public async Task SaveAsync(Item item, CancellationToken cancellationToken)
        {
            var site = item.Site;
            var id = item.Data.Id;
            await MongoDbContext.Items.ReplaceOneAsync(x => x.Site == site && x.Data.Id == id, item, new ReplaceOptions { IsUpsert = true }, cancellationToken);
        }

        protected ItemRepository(MongoDbContext mongoDbContext) : base(mongoDbContext)
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