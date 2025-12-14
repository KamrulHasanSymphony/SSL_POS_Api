using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ShampanPOS.ViewModel
{



    public class PeramModel
    {
        public string? Id { get; set; }
        public string? BranchId { get; set; }
        public string? UserLogInId { get; set; }
        public string? ItemNo { get; set; }
        public int? CustomerId { get; set; }
        public string? CompanyId { get; set; }
        public string? TransactionType { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? Type { get; set; }
        public string? TableName { get; set; }

        public string? SearchValue { get; set; }
        public string? OrderName { get; set; }
        public string? orderDir { get; set; }
        public int? startRec { get; set; }
        public int? pageSize { get; set; }

    }


}