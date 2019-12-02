using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Atalasoft.Imaging;
using Atalasoft.Imaging.ImageProcessing.Transforms;
using Atalasoft.Imaging.ImageProcessing;
using System.Drawing;

namespace RealApplication01.Web
{
	public class Const
	{
		public const int blockSize = 20; // (width and height in pixel that equal to 5 mm in real world ;  just for sampling in first step)
		public const int threshold = 100; // < threshold maybe background (black) (range 0-255)

		public const int extendedEdeg = 50; //unit of pixel
		public const int samplingRate = 30; //sample edge of a card every XX px

		public const int transformWidth = 860;
		public const int transformHeight = 540;
	}

	public class BigImage
	{
		public HttpContext context;
		public AtalaImage image;
		public List<Segment> segmentList;
		Area[,] table;

		public BigImage(string filename, HttpContext _context)
		{
			this.context = _context;
			image = new AtalaImage(context.Server.MapPath("~/images/") + filename);
			segmentList = new List<Segment>();
		}

		private byte getPixel(int x,int y)
		{
			if (x >= 0 && x < image.Width && y >= 0 && y < image.Height)
			{
				Color c = image.GetPixelColor(x, y);
				return (byte)((c.R + c.G + c.B) / 3);
			}
			else
			{
				return 0;
			}
		}

