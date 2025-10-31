using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.UnderwritingAPI.HeaderParameter
{
    public class HeaderParameterModel
    {
        [FromHeader]
        public int pageLimit { get; set; }
        [FromHeader]
        public string apiKey { get; set; }
    }
}
