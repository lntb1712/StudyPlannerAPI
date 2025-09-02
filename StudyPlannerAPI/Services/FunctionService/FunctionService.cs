using StudyPlannerAPI.DTO;
using StudyPlannerAPI.DTOs.FunctionDTO;
using StudyPlannerAPI.Repositories.FunctionRepository;

namespace StudyPlannerAPI.Services.FunctionService
{
    public class FunctionService:IFunctionService
    {
        private readonly IFunctionRepository _functionRepository;
        public FunctionService(IFunctionRepository functionRepository)
        {
            _functionRepository = functionRepository;
        }

        public async Task<ServiceResponse<List<FunctionResponseDTO>>> GetAllFunctions()
        {
            var lstFunction = await _functionRepository.GetFunctionsAsync();
            if (lstFunction == null || lstFunction.Count == 0)
            {
                return new ServiceResponse<List<FunctionResponseDTO>>(false, "Không có dữ liệu chức năng", new List<FunctionResponseDTO>());
            }

            var lstFunctionDTO = lstFunction.Select(x => new FunctionResponseDTO
            {
                FunctionId = x.FunctionId,
                FunctionName = x.FunctionName,
                IsEnable = false,
                IsReadOnly = false,
            }).ToList();

            return new ServiceResponse<List<FunctionResponseDTO>>(true, "Lấy dữ liệu chức năng thành công", lstFunctionDTO);
        }
    }
}
