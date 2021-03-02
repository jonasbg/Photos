using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
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
            var dir = "/data";
            //var dir = "/mnt/mykid/";
            DirectoryInfo info = new DirectoryInfo(dir);
            var files = info.GetFiles("*.*", System.IO.SearchOption.AllDirectories)
                .Where(p => !p.Name.EndsWith(".txt"))
                .OrderByDescending(p => p.CreationTime)
                .ToArray();

            var lastDay = files.First().CreationTime;
            var filess = files.Where(p => 
                p.CreationTime.Year == lastDay.Year &&
                p.CreationTime.Month == lastDay.Month &&
                p.CreationTime.Day == lastDay.Day
            ).Select(p => p.FullName);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
            // Keep in cache for this time, reset time if accessed.
            .SetSlidingExpiration(TimeSpan.FromMinutes(10));

            // Save data in cache.
            _cache.Set("files", filess, cacheEntryOptions);

            return filess;
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

            var file = System.IO.File.OpenRead(files.ToArray()[--index]);

            string contentType = "";
            if(file.Name.EndsWith("heic"))
                contentType = "image/heic";

            var fileProvider = new FileExtensionContentTypeProvider();
            // Figures out what the content type should be based on the file name.  
            if (string.IsNullOrEmpty(contentType))
            {
                if(!fileProvider.TryGetContentType(file.Name, out contentType))
                    throw new ArgumentOutOfRangeException($"Unable to find Content Type for file name {file.Name}.");
            }

            return File(file, contentType);
            
        }
    }
}
