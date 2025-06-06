using Microsoft.AspNetCore.Mvc.Rendering;

namespace BakingIt.Services
{
    public interface IMeasureService
    {
        Task<IEnumerable<SelectListItem>> GetMeasuresSelectListAsync();
    }
}