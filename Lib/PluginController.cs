using Microsoft.AspNetCore.Mvc;

namespace Lib
{
    public abstract class PluginController : ControllerBase
    {
        public abstract string Version { get; }

        [HttpGet("Version")]
        public ActionResult GetVersion()
        {
            return Ok(new { Version = Version });
        }
    }
}
