using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace MakeIT.Nop.Plugin.Payments.Ogone.Components
{
	public class RemotePost
	{
		private readonly SortedDictionary<string, string> _inputs = new SortedDictionary<string, string>();

		public RemotePost()
		{
			Method = "Post";
			FormName = "Form1";
		}

		public RemotePost(string formName) : this()
		{
			FormName = formName;
		}

		public RemotePost(string formName, string url) : this()
		{
			FormName = formName;
			Url = url;
		}

		public string Url { get; set; }

		public string Method { get; set; }

		public string FormName { get; set; }

		public SortedDictionary<string, string> SortedPostFields
		{
			get { return _inputs; }
		} 

		public void Add(string name, string value)
		{
			_inputs.Add(name, value);
		}

		public void Post()
		{
			HttpContext.Current.Response.Clear();
			HttpContext.Current.Response.Write("<html><head>");
			HttpContext.Current.Response.Write(
				string.Format("</head><body onload=\"document.{0}.submit()\">", FormName));

			HttpContext.Current.Response.Write(
				string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" >", FormName, Method, Url));
			
			foreach (string key in _inputs.Keys)
			{
				HttpContext.Current.Response.Write(string.Format(
					"<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", key, _inputs[key]));
			}

			HttpContext.Current.Response.Write("</form>");
			HttpContext.Current.Response.Write("</body></html>");
			HttpContext.Current.Response.End();
		}
	}
}