		private int tableWidth;
		private int tableHeight;
		private bool inRange(int x, int y)
		{
			if (x >= 0 && x < tableWidth && y >= 0 && y < tableHeight)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private Segment fillColor(int beginX, int beginY, int color, bool floodInExpandedArea)
		{
			Segment newSegment = new Segment(this);		//this constructor set size.blockCount = 0
			newSegment.color = color;

			newSegment.minX = beginX;
			newSegment.maxX = beginX;
			newSegment.minY = beginY;
			newSegment.maxY = beginY;

			List<Point> queue = new List<Point>();
			queue.Add(new Point(beginX, beginY));
			table[beginX, beginY].color = color;
			while (queue.Count > 0)
			{
				Point current = queue[0];
				
				if (current.X < newSegment.minX)
					newSegment.minX = current.X;
				else if (current.X > newSegment.maxX)
					newSegment.maxX = current.X;

				if (current.Y < newSegment.minY)
					newSegment.minY = current.Y;
				else if (current.Y > newSegment.maxY)
					newSegment.maxY = current.Y;

				newSegment.blockCount++;
				//flood in eight direction
				for (int i = -1; i <= 1; i++)
				{
					for (int j = -1; j <= 1; j++)
					{
						if (inRange(current.X + i, current.Y + j)
							&& table[current.X + i, current.Y + j].expanded == floodInExpandedArea
							&& table[current.X + i, current.Y + j].color != color)
						{
							table[current.X+i, current.Y+j].color = color;
							queue.Add(new Point(current.X + i, current.Y + j));
						}
					}
				}
				queue.Remove(current);
			}
			newSegment.width = newSegment.maxX - newSegment.minX + 1;
			newSegment.height = newSegment.maxY - newSegment.minY + 1;
			return newSegment;
		}

		public bool passSizeScreening(Segment size)
		{
			/*
			if (size.width < 8)
				return false;
			if (size.height < 8)
				return false;
			if (size.width > 30)
				return false;
			if (size.height > 30)
				return false;
			*/
			if (size.blockCount < 150)
				return false;
			//if (size.blockCount > 300)
			//	return false;
			
			return true;
		}

		/// <summary>
		/// main must call this method
		/// </summary>
		public void seperate()
		{
			tableWidth = image.Width / Const.blockSize + ((image.Width%Const.blockSize == 0) ? 0 : 1);
			tableHeight = image.Height / Const.blockSize + ((image.Height % Const.blockSize == 0) ? 0 : 1);
			table = new Area[tableWidth, tableHeight];
			//running
			//i-> x
			//j-> y

			//new object and sampling value from image
			for (int i = 0; i < tableWidth; i++)
			{
				for (int j = 0; j < tableHeight; j++)
				{
					table[i,j] = new Area(this);
					//select the brightest color of four points.
					table[i, j].samplingValue = Math.Max(
						Math.Max(
						getPixel(i*Const.blockSize, j*Const.blockSize),
						getPixel(i*Const.blockSize,j*Const.blockSize+Const.blockSize/2)),
						Math.Max(
						getPixel(i*Const.blockSize+Const.blockSize/2, j*Const.blockSize),
						getPixel(i*Const.blockSize+Const.blockSize/2,j*Const.blockSize+Const.blockSize/2)));
				}
			}
			
			//expand area of card (>=Const.threshold)
			for (int i = 0; i < tableWidth; i++)
			{
				for (int j = 0; j < tableHeight; j++)
				{
					if (table[i, j].samplingValue >= Const.threshold)
					{
						//loop around this area for expand (9 blocks)
						for (int k = -1; k <= 1; k++)
						{
							for (int l = -1; l <= 1; l++)
							{
								if (inRange(i + k, j + l))
								{
									table[i + k, j + l].expanded = true;
								}
							}
						}
					}
				}
			}

			//To fill color in the hole of card, i must fill the background with a color (-2)
			//find the first area that will be the start flood point of background
			bool founded = false;
			for (int i = 0; i < tableWidth; i++)
			{
				for (int j = 0; j < tableHeight; j++)
				{
					if (table[i, j].expanded == false)
					{
						fillColor(i, j, -2, false);
						founded = true;
						break;
					}
				}
				if (founded)
					break;
			}
			//Then, find the hole(color = -1) and fill it(change expanded to true)
			for (int i = 0; i < tableWidth; i++)
			{
				for (int j = 0; j < tableHeight; j++)
				{
					if (table[i, j].color == -1)			//this is a hole
					{
						table[i, j].expanded = true;
					}
				}
			}

			//fill color in each seperated area with breadth first(Because recursive method maybe raise stack overflow problem in Very big image)
			List<Segment> passSegments = new List<Segment>();		//for keep the color that pass the size screening.
			int currentColor = 0;
			for (int i = 0; i < tableWidth; i++)
			{
				for (int j = 0; j < tableHeight; j++)
				{
					if (table[i, j].expanded && table[i,j].color==-1)
					{
						Segment newSegment = fillColor(i, j, currentColor,true);
						if (passSizeScreening(newSegment))
						{
							passSegments.Add(newSegment);
						}
						currentColor++;	//change color
					}
				}
			}

			//crop each segment from a the big image
			int tmpName = 1;
			foreach (Segment seg in passSegments)
			{
				Rectangle cropArea = new Rectangle(seg.minX*Const.blockSize
					,seg.minY*Const.blockSize
					,seg.width*Const.blockSize				//this not provide extended area if width of big image % blocksize !=0 (not error,I have already test it)
					,seg.height*Const.blockSize
					);
				CropCommand crop = new CropCommand(cropArea);
				seg.image = crop.Apply(this.image).Image;

				//overlay (patch) the other color that is not background (that may be noise)
				AtalaImage blackBlock = new AtalaImage(Const.blockSize, Const.blockSize, PixelFormat.Pixel32bppBgra, Color.Black);
				for (int i = seg.minX; i <= seg.maxX; i++)
				{
					for (int j = seg.minY; j <= seg.maxY; j++)
					{
						if (table[i, j].color != seg.color)
						{
							OverlayCommand patcher = new OverlayCommand(blackBlock, new Point((i-seg.minX)*Const.blockSize, (j-seg.minY)*Const.blockSize));
							seg.image = patcher.Apply(seg.image).Image;
						}
					}
				}
				//extend the image with black edge (prevent the error when use complex transform later)
				OverlayCommand overlay = new OverlayCommand(seg.image, new Point(Const.extendedEdeg, Const.extendedEdeg));
				AtalaImage backGround = new AtalaImage(seg.image.Width + 2 * Const.extendedEdeg, seg.image.Height + 2 * Const.extendedEdeg, PixelFormat.Pixel32bppBgra, Color.Black);
				seg.image = overlay.Apply(backGround).Image;
				
				seg.transform();
				//debug zone : show the segmented area by save to the image
				//seg.transformedImage.Save(context.Server.MapPath("~/images/") +tmpName.ToString()+ "-transformed.jpg", new Atalasoft.Imaging.Codec.JpegEncoder(), null);
				tmpName++;
				//end debug zone
			}
			this.segmentList = passSegments;	
		}
	}

