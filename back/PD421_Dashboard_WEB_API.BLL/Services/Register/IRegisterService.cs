using PD421_Dashboard_WEB_API.BLL.Dtos.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PD421_Dashboard_WEB_API.BLL.Services.Register
{
    public interface IRegisterService
    {
        Task<ServiceResponse> RegisterAsync(RegisterDto dto);

    }
}
