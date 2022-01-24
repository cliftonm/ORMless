using Microsoft.AspNetCore.Mvc;

using Interfaces;

namespace Plugin
{
    [ApiController]
    [Route("[controller]")]
    public class PluginController : ControllerBase
    {
        private IPluginService ps;
        private IPlugin2Service ps2;
        private IApplicationService appSvc;

        public PluginController(IPluginService ps, IPlugin2Service ps2, IApplicationService appSvc)
        {
            this.ps = ps;
            this.ps2 = ps2;
            this.appSvc = appSvc;
        }

        [HttpGet("Version")]
        public object Version()
        {
            return $"Plugin Controller v 1.0 {ps.Test()} {appSvc.Test()} and 1 + 2 = {ps2.Add(1, 2)}";
        }
    }
}