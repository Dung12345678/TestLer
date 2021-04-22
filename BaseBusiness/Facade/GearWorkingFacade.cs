
using System.Collections;
using BMS.Model;
namespace BMS.Facade
{
	
	public class GearWorkingFacade : BaseFacade
	{
		protected static GearWorkingFacade instance = new GearWorkingFacade(new GearWorkingModel());
		protected GearWorkingFacade(GearWorkingModel model) : base(model)
		{
		}
		public static GearWorkingFacade Instance
		{
			get { return instance; }
		}
		protected GearWorkingFacade():base() 
		{ 
		} 
	
	}
}
	