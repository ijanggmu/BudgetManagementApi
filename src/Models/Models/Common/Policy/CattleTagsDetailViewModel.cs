using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy
{
    public class CattleTagsDetailViewModel
    {
        public List<ParseCattleTagVM> parseCattleTagVMs { get; set; }
    }

    public class ParseCattleTagVM
    {
        public string PartyId { get; set; }
        public string TagName { get; set; }
        public string TagId { get; set; }
        public string PolicyNumber { get; set; }
        public string PartyName { get; set; }
        public string Index { get; set; }
        public string Status { get; set; }
        public string IsSelected { get; set; }
    }

    public class UpdateCattleTagModel
    {
        public string PolicyNumber { get; set; }
        public string TagName { get; set; }
    }
}
