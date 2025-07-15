using AspireApp.MasterDataService.Models;

namespace AspireApp.MasterDataService.Interfaces
{
    public interface ILocationService
    {
        Task<IEnumerable<Location>> GetAllAsync();
        Task<Location?> GetByIdAsync(int id);
        Task<int> CreateAsync(Location location);
        Task<bool> UpdateAsync(Location location);
        Task<bool> DeleteAsync(int id);
    }
}