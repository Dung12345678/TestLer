
using System;
namespace BMS.Model
{
	public partial class DaodetailModel : BaseModel
	{
		public int ID {get; set;}
		
		public int ProductID {get; set;}
		
		public int QtyProductMax {get; set;}
		
		public int QtyProduct {get; set;}
		
		public int TotalProduct {get; set;}
		
		public DateTime? CreatedAt {get; set;}
		
	}
}
	