
using System;
namespace BMS.Model
{
	public partial class UserGroupLinkModel : BaseModel
	{
		public int ID {get; set;}
		
		public int UserGroupID {get; set;}
		
		public int UserID {get; set;}
		
	}
}
	