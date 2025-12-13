using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShampanPOS.Repository
{
    public interface IRecentDateRepository
    {
        RecentDateVM GetRecentDate();
    }
}
namespace ShampanPOS.Repository
{
    public class RecentDateRepository : IRecentDateRepository
    {
        public RecentDateVM GetRecentDate()
        {
            return new RecentDateVM { Date = DateTime.Now };
        }
    }
}
