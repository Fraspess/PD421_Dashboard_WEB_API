using PD421_Dashboard_WEB_API.BLL.Dtos.Auth;

namespace PD421_Dashboard_WEB_API.BLL.Services.Auth
{
    public interface IAuthService
    {
        Task<ServiceResponse> LoginAsync(LoginDto dto);
    }
}
