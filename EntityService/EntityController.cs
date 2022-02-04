using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Interfaces;
using Lib;

using Parameters = System.Collections.Generic.Dictionary<string, object>;

namespace Clifton.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EntityController : PluginController
    {
        public override string Version => "1.00";

        private readonly IEntityService ts;

        public EntityController(IEntityService ts)
        {
            this.ts = ts;
        }

        // TODO: Pagination?
        [Authorize()]
        [HttpGet("{entityName}")]
        public ActionResult GetAll(string entityName)
        {
            var result = ts.GetAll(entityName);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("{entityName}/{entityId}")]
        public ActionResult GetById(string entityName, int entityId)
        {
            var result = ts.GetById(entityName, entityId);
            var ret = result == null ? (ActionResult)NotFound() : Ok(result);

            return ret;
        }

        [Authorize(Policy = "entityAuthorization")]
        [HttpPost("{entityName}")]
        public ActionResult Insert(string entityName, Parameters data)
        {
            var result = ts.Insert(entityName, data);

            return Ok(result);
        }

        [Authorize]
        [HttpPatch("{entityName}/{entityId}")]
        public ActionResult Update(string entityName, int entityId, Parameters data)
        {
            var result = ts.Update(entityName, entityId, data);

            return Ok(result);
        }

        // REMOVE webDAV for this to work! https://www.c-sharpcorner.com/forums/webapi-delete-405-method-not-allowed
        // Or, if webDAV isn't configured in IIS, this will cause a failure in the web.config file:
        // <modules>
        //   <remove name = "WebDAVModule" />
        // </ modules >
        [Authorize]
        [HttpDelete("{entityName}/{entityId}")]
        public ActionResult SoftDelete(string entityName, int entityId)
        {
            ts.SoftDelete(entityName, entityId);

            return NoContent();
        }

        // REMOVE webDAV for this to work! https://www.c-sharpcorner.com/forums/webapi-delete-405-method-not-allowed
        // Or, if webDAV isn't configured in IIS, this will cause a failure in the web.config file:
        // <modules>
        //   <remove name = "WebDAVModule" />
        // </ modules >
        [Authorize]
        [HttpDelete("{entityName}/{entityId}/Hard")]
        public ActionResult HardDelete(string entityName, int entityId)
        {
            ts.HardDelete(entityName, entityId);

            return NoContent();
        }
    }
}
