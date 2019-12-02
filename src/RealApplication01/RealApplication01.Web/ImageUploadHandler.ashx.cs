using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Drawing;

using RealApplication01;

namespace RealApplication01.Web
{
    /// <summary>
    /// Summary description for ImageUploadHandler
    /// </summary>
    public class ImageUploadHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
			//write a log for debugging  
			/*
			using (StreamWriter outfile = new StreamWriter(context.Server.MapPath("~/log/")+"lastLog.txt"))
			{
				outfile.WriteLine(DateTime.Now);
				outfile.WriteLine(context.Request.InputStream.Length.ToString());
				//outfile.WriteLine(context.Request.I
				outfile.Close();
				outfile.Dispose();
			}
			*/
			if (context.Request.InputStream.Length == 0)
			{
				context.Response.ContentType = "text/xml";
				context.Response.Write("blank inputStream");
				return;
			}
			try
			{
				//string username = context.Request["username"];
				string data;
				string bigimagename;
				//read data from client and save as a big image
				using (StreamReader sr = new StreamReader(context.Request.InputStream))
				{
					data = sr.ReadToEnd();
					byte[] array = Convert.FromBase64String(data);
					//byte[] array = (new System.Text.UTF8Encoding()).GetBytes(data);	// do not use this line

					bigimagename = "bigtemp" + DateTime.Now.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '_') + ".jpg";
					using (FileStream fs = new FileStream(context.Server.MapPath("~/images/") + bigimagename, FileMode.Create))
					{
						fs.Write(array, 0, array.Length);
						fs.Close();
					}
				}

				BigImage bigimage = new BigImage(bigimagename, context);
				bigimage.seperate();

				//release bigtemp for deleting
				bigimage.image.Dispose();

				List<Card> crop = new List<Card>();

				for (int i = 0; i < bigimage.segmentList.Count; i++)
				{
					string newname = "temp_" + i.ToString() + "_" + DateTime.Now.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '_') + ".jpg";

					//save transformedImage
					bigimage.segmentList[i].transformedImage.Save(
						context.Server.MapPath("~/images/") + newname, new Atalasoft.Imaging.Codec.JpegEncoder(), null);

					//clear for renaming
					bigimage.segmentList[i].transformedImage.Dispose();

					//cannot use bigimage.segmentList[i].transformedImage.ToBitmap() //this will get black image
					Card newcard = new Card(new Bitmap(context.Server.MapPath("~/images/") + newname), newname);


					// get raw id from image
					//newcard.bitmap.Save(context.Server.MapPath("~/images/") + "bitmap_" + newcard.name);
					Bitmap idZone = ZonePicker.pickImage(newcard.bitmap, 1);
					//idZone.Save(context.Server.MapPath("~/images/") + "num_"+newcard.name);
					newcard.cardID = DigitReader.read(idZone, context);

					// validate and fix the cardID (please uncomment in the real use)
					if (!newcard.validateID())
					{
						if (newcard.cardID.Length < 13)
						{
							idZone = ZonePicker.pickImage(newcard.bitmap, 2);
							newcard.cardID = DigitReader.read(idZone, context);

						}
						//if (!newcard.validateID())	newcard.cardID = "";
					}

					//clear for renaming
					newcard.bitmap.Dispose();

					crop.Add(newcard);
				}

				//send back the filename and id to client with xml
				context.Response.ContentType = "text/xml";
				context.Response.ContentEncoding = System.Text.Encoding.UTF8;
				context.Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<cards>\n");
				foreach (Card card in crop)
				{
					context.Response.Write(String.Format("\t<card ID=\"{0}\" filename=\"{1}\"></card>\n", card.cardID, card.name));
				}
				context.Response.Write("</cards>");
				//context.Response.End();
			}
			catch (Exception e)
			{
				context.Response.ContentType = "text/xml";
				context.Response.Write("Error:\n");
				context.Response.Write(e.Message);
				context.Response.Write(e.ToString());
				context.Response.Write(e.InnerException.ToString());
				context.Response.End();
			}
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }


	public class Card
	{
		public Bitmap bitmap;
		public string name;
		public string cardID;
		public Card(Bitmap _bitmap,string _name)
		{
			bitmap = _bitmap;
			name=_name;
		}

		public bool validateID()
		{
			if (cardID.Length != 13)
				return false;
			if (cardID[0] == '0' || cardID[0] == '9')
				return false;
			try
			{
				int sum = 0;
				for (int i = 0; i <= 11; i++)
				{
					sum+= Int32.Parse(cardID[i].ToString())*(13-i);
				}
				int lastDigit = (11 - (sum % 11)) % 10;
				if (cardID[12].ToString() == lastDigit.ToString())
					return true;
				else
					return false;
			}
			catch
			{
				//parse digit error
				return false;
			}
		}
	}

}