
using System;
namespace BMS.Model
{
	public partial class ProductCheckHistoryModel : BaseModel
	{
		public long ID {get; set;}
		
		public string QRCode {get; set;}
		
		public int ProductID {get; set;}
		
		public int ApprovedID {get; set;}
		
		public int MonitorID {get; set;}
		
		public string Approved {get; set;}
		
		public string Monitor {get; set;}
		
		public DateTime? DateLR {get; set;}
		
		public string EditContent {get; set;}
		
		public DateTime? EditDate {get; set;}
		
		public string CreatedBy {get; set;}
		
		public DateTime? CreatedDate {get; set;}
		
		public string UpdatedBy {get; set;}
		
		public DateTime? UpdatedDate {get; set;}
		
	}
}
	