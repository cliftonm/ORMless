using System.Linq;

// if we add [Authorize] tags.
// using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Interfaces;

namespace Clifton.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MigratorController : ControllerBase
    {
        private readonly IMigratorService ms;
        private readonly IAppDbContext context;

        public MigratorController(IMigratorService ms, IAppDbContext context)
        {
            this.ms = ms;
            this.context = context;
        }

        [HttpGet("VersionInfo")]
        public ActionResult VersionInfo()
        {
            var recs = context.VersionInfo.OrderByDescending(v => v.Version);

            return Ok(recs);
        }

        [HttpGet("MigrateUp")]
        public ActionResult MigrateUp()
        {
            var resp = ms.MigrateUp();

            return Ok(resp);
        }
    }
}
