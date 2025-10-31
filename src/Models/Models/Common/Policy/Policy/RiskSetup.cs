using System;
using System.Text;

namespace Models.Common.Policy.Policy
{
    public class RiskSetup
    {
        public string Type { get; set; }
        public string TypeEnumValue { get; set; }
        public string DataType { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }
        public string LowerLimit { get; set; }
        public string UpperLimit { get; set; }
        public string LowerLimitEquals { get; set; }
        public string UpperLimitEquals { get; set; }
        public string PortfolioAlias { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
