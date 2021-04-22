
using System;
namespace BMS.Model
{
	public partial class TestlerLapModel : BaseModel
	{
		public long ID {get; set;}
		
		public string OrderCode {get; set;}
		
		public string HypCode {get; set;}
		
		public string GearCode {get; set;}
		
		public int Qty {get; set;}
		
		public string WorkerCode {get; set;}
		
		public DateTime? DateLR {get; set;}
		
		public int GearWorkingID {get; set;}
		
		public string GearWorkingName {get; set;}
		
		public decimal MinValue {get; set;}
		
		public decimal MaxValue {get; set;}
		
		public decimal DefaultValue {get; set;}
		
		public int SortOrder {get; set;}
		
		public decimal RealValue {get; set;}
		
		public string Hyp {get; set;}
		
		public string Gear {get; set;}
		
		public int STT {get; set;}
		
		public string TanSuat {get; set;}
		
		public string CreatedBy {get; set;}
		
		public DateTime? CreatedDate {get; set;}
		
		public string UpdatedBy {get; set;}
		
		public DateTime? UpdatedDate {get; set;}
		
		public int QtyLap {get; set;}
		
		public string Result {get; set;}

		public string Batch { get; set; }

		public string Confirmer { get; set; }

		public int SttStart { get; set; }

		public string TesterName { get; set; }

	}
}
	