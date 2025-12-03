using CloudStructures;
using CloudStructures.Structures;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

public class RdbAuthUserData 
{
    public string AuthToken { get; set; } = "";
    public long UId { get; set; } = 0;
}

public class MemoryDbKeyMaker 
{
    const string longinUID = "UID_";

    public static string MakeUIDKey(string id) 
    {
        return longinUID + id;
    }
}

namespace WebApplication1.Repository
{
    public class MemoryDB : IMemoryDB
    {

        public RedisConnection _redisConn;

        public MemoryDB(IOptions<DbConfig> dbConfig) 
        {
            RedisConfig config = new("default", dbConfig.Value.Redis);
            _redisConn = new RedisConnection(config);
        }

        // IMemory인터페이스 구현시 필수
        public void Dispose()
        {
            
        }

        public async Task<ErrorCode> RegistUserAsync(string id, string authToken, long uid)
        {
            string key = MemoryDbKeyMaker.MakeUIDKey(id);
            ErrorCode result = ErrorCode.None;

            RdbAuthUserData user = new()
            {
                AuthToken = authToken,
                UId = uid
            };

            try
            {
                var expiryTime = TimeSpan.FromMinutes((60 * 24));
                RedisString<RdbAuthUserData> redis = new(_redisConn, key, expiryTime);

                if (await redis.SetAsync(user, expiryTime) == false)
                {
                    result = ErrorCode.LoginFailAddRedis;
                    return result;
                }
            }
            catch 
            {
                result = ErrorCode.LoginFailAddRedis;
                return result;
            }

            return result;
        }
    }


}
