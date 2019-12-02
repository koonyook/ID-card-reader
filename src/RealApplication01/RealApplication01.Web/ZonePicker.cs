using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;

namespace RealApplication01.Web
{
	public class ZonePicker
	{
		public static Bitmap pickImage(Bitmap image,int type)	//type=(1:normal,2:no crut)
		{
			//in the 860 x 540 image
			//i will crop at (377,64) with 360x38-size-rectangle
			//all of this will be implement in ratio for flexible input support
			if (type == 1)
			{
				return image.Clone(new RectangleF(
					image.Width * 377 / 860f,
					image.Height * 64 / 540f,
					image.Width * 360 / 860f,
					image.Height * 38 / 540f), image.PixelFormat);
			}
			else if (type == 2)
			{
				return image.Clone(new RectangleF(
					image.Width * 315 / 860f,
					image.Height * 64 / 540f,
					image.Width * 302 / 860f,
					image.Height * 38 / 540f), image.PixelFormat);
			}
			else
				return null;
		}

	}
}