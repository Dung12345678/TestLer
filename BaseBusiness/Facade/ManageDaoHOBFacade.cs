
using System.Collections;
using BMS.Model;
namespace BMS.Facade
{
	
	public class ManageDaoHOBFacade : BaseFacade
	{
		protected static ManageDaoHOBFacade instance = new ManageDaoHOBFacade(new ManageDaoHOBModel());
		protected ManageDaoHOBFacade(ManageDaoHOBModel model) : base(model)
		{
		}
		public static ManageDaoHOBFacade Instance
		{
			get { return instance; }
		}
		protected ManageDaoHOBFacade():base() 
		{ 
		} 
	
	}
}
	