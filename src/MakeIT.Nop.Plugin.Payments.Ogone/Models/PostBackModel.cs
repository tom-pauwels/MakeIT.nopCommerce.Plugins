using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MakeIT.Nop.Plugin.Payments.Ogone.Models
{
	public class PostBackModel
	{
		public string ACCEPTANCE { get; set; }
		public string AMOUNT { get; set; }
		public string BRAND { get; set; }
		public string CARDNO { get; set; }
		public string CN { get; set; }
		public string CURRENCY { get; set; }
		public string NCERROR { get; set; }
		public string ORDERID { get; set; }
		public string PAYID { get; set; }
		public string PM { get; set; }
		public int STATUS { get; set; }
		public string TRXDATE { get; set; }
		public string SHASIGN { get; set; }
	}
}