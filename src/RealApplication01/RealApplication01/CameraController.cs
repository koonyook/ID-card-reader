using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using FluxJpeg.Core;
using System.IO;

namespace RealApplication01
{
	public class CameraController
	{
		public const int encodeQuality = 70;	//0..100 (quality of encoding bigimage before send to server)

		public EventHandler onCompleted;

		private ComboBox deviceComboBox;
		private Rectangle rectangle1;
		public CameraController(ComboBox _deviceComboBox, Rectangle _rectangle1)
		{
			deviceComboBox = _deviceComboBox;
			rectangle1 = _rectangle1;

			deviceComboBox.SelectionChanged += deviceComboBox_SelectionChanged;
			deviceComboBox.Unloaded += deviceComboBox_Unloaded;
		}

		private BitmapImage bi = new BitmapImage();
        private CaptureSource _captureSource = new CaptureSource();
		private VideoBrush vidBrush = new VideoBrush();
        
		public void load()
        {
			//set camera
			deviceComboBox.ItemsSource = CaptureDeviceConfiguration.GetAvailableVideoCaptureDevices();
			if (deviceComboBox.Items.Count > 0)
			{
				bool haveDefault = false;
				//
				_captureSource.CaptureImageCompleted += captureSource_CaptureImageComplete;

				if (CaptureDeviceConfiguration.AllowedDeviceAccess || CaptureDeviceConfiguration.RequestDeviceAccess())
				{
					foreach (VideoCaptureDevice device in deviceComboBox.Items)
					{
						if (device.IsDefaultDevice)
						{
							deviceComboBox.SelectedItem = device;
							haveDefault = true;
							break;
						}
					}
					if(!haveDefault)
						deviceComboBox.SelectedIndex = 0;
				}
			}
			
        }

        private void deviceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			if (deviceComboBox.SelectedIndex > -1)
			{
				if (_captureSource != null)
				{
					_captureSource.Stop(); // stop whatever device may be capturing
					// set the devices for the capture source
					_captureSource.VideoCaptureDevice = (VideoCaptureDevice)deviceComboBox.SelectedItem;
					if (_captureSource.VideoCaptureDevice.FriendlyName == "Logitech HD Pro Webcam C910")
					{
						_captureSource.VideoCaptureDevice.DesiredFormat = new VideoFormat(PixelFormatType.Unknown, 2592, 1944, 5);
					}
					// create the brush
					//VideoBrush vidBrush = new VideoBrush();
					vidBrush.SetSource(_captureSource);
					rectangle1.Fill = vidBrush; // paint the brush on the rectangle
					//captureStatus.Text = _captureSource.State.ToString();
					// request user permission and display the capture
					if (CaptureDeviceConfiguration.AllowedDeviceAccess || CaptureDeviceConfiguration.RequestDeviceAccess())
					{
						_captureSource.Start();
						//captureStatus.Text = ((VideoCaptureDevice)deviceComboBox.SelectedItem).FriendlyName;
					}
					else
					{
						deviceComboBox.SelectedIndex = -1;
					}
				}
				else
				{
					//captureStatus.Text = "_captureSource==null";
				}
			}
        }

        public void capture()
        {
            if (_captureSource != null)
            {
                // capture the current frame and add it to our observable collection 
               myvar.busyBinding.myBusy = true;
                _captureSource.CaptureImageAsync();
            }
        }

        private void captureSource_CaptureImageComplete (object s, CaptureImageCompletedEventArgs args)
        {
            //image1.Source = args.Result;
            WriteableBitmap wb = new WriteableBitmap(args.Result);

			////adding : 15/5/54 : free webcam for get more resource	//comment out for easy debugging
			//_captureSource.Stop();	
			///end of adding : 15/5/54

            byte[] binaryData;
            convertToBytesArray(wb, out binaryData);
            //textBox1.Text = (binaryData.Length.ToString() + " ok 55");

            myvar.serializedImage = Convert.ToBase64String(binaryData);
			//string data = (new System.Text.UTF8Encoding()).GetString(binaryData, 0, binaryData.Length);	//do not use this line

            WebClient oWebClient = new WebClient();
			
			//string hostname = Application.Current.Host.Source.AbsoluteUri.Replace(Application.Current.Host.Source.AbsolutePath,"");
			//(myvar.currentPage as Home).textBox1.Text = hostname;

			oWebClient.UploadStringCompleted += new UploadStringCompletedEventHandler(oWebClient_UploadStringCompleted);
			//MessageBox.Show(binaryData.Length.ToString() + "," + data.Length.ToString());
			oWebClient.UploadStringAsync(new Uri(Util.getRootPath() + "/ImageUploadHandler.ashx"), myvar.serializedImage);
			//oWebClient.UploadStringAsync(new Uri(Util.getRootPath() + "/IgeUadHler.ashx"), "");
		}

        private void oWebClient_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
			if (e.Error == null && e.Cancelled == false)
			{
				if (e.Result != null)
				{
					onCompleted(e.Result, null);
					myvar.busyBinding.myBusy = false;
				}
				else
				{
					MessageBox.Show("e.Result is null");	//this line cannot be reached.
				}
			}
			else //case of error
			{
				MessageBox.Show(e.Cancelled+"\n"+e.Error.Message+"\n"+e.Error.InnerException.Message);
				//send it again
				WebClient oWebClient = new WebClient();
				oWebClient.UploadStringCompleted += new UploadStringCompletedEventHandler(oWebClient_UploadStringCompleted);
				oWebClient.UploadStringAsync(new Uri(Util.getRootPath() + "/ImageUploadHandler.ashx"), myvar.serializedImage);
			}
			/*
			try
			{
				myvar.busyBinding.myBusy = false;
				if (e.Result != null)
					onCompleted(e.Result, null);
			}
			catch (Exception error)
			{
				
			}
			*/
        }

       	private void deviceComboBox_Unloaded(object sender, RoutedEventArgs e)
		{
			if (_captureSource != null)
				_captureSource.Stop();		//very important
		}

        private void convertToBytesArray(WriteableBitmap bitmap, out byte[] binaryData)
        {
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            int bands = 3;
            byte[][,] raster = new byte[bands][,];

            //Convert the Image to pass into FJCore
            //Code From http://stackoverflow.com/questions/1139200/using-fjcore-to-encode-silverlight-writeablebitmap
            for (int i = 0; i < bands; i++)
            {
                raster[i] = new byte[width, height];
            }

            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    int pixel = bitmap.Pixels[width * row + column];
                    raster[0][column, row] = (byte)(pixel >> 16);
                    raster[1][column, row] = (byte)(pixel >> 8);
                    raster[2][column, row] = (byte)pixel;
                }
            }

            ColorModel model = new ColorModel { colorspace = ColorSpace.RGB };
            FluxJpeg.Core.Image img = new FluxJpeg.Core.Image(model, raster);

            //textBox1.Text = "before encode";
            //Encode the Image as a JPEG
            MemoryStream stream = new MemoryStream();
			//config the second parameter for balancing the image quality and transfering time
            FluxJpeg.Core.Encoder.JpegEncoder encoder = new FluxJpeg.Core.Encoder.JpegEncoder(img, encodeQuality, stream);
            encoder.Encode();
            
			/* change to only one line (20/5/54) no testing yet
            //Back to the start	
            stream.Seek(0, SeekOrigin.Begin);

            //Get teh Bytes and write them to the stream
            binaryData = new Byte[stream.Length];
            long bytesRead = stream.Read(binaryData, 0, (int)stream.Length);
            */
			binaryData = stream.ToArray();
			stream.Close();
        }

	}
}
