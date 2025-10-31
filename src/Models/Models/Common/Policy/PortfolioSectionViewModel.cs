using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy
{
    public class PortfolioSectionViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Portfolio Alias must be selected")]
        public string PortfolioAlias { get; set; }
        [Required(ErrorMessage = "Type is required")]
        [RegularExpression("[A-Z0-9]?", ErrorMessage = ("Must be a character in uppercase"))]

        public string Type { get; set; }
        public string Description { get; set; }
        public string CreatedDate { get; set; }
        public int SN { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
    }
}
