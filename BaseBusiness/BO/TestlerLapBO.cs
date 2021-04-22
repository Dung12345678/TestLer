
using System;
using System.Collections;
using BMS.Facade;
using BMS.Model;
namespace BMS.Business
{

	
	public class TestlerLapBO : BaseBO
	{
		private TestlerLapFacade facade = TestlerLapFacade.Instance;
		protected static TestlerLapBO instance = new TestlerLapBO();

		protected TestlerLapBO()
		{
			this.baseFacade = facade;
		}

		public static TestlerLapBO Instance
		{
			get { return instance; }
		}
		
	
	}
}
	