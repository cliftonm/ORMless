using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Clifton;
using Lib;

namespace Demo.Controllers
{
    public class VersionResult
    {
        public string Version { get; set; }
    }

    public class PluginVersion
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Error { get; set; }
    }

    public class VersionResponse
    {
        public string Version { get; set; }
        public List<PluginVersion> Plugins = new List<PluginVersion>();
    }

    [ApiController]
    [Route("[controller]")]
    public class PublicController : ControllerBase
    {
        public const string VERSION = "1.00";

        public PublicController()
        {
        }

        [AllowAnonymous]
        [HttpGet("Version")]
        public ActionResult Version()
        {
            var request = HttpContext.Request;
            var basePath = $"{request.Scheme}://{request.Host}{request.PathBase}";
            var response = new VersionResponse() { Version = VERSION };

            AppSettings.Settings.Plugins.ForEach(plugin =>
            {
                // This assumes that the service is a DLL and that it that name is "[something]Service.dll"
                var endpoint = plugin.Path.RightOfRightmostOf("\\").LeftOf(".dll").Replace("Service", "");
                var url = $"{basePath}/{endpoint}/Version";
                var pv = new PluginVersion() { Name = endpoint };

                try
                {
                    var ret = RestService.Get<VersionResult>(url);

                    if (ret.status == HttpStatusCode.OK)
                    {
                        pv.Version = ret.item.Version;
                    }
                    else
                    {
                        pv.Error = String.IsNullOrEmpty(ret.content) ? ret.status.ToString() : ret.content;
                    }
                }
                catch(Exception ex)
                {
                    pv.Error = ex.Message;
                }
                finally
                {
                    response.Plugins.Add(pv);
                }
            });

            response.Plugins = response.Plugins.OrderBy(p => p.Name).ToList();

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("AppSettings")]
        public ActionResult GetAppSettings()
        {
            return Ok(AppSettings.Settings);
        }

        [AllowAnonymous]
        [HttpGet("TestException")]
        public void TestException()
        {
            throw new Exception("Exception occurred!");
        }

        [Authorize]
        [HttpGet("TestAuthentication")]
        public ActionResult TestAuthentication()
        {
            return Ok();
        }
    }
}
