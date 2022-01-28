using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using TopCharts.DataAccess.Abstractions;
using TopCharts.Domain.Model;

namespace TopCharts.DataAccess.Db
{
    public class KeyValueRepository: MongoDbRepositoryBase<KeyValue>, IKeyValueRepository
    {
        public async Task<string> GetAsync(Site site, string key, CancellationToken cancellationToken)
        {
            return (await MongoDbContext.KeyValues
                .Find(x => x.Site == site && x.Key == key).SingleOrDefaultAsync(cancellationToken))?.Value;
        }

        public async Task SetAsync(Site site, string key, string value, CancellationToken cancellationToken)
        {
            await MongoDbContext.KeyValues.ReplaceOneAsync(x => x.Site == site && x.Key == key, new KeyValue()
            {
                Key = key,
                Value = value,
                Site = site,
            }, new ReplaceOptions { IsUpsert = true }, cancellationToken);
        }

        protected KeyValueRepository(MongoDbContext mongoDbContext) : base(mongoDbContext)
        {
            var options = new CreateIndexOptions<KeyValue>
            {
                Unique = true
            };
            var indexKeysDefinition = Builders<KeyValue>.IndexKeys.Ascending(x => x.Site).Ascending(x=>x.Key);
            mongoDbContext.KeyValues.Indexes.CreateOne(new CreateIndexModel<KeyValue>(indexKeysDefinition, options));
        }
    }
}