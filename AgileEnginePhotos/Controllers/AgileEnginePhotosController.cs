using System;
using System.Collections.Generic;
using System.Linq;
using AgileEnginePhotos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace API.Controllers
{
    [ApiController]
    [Route("api")]
    public class AgileEnginePhotosController : ControllerBase
    {
        private readonly IMemoryCache memoryCache;

        public AgileEnginePhotosController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        [HttpGet("photos")]
        public List<PicturesFullDataModel> GetPicturesFullData()
        {
            if (memoryCache.TryGetValue("Photos", out List<PicturesFullDataModel> picturesFullDataModels))
            {
                return picturesFullDataModels;
            }
            else
            {
                throw new Exception("Cache is not ready yet. Please try again in a few seconds");
            }
        }

        [HttpGet("photos/{id}")]
        public PicturesFullDataModel GetPictureFullDataById(string id)
        {
            if (memoryCache.TryGetValue("Photos", out List<PicturesFullDataModel> picturesFullDataModels))
            {
                return picturesFullDataModels.FirstOrDefault(e => e.Id.Equals(id));
            }
            else
            {
                throw new Exception("Cache is not ready yet. Please try again in a few seconds");
            }
        }

        [HttpGet("search")]
        public IEnumerable<PicturesFullDataModel> GetPicturesFullData(string author, string camera, string tags)
        {
            if (memoryCache.TryGetValue("Photos", out List<PicturesFullDataModel> picturesFullDataModels))
            {
                var tagsList = string.IsNullOrEmpty(tags) ? null : tags.Split(' ').ToList();

                return picturesFullDataModels.Where(e =>
                    (string.IsNullOrEmpty(author) || e.Author.Equals(author, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(camera) || (e.Camera != null && e.Camera.Equals(camera, StringComparison.OrdinalIgnoreCase))) &&
                    (string.IsNullOrEmpty(tags) || (e.Camera != null && e.Tags.Split(' ').ToList().Intersect(tagsList).Count() == tagsList.Count))
                );
            }
            else
            {
                throw new Exception("Cache is not ready yet. Please try again in a few seconds");
            }
        }
    }
}
