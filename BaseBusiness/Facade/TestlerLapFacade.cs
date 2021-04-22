
using System.Collections;
using BMS.Model;
namespace BMS.Facade
{
	
	public class TestlerLapFacade : BaseFacade
	{
		protected static TestlerLapFacade instance = new TestlerLapFacade(new TestlerLapModel());
		protected TestlerLapFacade(TestlerLapModel model) : base(model)
		{
		}
		public static TestlerLapFacade Instance
		{
			get { return instance; }
		}
		protected TestlerLapFacade():base() 
		{ 
		} 
	
	}
}
	