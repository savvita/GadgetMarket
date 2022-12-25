using GadgetMarket.Model;

namespace GadgetMarket.Repositories
{
    public interface IGadgetsRepository
    {
        Task<List<Gadget>> GetAllGadgetsAsync();
        Task<GadgetsPageModel> GetGadgetsAsync(int page, int perPage);
        Task<List<Gadget>> GetGadgetsByCategoryIdAsync(int id);
        Task<GadgetsPageModel> GetGadgetsByCategoryIdAsync(int id, int page, int perPage);
        Task<GadgetsPageModel> GetGadgetsByNameAsync(string name, int page, int perPage);
        Task<List<Gadget>> GetGadgetsByOwnerIdAsync(int id);
        Task<Gadget?> GetGadgetByIdAsync(int id);
        Task<Gadget> CreateGadgetAsync(Gadget gadget);
        Task<Gadget> UpdateGadgetAsync(Gadget gadget);
        Task<User?> GetGadgetOwnerByGadgetIdAsync(int id);
        Task<bool> RemoveGadgetByIdAsync(int id);
        Task<bool> RemoveAllGadgetsByOwnerIdAsync(int id);
        Task<bool> PassGadgetsToUserAsync(int ownerId, int inheritorId);
    }
}
