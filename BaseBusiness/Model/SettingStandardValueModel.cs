
using System;
namespace BMS.Model
{
	public partial class SettingStandardValueModel : BaseModel
	{
		public int ID {get; set;}
		
		public int ProductGroupID {get; set;}
		
		public string ProductGroupCode {get; set;}
		
		public int WorkingID {get; set;}
		
		public int ProductID {get; set;}
		
		public int ProductCode {get; set;}
		
		public int QtyTruc {get; set;}
		
		public string Ratio {get; set;}
		
		public string Value {get; set;}
		
		public int Type {get; set;}
		
		public decimal MinValue {get; set;}
		
		public decimal MaxValue {get; set;}
		
	}
}
	