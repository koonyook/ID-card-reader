using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Threading;
using tessnet2;


namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("test tesseract");
            Bitmap image = new Bitmap("5mpinbox.jpg");
            //Console.ReadLine();

            tessnet2.Tesseract ocr = new tessnet2.Tesseract();
            //Console.ReadLine();

            ocr.SetVariable("tessedit_char_whitelist", "0123456789"); // If digit only
            //Console.ReadLine();
            //Console.ReadLine();
            ocr.Init(null,"eng", true); // To use correct tessdata
            //Console.ReadLine();

            List<tessnet2.Word> result = ocr.DoOCR(image, Rectangle.Empty);
            //Console.ReadLine();

            foreach (tessnet2.Word word in result)
                Console.WriteLine("{0} : {1}", word.Confidence, word.Text); 

            Console.ReadLine();
        }
    }
}
