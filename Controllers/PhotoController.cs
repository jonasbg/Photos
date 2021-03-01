using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace photos.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PhotosController : ControllerBase
    {
        private readonly ILogger<PhotosController> _logger;
        private IMemoryCache _cache;


        public PhotosController(ILogger<PhotosController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _cache = memoryCache;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            DirectoryInfo info = new DirectoryInfo("/data");
            var files = info.GetFiles("*.*", System.IO.SearchOption.AllDirectories).OrderBy(p => p.CreationTime).Select(p => p.FullName).ToArray();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
            // Keep in cache for this time, reset time if accessed.
            .SetSlidingExpiration(TimeSpan.FromMinutes(10));

            // Save data in cache.
            _cache.Set("files", files, cacheEntryOptions);

            return files;
        }

        [HttpGet("{index}")]
        public IActionResult Get(int index)
        {
            IEnumerable<string> files;

            // Look for cache key.
            if (!_cache.TryGetValue("files", out files))
            {
                files = Get();
                _logger.LogInformation("Repopulating cache");
            } 

            var image = System.IO.File.OpenRead(files.ToArray()[--index]);
            return File(image, "image/jpeg");
            
        }
    }
}
