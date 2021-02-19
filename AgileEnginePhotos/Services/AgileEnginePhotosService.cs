using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AgileEnginePhotos.Models;
using API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace API.Services
{
    public class AgileEnginePhotosService : IAgileEnginePhotosService
    {
        private readonly HttpClient httpClient;

        private ILogger<AgileEnginePhotosService> logger;

        public AgileEnginePhotosService(HttpClient httpClient, ILogger<AgileEnginePhotosService> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<List<PicturesMetadataModel>> GetPicturesMetadata()
        {
            logger.LogInformation("Pulling pictures metadata from AgileEngine endpoint");
            try
            {
                List<PicturesMetadataModel> picturesMetadata = new List<PicturesMetadataModel>();
                AgileEnginePhotosModel photosResponseModel;
                int page = 1;

                do
                {
                    var response = await httpClient.GetAsync("images" + (page > 1 ? $"?page={page}" : string.Empty));
                    response.EnsureSuccessStatusCode();
                    photosResponseModel = JsonConvert.DeserializeObject<AgileEnginePhotosModel>(await response.Content.ReadAsStringAsync());
                    picturesMetadata.AddRange(photosResponseModel.Pictures);
                    page++;
                } while (photosResponseModel.HasMore);

                return picturesMetadata;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error has occurred while trying to pull the images metadata. Details: {ex.Message}");
                throw;
            }
        }

        public async Task<List<PicturesFullDataModel>> GetPicturesFullData(List<PicturesMetadataModel> picturesMetadataModels)
        {
            logger.LogInformation("Pulling full pictures data from AgileEngine endpoint");
            try
            {
                List<PicturesFullDataModel> picturesFullData = new List<PicturesFullDataModel>();

                foreach (string pictureId in picturesMetadataModels.Select(e => e.Id))
                {
                    var response = await httpClient.GetAsync($"images/{pictureId}");
                    response.EnsureSuccessStatusCode();
                    picturesFullData.Add(JsonConvert.DeserializeObject<PicturesFullDataModel>(await response.Content.ReadAsStringAsync()));
                }

                return picturesFullData;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error has occurred while trying to pull the images full data. Details: {ex.Message}");
                throw;
            }
        }
    }
}
