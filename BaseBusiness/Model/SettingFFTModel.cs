
using System;
namespace BMS.Model
{
	public partial class SettingFFTModel : BaseModel
	{
		public int ID {get; set;}
		
		public int ProductGroupID {get; set;}
		
		public int ProductTypeID {get; set;}
		
		public string RatioCode {get; set;}
		
		public string FFTValue {get; set;}
		
		public string NoiseValue {get; set;}
		
		public string CreatedBy {get; set;}
		
		public DateTime? CreatedDate {get; set;}
		
		public string UpdatedBy {get; set;}
		
		public DateTime? UpdatedDate {get; set;}
		
	}
}
	