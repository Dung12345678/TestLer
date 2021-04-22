
using System;
namespace BMS.Model
{
	public partial class UserGroupModel : BaseModel
	{
		public int ID {get; set;}
		
		public string Name {get; set;}
		
		public string Code {get; set;}
		
		public int Leader {get; set;}
		
		public int DepartmentID {get; set;}
		
	}
}
	