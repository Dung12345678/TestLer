
using System;
using System.Collections;
using BMS.Facade;
using BMS.Model;
namespace BMS.Business
{

	
	public class GearGroupWorkingBO : BaseBO
	{
		private GearGroupWorkingFacade facade = GearGroupWorkingFacade.Instance;
		protected static GearGroupWorkingBO instance = new GearGroupWorkingBO();

		protected GearGroupWorkingBO()
		{
			this.baseFacade = facade;
		}

		public static GearGroupWorkingBO Instance
		{
			get { return instance; }
		}
		
	
	}
}
	