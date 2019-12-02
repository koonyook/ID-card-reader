using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace RealApplication01.Web
{
	/// <summary>
	/// Summary description for ImageRenameHandler
	/// </summary>
	public class ImageRenameHandler : IHttpHandler
	{

		public void ProcessRequest(HttpContext context)
		{
			string username = context.Request["username"];	//<---from get variable
			//context.Request.InputStream
			XmlReader reader = XmlReader.Create(context.Request.InputStream);

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element && reader.Name == "rename")
				{
					string oldName = reader.GetAttribute("oldName");
					string newName = reader.GetAttribute("newName");
					
					System.IO.File.Move(context.Server.MapPath("~/images/") + oldName, context.Server.MapPath("~/images/") + newName);
				}
			}

			context.Response.ContentType = "text/plain";
			context.Response.Write("ok");
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}