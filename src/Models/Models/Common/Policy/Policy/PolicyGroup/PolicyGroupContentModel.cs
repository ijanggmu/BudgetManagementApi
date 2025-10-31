using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.PolicyGroup
{
    public class PolicyGroupContentModel
    {
        public IFormFile PolicyGroupContentData { get; set; }
        public string PolicyGroupId { get; set; }
    }
}
