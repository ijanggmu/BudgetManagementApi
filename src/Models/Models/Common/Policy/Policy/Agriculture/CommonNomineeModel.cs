using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Agriculture
{
    public class CommonNomineeModel
    {
        public string FullName { get; set; }
        public string Relation { get; set; }
        public string FathersName { get; set; }
        public string MothersName { get; set; }
        [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = ("Must be a valid number of 7 to 15 characters"))]
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string SpouseName { get; set; }
    }
}
