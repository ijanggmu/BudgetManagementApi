using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.Agriculture
{
    public class CattleFileModel
    {
        public IFormFile CattleMaterialsFile { get; set; }
        public string PolicyNumber { get; set; }
        public bool IsOldEndorsement { get; set; }
        public bool IsInternal { get; set; }
        public string PartyName { get; set; }
        public string PartyId { get; set; }
    }
}
