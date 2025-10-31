using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.EndorsablePortfolio
{
    public class EndorsablePortfoliosViewModel
    {
        [Required]
        public List<string> Portfolio { get; set; }

        public string UserId { get; set; }
    }
}