	public class Area		//size of blockSize X blockSize
	{
		public int samplingValue;
		public bool expanded = false;
		public int color = -1;		//true color is started from zero

		public BigImage parent;

		public Area(BigImage _parent)
		{
			parent = _parent;
		}

		public override string ToString()
		{
			return string.Format("({0},{1},{2})", color, expanded,samplingValue);
		}
	}

	public class Segment
	{
		public BigImage parent;
		public Segment(BigImage _parent)
		{
			parent = _parent;
		}

		// zone 1 variables for manage with block table
		public int color;

		public int minX;
		public int maxX;
		public int minY;
		public int maxY;

		public int blockCount=0;
		public int width=0;
		public int height=0;

		// zone 2 the real image
		public AtalaImage image;
		public AtalaImage transformedImage;

		public void transform()
		{
			//keep the series of edge in 4 Lists

			//project from the left side (top to bottom)
			List<Point> leftList = new List<Point>();
			for (int j = Const.extendedEdeg; j < image.Height - Const.extendedEdeg; j += Const.samplingRate)
			{
				for (int i = Const.extendedEdeg; i < image.Width - Const.extendedEdeg; i++)
				{
					Color x = image.GetPixelColor(i, j);
					int avg = (x.R + x.G + x.B) / 3;
					if (avg >= Const.threshold)		//the first point is edge
					{
						leftList.Add(new Point(i, j));
						break;
					}
				}
			}

			//project from the right side (top to bottom)
			List<Point> rightList = new List<Point>();
			for (int j = Const.extendedEdeg; j < image.Height - Const.extendedEdeg; j += Const.samplingRate)
			{
				for (int i = image.Width - Const.extendedEdeg; i > Const.extendedEdeg; i--)
				{
					Color x = image.GetPixelColor(i, j);
					int avg = (x.R + x.G + x.B) / 3;
					if (avg >= Const.threshold)		//the first point is edge
					{
						rightList.Add(new Point(i, j));
						break;
					}
				}
			}

			//project from the top side (left to right)
			List<Point> topList = new List<Point>();
			for (int i = Const.extendedEdeg; i < image.Width - Const.extendedEdeg; i += Const.samplingRate)
			{
				for (int j = Const.extendedEdeg; j < image.Height - Const.extendedEdeg; j++)
				{
					Color x = image.GetPixelColor(i, j);
					int avg = (x.R + x.G + x.B) / 3;
					if (avg >= Const.threshold)		//the first point is edge
					{
						topList.Add(new Point(i, j));
						break;
					}
				}
			}

			//project from the bottom side (left to right)
			List<Point> bottomList = new List<Point>();
			for (int i = Const.extendedEdeg; i < image.Width - Const.extendedEdeg; i += Const.samplingRate)
			{
				for (int j = image.Height - Const.extendedEdeg; j > Const.extendedEdeg; j--)
				{
					Color x = image.GetPixelColor(i, j);
					int avg = (x.R + x.G + x.B) / 3;
					if (avg >= Const.threshold)		//the first point is edge
					{
						bottomList.Add(new Point(i, j));
						break;
					}
				}
			}
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			////////finish keep the value from projection
			List<Dot> dotList;

			//Analyse the linear equation(y = mx + c) of each side
			//left side
			dotList = new List<Dot>();			
			for (int r = 0; r < leftList.Count - 1; r++)
			{
				//screen and keep the point on the left-half-side of the image only
				if (leftList[r].X > image.Width / 2)
					continue;
				float slope = (float)(leftList[r + 1].X - leftList[r].X) / (leftList[r + 1].Y - leftList[r].Y);
				if (Math.Abs(slope) < 1.0)		//( |slope| < 45 degree ) give only one line in theoretic situation
					dotList.Add(new Dot(leftList[r], slope));
			}
			StraightLine leftLine = getStraightLine(dotList);

			//right side
			dotList = new List<Dot>();
			for (int r = 0; r < rightList.Count - 1; r++)
			{
				//screen and keep the point on the right-half-side of the image only
				if (rightList[r].X < image.Width / 2)
					continue;
				float slope = (float)(rightList[r + 1].X - rightList[r].X) / (rightList[r + 1].Y - rightList[r].Y);
				if (Math.Abs(slope) < 1.0)		//( |slope| < 45 degree ) give only one line in theoretic situation
					dotList.Add(new Dot(rightList[r], slope));
			}
			StraightLine rightLine = getStraightLine(dotList);

			//top side
			dotList = new List<Dot>();
			for (int r = 0; r < topList.Count - 1; r++)
			{
				//screen and keep the point on the top-half-side of the image only
				if (topList[r].Y > image.Height / 2)
					continue;
				float slope = (float)(topList[r + 1].Y - topList[r].Y) / (topList[r + 1].X - topList[r].X);
				if (Math.Abs(slope) < 1.0)		//( |slope| < 45 degree ) give only one line in theoretic situation
					dotList.Add(new Dot(topList[r], slope));
			}
			StraightLine topLine = getStraightLine(dotList);

			//bottom side
			dotList = new List<Dot>();
			for (int r = 0; r < bottomList.Count - 1; r++)
			{
				//screen and keep the point on the bottom-half-side of the image only
				if (bottomList[r].Y < image.Height / 2)
					continue;
				float slope = (float)(bottomList[r + 1].Y - bottomList[r].Y) / (bottomList[r + 1].X - bottomList[r].X);
				if (Math.Abs(slope) < 1.0)		//( |slope| < 45 degree ) give only one line in theoretic situation
					dotList.Add(new Dot(bottomList[r], slope));
			}
			StraightLine bottomLine = getStraightLine(dotList);

			//conclude four point of interception
			Point cornerTopLeft = StraightLine.intercept(leftLine, topLine);
			Point cornerTopRight = StraightLine.intercept(rightLine, topLine);
			Point cornerBottomLeft = StraightLine.intercept(leftLine, bottomLine);
			Point cornerBottomRight = StraightLine.intercept(rightLine, bottomLine);
			//fix the data if cannot find some line (corner point will be (-1,-1))

			if (cornerTopLeft.X < 0 || cornerTopLeft.Y < 0 || cornerTopLeft.X >= image.Width || cornerTopLeft.Y >= image.Height)
			{
				cornerTopLeft = new Point(Const.extendedEdeg + Const.blockSize, Const.extendedEdeg + Const.blockSize);
			}
			if (cornerTopRight.X < 0 || cornerTopRight.Y < 0 || cornerTopRight.X >= image.Width || cornerTopRight.Y >= image.Height)
			{
				cornerTopRight = new Point(image.Width - Const.extendedEdeg - Const.blockSize, Const.extendedEdeg + Const.blockSize);
			}
			if (cornerBottomLeft.X < 0 || cornerBottomLeft.Y < 0 || cornerBottomLeft.X >= image.Width || cornerBottomLeft.Y >= image.Height)
			{
				cornerBottomLeft = new Point(Const.extendedEdeg + Const.blockSize, image.Height - Const.extendedEdeg - Const.blockSize);
			}
			if (cornerBottomRight.X < 0 || cornerBottomRight.Y < 0 || cornerBottomRight.X >= image.Width || cornerBottomRight.Y >= image.Height)
			{
				cornerBottomRight = new Point(image.Width - Const.extendedEdeg - Const.blockSize, image.Height - Const.extendedEdeg - Const.blockSize);
			}

			//trans form image to rectangle in the real ratio (8.6 : 5.4 cm) (example 430 x 270)
			Point destTopLeft = new Point(0, 0);
			Point destTopRight = new Point(Const.transformWidth,0);
			Point destBottomLeft = new Point(0, Const.transformHeight);
			Point destBottomRight = new Point(Const.transformWidth, Const.transformHeight);

			QuadrilateralWarpCommand cmd = new QuadrilateralWarpCommand(
				cornerBottomLeft, cornerTopLeft, cornerTopRight, cornerBottomRight,
				destBottomLeft, destTopLeft, destTopRight, destBottomRight,
				InterpolationMode.BiCubic, Color.Black);

			this.transformedImage = cmd.Apply(image).Image;

		}

