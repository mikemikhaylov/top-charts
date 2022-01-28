namespace TopCharts.DataAccess.Db
{
    public class MongoDbRepositoryBase<T>
    {
        private static bool _init;
        private static readonly object InitLock = new object();
        private readonly MongoDbContext _mongoDbContext;

        protected MongoDbContext MongoDbContext
        {
            get
            {
                if (!_init)
                {
                    lock (InitLock)
                    {
                        if (!_init)
                        {
                            InitAction(_mongoDbContext);
                            _init = true;
                        }
                    }
                }
                return _mongoDbContext;
            }
        }

        protected MongoDbRepositoryBase(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }

        protected virtual void InitAction(MongoDbContext mongoDbContext)
        {
            
        }
    }
}