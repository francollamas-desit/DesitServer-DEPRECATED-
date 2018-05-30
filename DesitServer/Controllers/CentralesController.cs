using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesitServer.Models.Central;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DesitServer.Controllers
{
    [Route("api/[controller]")]
    public class CentralesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IActionResult Get(int id)
        {
            return Ok(CentralMonitoreo.GetCentralesConEstado());
        }
    }
}