		private StraightLine getStraightLine(List<Dot> dotList)
		{
			//rare case
			if (dotList.Count < 2)	//cannot do anything
				return null;
			if (dotList.Count == 2 || dotList.Count == 3)
			{
				//pick first and last to make straightline
				return new StraightLine(dotList[0].location, dotList[dotList.Count - 1].location);
			}
			//select sequence of 4 Dot minimum range that give minimum range of slope (round corner cannot win this contest)
			float minRange = 2.1f;
			int minIndex = -1;
			for (int r = 0; r < dotList.Count - 3; r++)
			{
				float maxSlope = Math.Max(Math.Max(dotList[r].slope, dotList[r + 1].slope), Math.Max(dotList[r + 2].slope, dotList[r + 3].slope));
				float minSlope = Math.Min(Math.Min(dotList[r].slope, dotList[r + 1].slope), Math.Min(dotList[r + 2].slope, dotList[r + 3].slope));
				float range = maxSlope - minSlope;
				if (range < minRange)
				{
					minRange = range;
					minIndex = r;
				}
			}
			//pick 2 point from the selected sequence for make an equation   //i choose the first and the last (no reason)
			return new StraightLine(dotList[minIndex].location, dotList[minIndex + 3].location);
		}
	}


	public class Dot
	{
		public Point location;
		public float slope;

