using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace RealApplication01.Web
{
	/// <summary>
	/// Summary description for DeleteTempImageHandler
	/// </summary>
	public class DeleteTempImageHandler : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
			TimeSpan tempImageLifeTime = new TimeSpan(1,0,0,0);	//1 day

			context.Response.ContentType = "text/plain";
			context.Response.Write("List of deleted files.\n\n");
			DirectoryInfo directory = new DirectoryInfo(context.Server.MapPath("~/images/"));
			FileInfo[] fileList = directory.GetFiles("temp*.jpg");
			foreach (FileInfo file in fileList)
			{
				if(DateTime.Now - file.LastWriteTime > tempImageLifeTime)
				{
					context.Response.Write(context.Server.MapPath("~/images/")+file.Name+"\n");
					System.IO.File.Delete(context.Server.MapPath("~/images/") + file.Name);
				}
			}
			fileList = directory.GetFiles("bigtemp*.jpg");

			foreach (FileInfo file in fileList)
			{
				if(DateTime.Now - file.LastWriteTime > tempImageLifeTime)
				{
					context.Response.Write(context.Server.MapPath("~/images/")+file.Name + "\n");
					System.IO.File.Delete(context.Server.MapPath("~/images/") + file.Name);
				}
			}
			context.Response.End();
		}
		

		public bool IsReusable
		{
			get
			{
				return true;	//Koon change this line to true (21/5/54) not test with real server yet)
			}
		}
	}
}