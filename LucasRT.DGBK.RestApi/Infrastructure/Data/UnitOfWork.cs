namespace LucasRT.DGBK.RestApi.Infrastructure.Data
{
    public class UnitOfWork(PostgreSQL dbContext) : IUnitOfWork
    {
        private readonly PostgreSQL _DBContext = dbContext;

        public PostgreSQL GetDbContext() => _DBContext;

        public async Task<bool> CommitDataContextAsync()
        {
            var success = await _DBContext.SaveChangesAsync() > 0;
            return success;
        }

        public void Dispose()
        {
            _DBContext.Dispose();
        }
    }

    public interface IUnitOfWork : IDisposable
    {
        PostgreSQL GetDbContext();
        Task<bool> CommitDataContextAsync();
    }
}