		public Dot(Point _location, float _slope)
		{
			location = _location;
			slope = _slope;
		}
	}

	public class StraightLine
	{
		//This is equation ax + by + c = 0
		public float a;
		public float b;
		public float c;

		public StraightLine(Point p1, Point p2)
		{
			a = p1.Y - p2.Y;
			b = p2.X - p1.X;
			c = (p1.X * p2.Y) - (p2.X * p1.Y);

			//formula from http://bobobobo.wordpress.com/2008/01/07/solving-linear-equations-ax-by-c-0/
		}

		public StraightLine(PointF p1, PointF p2)
		{
			a = p1.Y - p2.Y;
			b = p2.X - p1.X;
			c = (p1.X * p2.Y) - (p2.X * p1.Y);
		}

		public static Point intercept(StraightLine s1,StraightLine s2)
		{
			if (s1 == null || s2 == null)
			{
				return new Point(-1,-1);	//it's cannot be like this in the real situation
			}
			//case of a or b is zero
			float ansX, ansY;
			if (s1.a * s1.b * s2.a * s2.b == 0)
			{
				if (s2.a == 0)
				{
					//swap s2 with s1
					StraightLine tmp = s2;
					s2 = s1;
					s1 = tmp;
				}
				if (s1.a == 0)
				{
					ansY = -s1.c / s1.b;
					ansX = -(s2.b * ansY + s2.c) / s2.a;
				}
				else
				{
					if (s2.b == 0)
					{
						//swap s2 with s1
						StraightLine tmp = s2;
						s2 = s1;
						s1 = tmp;
					}
					if (s1.b == 0)
					{
						ansX = -s1.c / s1.a;
						ansY = -(s2.a * ansX + s2.c) / s2.b;
					}
					else
					{
						//if code run to this line -> it's bug
						ansX = -1;
						ansY = -1;
					}
				}
			}
			else
			{
				ansY = (s1.a * s2.c / s2.a - s1.c) / (s1.b - s1.a * s2.b / s2.a);
				ansX = -(s1.b * ansY + s1.c) / s1.a;
			}
			return new Point((int)(Math.Round(ansX)), (int)(Math.Round(ansY)));
		}
	}
}