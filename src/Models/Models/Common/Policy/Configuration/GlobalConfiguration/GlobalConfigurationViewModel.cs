using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Configuration.GlobalConfiguration
{
    public class GlobalConfigurationViewModel
    {
        public int SN { get; set; }
        public string Id { get; set; }
        [Required]
        public string Type { get; set; }
        public int? TypeEnumValue { get; set; }
        [Required]
        public string DataType { get; set; }
        [Required]
        public string ValueType { get; set; }
        [Required]
        [RegularExpression("[0-9,]{0,23}[.]?([0-9]{1,4})?", ErrorMessage = ("Must be a number with 4 decimal points and maximum length of 25"))]
        public decimal Value { get; set; }
        public string Level { get; set; }
        public decimal LowerLimit { get; set; }
        public decimal UpperLimit { get; set; }
        public bool LowerLimitEquals { get; set; }
        public bool UpperLimitEquals { get; set; }
        public bool IsConfigured { get; set; }
        public string CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string TypeName { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
