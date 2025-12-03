using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using WebApplication1.Repository;
using ZLogger;

namespace WebApplication1.Controllers
{
    public class PKLoginRequest 
    {
        public string ID { get; set; }
        public string PW { get; set; }
    }

    public class PKLoginResponse 
    {
        public ErrorCode Result { get; set; }
        public string AuthToken { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IGameDB _gamdDB;
        private readonly IMemoryDB _memoryDB;
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger,IGameDB gamdDB, IMemoryDB memoryDB)
        {
            _gamdDB = gamdDB;
            _memoryDB = memoryDB;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult DebugGet()
        {
            _logger.LogInformation("GET /api/login 호출됨!");
            return Ok("LoginController GET OK");
        }

        [HttpPost]
        public async Task<PKLoginResponse> Post(PKLoginRequest request) 
        {
            //Console.WriteLine($"[Request Login] Id : {request.ID} , PW:{request.PW}");

            _logger.ZLogInformation($"[Request Login] ID : {request.ID}, PW:{request.PW}");

            var response = new PKLoginResponse();

            // ID, PW 검증
            (ErrorCode errorCode, long uid) = await _gamdDB.AuthCheck(request.ID, request.PW);
            if (errorCode != ErrorCode.None) 
            {
                response.Result = errorCode;
                return response;
            }

            string authToken = CreateAuthToken();
            errorCode = await _memoryDB.RegistUserAsync(request.ID, authToken, uid);
            if(errorCode != ErrorCode.None) 
            {
                response.Result = errorCode;
                return response;
            }

            response.AuthToken = authToken;
            return response;
        }

        private const String AllowableCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";
        public string CreateAuthToken() 
        {
            var bytes = new Byte[25];
            using(var random = RandomNumberGenerator.Create()) 
            {
                random.GetBytes(bytes);
            }

            return new String(bytes.Select(x => AllowableCharacters[x % AllowableCharacters.Length]).ToArray());
        }
    }

}
