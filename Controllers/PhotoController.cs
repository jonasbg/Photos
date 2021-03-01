using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace photos.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PhotosController : ControllerBase
    {
        private readonly ILogger<PhotosController> _logger;

        public PhotosController(ILogger<PhotosController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            DirectoryInfo info = new DirectoryInfo("/data");
            var files = info.GetFiles("*.*", System.IO.SearchOption.AllDirectories).OrderBy(p => p.CreationTime).Select(p => p.FullName).ToArray();

            return files;
        }

        [HttpGet("{index}")]
        public IActionResult Get(int index)
        {
            var files = Get();
            var image = System.IO.File.OpenRead(files.ToArray()[--index]);
            return File(image, "image/jpeg");
        }
    }
}
