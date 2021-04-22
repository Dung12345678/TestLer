
using System;
using System.Collections;
using BMS.Facade;
using BMS.Model;
namespace BMS.Business
{

	
	public class GearGroupBO : BaseBO
	{
		private GearGroupFacade facade = GearGroupFacade.Instance;
		protected static GearGroupBO instance = new GearGroupBO();

		protected GearGroupBO()
		{
			this.baseFacade = facade;
		}

		public static GearGroupBO Instance
		{
			get { return instance; }
		}
		
	
	}
}
	