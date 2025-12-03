namespace WebApplication1.Repository
{
    public interface IMemoryDB : IDisposable
    {
        public Task<ErrorCode> RegistUserAsync(string id, string authToken, long accountId);
    }
}
