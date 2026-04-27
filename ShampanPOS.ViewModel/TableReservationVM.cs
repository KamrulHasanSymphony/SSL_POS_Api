using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.ViewModel
{
    public class TableReservationVM
    {

        public int Id { get; set; }

        public int? TableId { get; set; }
        public string? TableNumber { get; set; }
        public string? SectionName { get; set; }
        public string? StatusName { get; set; }


        public string? CustomerName { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTime? ReservationTime { get; set; }

        public string? Status { get; set; }

        public string? CreatedBy { get; set; }

        public string? CreatedOn { get; set; }

        public string? CreatedFrom { get; set; }

        public string? LastModifiedBy { get; set; }

        public string? LastModifiedOn { get; set; }

        public string? LastUpdateFrom { get; set; }

        public int? BranchId { get; set; }
        public string? BranchName { get; set; }


        public int? CompanyId { get; set; }

        [Display(Name = "Operation")]
        public string? Operation { get; set; }

    }
}
