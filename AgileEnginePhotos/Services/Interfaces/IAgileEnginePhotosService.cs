using System.Collections.Generic;
using System.Threading.Tasks;
using AgileEnginePhotos.Models;

namespace API.Services.Interfaces
{
    public interface IAgileEnginePhotosService
    {
        Task<List<PicturesMetadataModel>> GetPicturesMetadata();

        Task<List<PicturesFullDataModel>> GetPicturesFullData(List<PicturesMetadataModel> picturesMetadataModels);
    }
}
