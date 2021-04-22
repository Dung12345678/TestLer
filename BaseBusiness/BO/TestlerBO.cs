
using System;
using System.Collections;
using BMS.Facade;
using BMS.Model;
namespace BMS.Business
{

	
	public class TestlerBO : BaseBO
	{
		private TestlerFacade facade = TestlerFacade.Instance;
		protected static TestlerBO instance = new TestlerBO();

		protected TestlerBO()
		{
			this.baseFacade = facade;
		}

		public static TestlerBO Instance
		{
			get { return instance; }
		}
		
	
	}
}
	