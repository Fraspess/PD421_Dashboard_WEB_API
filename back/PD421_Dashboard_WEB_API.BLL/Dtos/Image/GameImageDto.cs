using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PD421_Dashboard_WEB_API.BLL.Dtos.Image
{
    public class GameImageDto
    {
        public string? Id { get; set; }
        public string? ImagePath { get; set; }
        public bool IsMain { get; set; } = false;
    }
}
