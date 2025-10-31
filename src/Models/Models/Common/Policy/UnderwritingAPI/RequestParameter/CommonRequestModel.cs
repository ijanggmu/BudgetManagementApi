using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.UnderwritingAPI.RequestParameter
{
    public class CommonRequestModel
    {
        public int ItemPerPage { get; set; }
        public int PageNumber { get; set; }
        public string RequestedBy { get; set; }
    }
}
