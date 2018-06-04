using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesitServer.Models.Central;
using DesitServer.Models.Central.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DesitServer.Controllers
{
    [Route("api/[controller]")]
    public class CentralesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            List<CentralMonitoreo> centrales = CentralMonitoreo.GetAll();
            List<object> res = new List<object>();

            foreach (CentralMonitoreo central in centrales)
            {
                CentralLog log = CentralLog.GetLast(central.CentralID);
                int estado = 0;
                if (log != null) estado = log.TipoLog.TipoLogId.GetValueOrDefault();

                res.Add(new { Id = central.CentralID, Barrio = central.Barrio.Nombre, estado });
            }

            return Ok(res);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            List<CentralLog> logs = CentralLog.GetAll(id);

            List<object> res = new List<object>();
            foreach(CentralLog log in logs)
            {

                res.Add(new { Fecha = log.Fecha, estado = log.TipoLog.TipoLogId.GetValueOrDefault() });
            }

            return Ok(res);
        }
    }
}