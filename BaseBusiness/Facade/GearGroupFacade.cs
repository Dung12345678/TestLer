
using System.Collections;
using BMS.Model;
namespace BMS.Facade
{
	
	public class GearGroupFacade : BaseFacade
	{
		protected static GearGroupFacade instance = new GearGroupFacade(new GearGroupModel());
		protected GearGroupFacade(GearGroupModel model) : base(model)
		{
		}
		public static GearGroupFacade Instance
		{
			get { return instance; }
		}
		protected GearGroupFacade():base() 
		{ 
		} 
	
	}
}
	