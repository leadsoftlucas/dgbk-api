namespace LucasRT.DGBK.RestApi.Infrastructure.Data
{
    public class UnitOfWork(PostgreSQL dbContext) : IUnitOfWork
    {
        private readonly PostgreSQL _DBContext = dbContext;

        public PostgreSQL GetDbContext() => _DBContext;

        public async Task<bool> CommitDataContextAsync(CancellationToken ct)
            => await _DBContext.SaveChangesAsync(ct) > 0;

        public async Task<bool> CommitDataContextAsync()
            => await _DBContext.SaveChangesAsync() > 0;

        public void Dispose()
        {
            _DBContext.Dispose();
        }
    }

    public interface IUnitOfWork : IDisposable
    {
        PostgreSQL GetDbContext();
        Task<bool> CommitDataContextAsync(CancellationToken ct);
        Task<bool> CommitDataContextAsync();
    }
}
