using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShampanPOS.Repository;
using System.Text.Json;
using System.Data;
using Microsoft.Extensions.Configuration;
using ShampanPOS.ViewModel.Utility;
using ShampanPOS.ViewModel.KendoCommon;

namespace ShampanPOS.Service
{
    public interface IRecentDateService
    {
        RecentDateVM GetRecentDate();
    }

}

namespace ShampanPOS.Service
{
    public class RecentDateService : IRecentDateService
    {
        private readonly IRecentDateRepository _recentDateRepository;

        public RecentDateService(IRecentDateRepository recentDateRepository)
        {
            _recentDateRepository = recentDateRepository;
        }

        public RecentDateVM GetRecentDate()
        {
            return _recentDateRepository.GetRecentDate();
        }
    }

}



