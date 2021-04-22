
using System;
namespace BMS.Model
{
	public partial class UserRightDistributionModel : BaseModel
	{
		public int ID {get; set;}
		
		public int FormAndFunctionID {get; set;}
		
		public int UserID {get; set;}
		
		public bool ViewRight {get; set;}
		
		public bool CreateRight {get; set;}
		
		public bool ModifyRight {get; set;}
		
		public bool DeleteRight {get; set;}
		
		public bool SpecialRight {get; set;}
		
		public int UserInsertID {get; set;}
		
		public DateTime? CreateDate {get; set;}
		
		public int UserUpdateID {get; set;}
		
		public DateTime? UpdateDate {get; set;}
		
	}
}
	