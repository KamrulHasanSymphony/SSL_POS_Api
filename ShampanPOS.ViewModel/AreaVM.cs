using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ShampanPOS.ViewModel
{

public class AreaVM
{
        public int Id { get; set; }

        [Display(Name = "Code")]
        public string? Code { get; set; }

        [Display(Name = "Name")]
        public string? Name { get; set; }

        [Display(Name = "Bangla Name")]
        public string? BanglaName { get; set; }

        [Display(Name = "Parent")]
        public string? ParentId { get; set; }

        [Display(Name = "Enum Type")]
        public int? EnumTypeId { get; set; }

        [Display(Name = "Country")]
        public int? CountryId { get; set; }

        [Display(Name = "Division")]
        public int? DivisionId { get; set; }

        [Display(Name = "District")]
        public int? DistrictId { get; set; }

        [Display(Name = "Thana")]
        public int? ThanaId { get; set; }

        [Display(Name = "Zip Code")]
        public string? ZipCode { get; set; }

        [Display(Name = "Archived")]
        public bool IsArchive { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Created On")]
        public string? CreatedOn { get; set; }

        [Display(Name = "Last Modified By")]
        public string? LastModifiedBy { get; set; }

        [Display(Name = "Last Modified On")]
        public string? LastModifiedOn { get; set; }

        [Display(Name = "Created From")]
        public string? CreatedFrom { get; set; }

        [Display(Name = "Last Update From")]
        public string? LastUpdateFrom { get; set; }
        public string? Operation { get; set; }

    }


}
