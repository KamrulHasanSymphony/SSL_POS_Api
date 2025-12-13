using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Linq;

namespace ShampanPOS.ViewModel
{
    public class Audit
    {
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedFrom { get; set; }

        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? LastUpdateFrom { get; set; }

        public string? PostedBy { get; set; }
        public DateTime? PostedOn { get; set; }
        public string? PostedFrom { get; set; }

        public string? PushedBy { get; set; }
        public DateTime? PushedOn { get; set; }
        public string? PushedFrom { get; set; }       


    }

    public abstract class Entity
	{
		public string? CreatedOn { get; set; }
		public string? CreatedFrom { get; set; }

        public string? LastModifiedOn { get; set; }
        public string? LastUpdateFrom { get; set; }

		public string? PostedOn { get; set; }
		public string? PostedFrom { get; set; }

		public string? PushedOn { get; set; }
		public string? PushedFrom { get; set; }

		public string? IP { get; set; }
		public string? MacAddress { get; set; }

		public string? Value { get; set; } 

	}

}
