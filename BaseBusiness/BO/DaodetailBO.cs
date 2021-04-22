
using System;
using System.Collections;
using BMS.Facade;
using BMS.Model;
namespace BMS.Business
{

	
	public class DaodetailBO : BaseBO
	{
		private DaodetailFacade facade = DaodetailFacade.Instance;
		protected static DaodetailBO instance = new DaodetailBO();

		protected DaodetailBO()
		{
			this.baseFacade = facade;
		}

		public static DaodetailBO Instance
		{
			get { return instance; }
		}
		
	
	}
}
	