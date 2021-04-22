
using System;
namespace BMS.Model
{
	public partial class SettingRatioRoundModel : BaseModel
	{
		public int ID {get; set;}
		
		public string RatioCode {get; set;}
		
		public decimal FrequencyNumber {get; set;}
		
		public string RoundValue {get; set;}
		
		public string CreatedBy {get; set;}
		
		public DateTime? CreatedDate {get; set;}
		
		public string UpdatedBy {get; set;}
		
		public DateTime? UpdatedDate {get; set;}
		
	}
}
	