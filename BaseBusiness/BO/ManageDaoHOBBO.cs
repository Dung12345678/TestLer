
using System;
using System.Collections;
using BMS.Facade;
using BMS.Model;
namespace BMS.Business
{

	
	public class ManageDaoHOBBO : BaseBO
	{
		private ManageDaoHOBFacade facade = ManageDaoHOBFacade.Instance;
		protected static ManageDaoHOBBO instance = new ManageDaoHOBBO();

		protected ManageDaoHOBBO()
		{
			this.baseFacade = facade;
		}

		public static ManageDaoHOBBO Instance
		{
			get { return instance; }
		}
		
	
	}
}
	