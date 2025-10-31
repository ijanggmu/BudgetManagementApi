using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Aviation
{
    public class AviationInsuredPersionViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = ("Please enter name"))]
        public string Name { get; set; }
        [Required(ErrorMessage = ("Please enter sum insured"))]
        public decimal SumInsured { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class EndorseAviationInsuredPersionViewModel : AviationInsuredPersionViewModel
    {
        public bool IsNew { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsExisting { get; set; }
    }
}
