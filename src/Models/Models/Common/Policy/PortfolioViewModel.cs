using System.ComponentModel.DataAnnotations;
using SharedKernel.Attributes;

namespace Models.Common.Policy
{
    public class PortfolioViewModel
    {
        public int Id { get; set; }
        public int SN { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string ClassNameEnglish { get; set; }
        public string ClassNameNepali { get; set; }
        [RequiredIf("Type", "1", ErrorMessage = "ParentId is required.")]
        public int? ParentId { get; set; }
        public string ClassAlias { get; set; }
        public string Type { get; set; }
        [RequiredIf("Type", "2", ErrorMessage = "ClassId is required.")]
        public int? ClassId { get; set; }
        public string PortfolioTypeValue { get; set; }
        /// 1 for Class Creation, 2 for sub-class Creation
    }
}

