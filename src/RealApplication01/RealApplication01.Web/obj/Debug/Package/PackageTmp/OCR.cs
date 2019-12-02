using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using tessnet2;

namespace RealApplication01.Web
{
	/// <summary>
	/// get digit string from pre-processed image
	/// </summary>
	public class DigitReader	
	{
		public static string read(Bitmap image,HttpContext context)
		{
			//this image must be pre-processed
			tessnet2.Tesseract ocr = new tessnet2.Tesseract();
			ocr.SetVariable("tessedit_char_whitelist", "0123456789"); // If digit only					
			ocr.Init(context.Server.MapPath("~/bin/tessdata/"), "eng", true);		// To use correct tessdata
			List<tessnet2.Word> result = ocr.DoOCR(image, Rectangle.Empty);

			string ans = "";
			foreach (tessnet2.Word word in result)
			{
				//Console.WriteLine("{0} : {1}", word.Confidence, word.Text); 
				ans += word.Text;
			}
			
			return ans;
		}
	}
}