using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Scheduling.Data;
using Scheduling.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Services
{
    public interface ISubBlockService
    {
        Task<SubBlockViewModel> GetSubBlockById(int id);
    }

    public class SubBlockService : ISubBlockService
    {
        private readonly IMapper _mapper;
        private MainDBContext _mainDBContext;
        private readonly ILogger<SubBlockService> _logger;

        public SubBlockService(ILogger<SubBlockService> logger,
                MainDBContext uciDBContext,
                IMapper mapper
            )
        {
            _mapper = mapper;
            _mainDBContext = uciDBContext;
            _logger = logger;
        }

        public async Task<SubBlockViewModel> GetSubBlockById(int id)
        {
            try
            {
                var subblock = await _mainDBContext.SubBlock.Where(x => x.SubblockId == id).FirstOrDefaultAsync();
                return _mapper.Map<SubBlockViewModel>(subblock);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(SubBlockService) }.{ nameof(GetSubBlockById) }]{ ex }");
                throw ex;
            }
        }
    }
}
