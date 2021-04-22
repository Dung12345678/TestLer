
using System;
namespace BMS.Model
{
	public partial class FormAndFunctionModel : BaseModel
	{
		public int ID {get; set;}
		
		public string Code {get; set;}
		
		public string Name {get; set;}
		
		public string Description {get; set;}
		
		public bool ShiftKey {get; set;}
		
		public bool CtrlKey {get; set;}
		
		public bool AltKey {get; set;}
		
		public string ShortcutKey {get; set;}
		
		public int FormAndFunctionGroupID {get; set;}
		
		public string CreatedBy {get; set;}
		
		public DateTime? CreatedDate {get; set;}
		
		public string UpdatedBy {get; set;}
		
		public DateTime? UpdatedDate {get; set;}
		
		public bool IsHide {get; set;}
		
	}
}
	