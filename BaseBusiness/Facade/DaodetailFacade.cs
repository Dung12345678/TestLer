
using System.Collections;
using BMS.Model;
namespace BMS.Facade
{
	
	public class DaodetailFacade : BaseFacade
	{
		protected static DaodetailFacade instance = new DaodetailFacade(new DaodetailModel());
		protected DaodetailFacade(DaodetailModel model) : base(model)
		{
		}
		public static DaodetailFacade Instance
		{
			get { return instance; }
		}
		protected DaodetailFacade():base() 
		{ 
		} 
	
	}
}
	