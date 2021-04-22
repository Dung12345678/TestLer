
using System.Collections;
using BMS.Model;
namespace BMS.Facade
{
	
	public class GearGroupWorkingFacade : BaseFacade
	{
		protected static GearGroupWorkingFacade instance = new GearGroupWorkingFacade(new GearGroupWorkingModel());
		protected GearGroupWorkingFacade(GearGroupWorkingModel model) : base(model)
		{
		}
		public static GearGroupWorkingFacade Instance
		{
			get { return instance; }
		}
		protected GearGroupWorkingFacade():base() 
		{ 
		} 
	
	}
}
	