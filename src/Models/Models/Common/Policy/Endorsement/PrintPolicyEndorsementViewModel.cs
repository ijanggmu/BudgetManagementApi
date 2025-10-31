using System;

namespace Models.Common.Policy.Endorsement
{
	public class PrintPolicyEndorsementViewModel
	{
		public int Index { get; set; }
		public string PolicyNumber { get; set; }
		public string DocumentNumber { get; set; }
		public string EndorsementDate { get; set; }
		public decimal Gross { get; set; }
		public decimal Stamp { get; set; }
		public decimal Vat { get; set; }
		public decimal Net { get; set; }
		public string EndorsementPrintMessage { get; set; }
	}
}