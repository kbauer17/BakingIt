using BakingIt.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BakingIt.Services
{
    public class MeasureService : IMeasureService
    {
        private readonly IBakingItRepository<Measure> _measureRepository;

        public MeasureService(IBakingItRepository<Measure> measureRepository)
        {
            _measureRepository = measureRepository;
        }

        public async Task<IEnumerable<SelectListItem>> GetMeasuresSelectListAsync()
        {
            var measures = await _measureRepository.GetAllAsync();
            return measures.Select(m => new SelectListItem
            {
                Value = m.MeasureId.ToString(),
                Text = m.MeasureName
            });
        }
    }
}