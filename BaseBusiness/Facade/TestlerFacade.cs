
using System.Collections;
using BMS.Model;
namespace BMS.Facade
{
	
	public class TestlerFacade : BaseFacade
	{
		protected static TestlerFacade instance = new TestlerFacade(new TestlerModel());
		protected TestlerFacade(TestlerModel model) : base(model)
		{
		}
		public static TestlerFacade Instance
		{
			get { return instance; }
		}
		protected TestlerFacade():base() 
		{ 
		} 
	
	}
}
	