using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.BankConfig
{
    public class BankConfigViewModel
    {
        public string Id { get; set; }
        public int SN { get; set; }
        public string BranchName { get; set; }
        [Required]
        public string BranchCode { get; set; }

    }
}