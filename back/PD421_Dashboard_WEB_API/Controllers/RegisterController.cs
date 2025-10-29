using Microsoft.AspNetCore.Mvc;
using PD421_Dashboard_WEB_API.BLL.Dtos.Register;
using PD421_Dashboard_WEB_API.BLL.Services.Register;
using PD421_Dashboard_WEB_API.Extensions;


namespace PD421_Dashboard_WEB_API.Controllers
{
    [ApiController]
    [Route("api/register")]
    public class RegisterController : ControllerBase
    {
        private readonly IRegisterService _registerService;
        public RegisterController(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto dto)
        {
            var response = await _registerService.RegisterAsync(dto);
            return this.ToActionResult(response);
        }
    }
}
