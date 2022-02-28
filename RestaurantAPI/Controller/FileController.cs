using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI.Controller
{
    [Route("file")]
    [Authorize]
    public class FileController : ControllerBase
    {
        [HttpGet]
        [ResponseCache(Duration =1200,VaryByQueryKeys = new[] {"filename"})]
        public ActionResult Get([FromQuery]string fileName)
        {
            var dir = Directory.GetCurrentDirectory();
            var path = $"{dir}/PrivateFiles/{fileName}";
            var pathExists = System.IO.File.Exists(path);
            if (!pathExists)
            {
                return NotFound("HWDP");
            }

            var fileContent = System.IO.File.ReadAllBytes(path);
            var contentProvider = new FileExtensionContentTypeProvider();
            contentProvider.TryGetContentType(path, out string contentType);

            return File(fileContent, contentType, path);
        }

        [HttpPost]
        public ActionResult Upload([FromForm]IFormFile file)
        {
            if(file!=null && file.Length > 0)
            {
                var dir = Directory.GetCurrentDirectory();
                var fileName = file.FileName;
                var path = $"{dir}/PrivateFiles/{fileName}";
                using(var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return Ok();
            }
            return BadRequest();
        }
    }
}
