using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ShampanPOS.ViewModel
{

    public class SalePersonMonthlyAchievementVM
    {
        public int Id { get; set; }

        [Display(Name = "Branch")]
        public int? BranchId { get; set; }

        [Display(Name = "Salesperson")]
        public int? SalePersonId { get; set; }
        public string? SalePersonName { get; set; }

        [Display(Name = "Monthly Sales")]
        [DataType(DataType.Currency)]
        public decimal? MonthlySales { get; set; }

        [Display(Name = "Monthly Target")]
        [DataType(DataType.Currency)]
        public decimal? MonthlyTarget { get; set; }

        [Display(Name = "Self Sale Commission Rate")]
        public decimal? SelfSaleCommissionRate { get; set; }

        [Display(Name = "Other Sale Commission Rate")]
        public decimal? OtherSaleCommissionRate { get; set; }

        [Display(Name = "Year")]
        public int? Year { get; set; }

        [Display(Name = "Month ID")]
        public int? MonthId { get; set; }

        [Display(Name = "Month Start")]
        public string? MonthStart { get; set; }

        [Display(Name = "Month End")]
        public string? MonthEnd { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Created On")]
        [DataType(DataType.DateTime)]
        public string? CreatedOn { get; set; }

        [Display(Name = "Created From")]
        public string? CreatedFrom { get; set; }

        [Display(Name = "Last Modified By")]
        public string? LastModifiedBy { get; set; }

        [Display(Name = "Last Modified On")]
        [DataType(DataType.DateTime)]
        public string? LastModifiedOn { get; set; }

        [Display(Name = "Last Update From")]
        public string? LastUpdateFrom { get; set; }
        public decimal? BonusAmount { get; set; }
        public decimal? TotalBonus { get; set; }
        public decimal? OtherCommissionBonus { get; set; }

        public string? Operation { get; set; }

        public string? Status { get; set; }

       // public string?[] IDs { get; set; }
    }

}
