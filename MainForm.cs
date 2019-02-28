// Simple Player sample application
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2006-2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using AForge.Video;
using AForge.Video.DirectShow;
using Uface;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Player
{
    public partial class MainForm : Form
    {
        private Stopwatch stopWatch = null;
        private UImage uimage_1 = new UImage();
        private QualityJudge judge = new QualityJudge(48);   
        private Bitmap tmpBitmap = null;
        // Class constructor
        public MainForm( )
        {
            InitializeComponent( );
            //
        }

        private void MainForm_FormClosing( object sender, FormClosingEventArgs e )
        {
            CloseCurrentVideoSource( );
        }

        // "Exit" menu item clicked
        private void exitToolStripMenuItem_Click( object sender, EventArgs e )
        {
            this.Close( );
        }

        // Open local video capture device
        private void localVideoCaptureDeviceToolStripMenuItem_Click( object sender, EventArgs e )
        {
            VideoCaptureDeviceForm form = new VideoCaptureDeviceForm( );

            if ( form.ShowDialog( this ) == DialogResult.OK )
            {
                // create video source
                VideoCaptureDevice videoSource = form.VideoDevice;

                // open it
                OpenVideoSource( videoSource );
            }
        }

        // Open video file using DirectShow
        private void openVideofileusingDirectShowToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( openFileDialog.ShowDialog( ) == DialogResult.OK )
            {
                // create video source
                FileVideoSource fileSource = new FileVideoSource(openFileDialog.FileName);
                //FileVideoSource fileSource = new FileVideoSource("single.avi");
                // open it
                OpenVideoSource( fileSource );
            }
        }

        // Open JPEG URL
        private void openJPEGURLToolStripMenuItem_Click( object sender, EventArgs e )
        {
            URLForm form = new URLForm( );

            form.Description = "Enter URL of an updating JPEG from a web camera:";
            form.URLs = new string[]
				{
					"http://195.243.185.195/axis-cgi/jpg/image.cgi?camera=1",
				};

            if ( form.ShowDialog( this ) == DialogResult.OK )
            {
                // create video source
                JPEGStream jpegSource = new JPEGStream( form.URL );

                // open it
                OpenVideoSource( jpegSource );
            }
        }

        // Open MJPEG URL
        private void openMJPEGURLToolStripMenuItem_Click( object sender, EventArgs e )
        {
            URLForm form = new URLForm( );

            form.Description = "Enter URL of an MJPEG video stream:";
            form.URLs = new string[]
				{
					"http://195.243.185.195/axis-cgi/mjpg/video.cgi?camera=4",
					"http://195.243.185.195/axis-cgi/mjpg/video.cgi?camera=3",
				};

            if ( form.ShowDialog( this ) == DialogResult.OK )
            {
                // create video source
                MJPEGStream mjpegSource = new MJPEGStream( form.URL );

                // open it
                OpenVideoSource( mjpegSource );
            }
        }

        // Capture 1st display in the system
        private void capture1stDisplayToolStripMenuItem_Click( object sender, EventArgs e )
        {
            OpenVideoSource( new ScreenCaptureStream( Screen.AllScreens[0].Bounds, 100 ) );
        }

        // Open video source
        private void OpenVideoSource( IVideoSource source )
        {
            // set busy cursor
            this.Cursor = Cursors.WaitCursor;

            // stop current video source
            CloseCurrentVideoSource( );

            // start new video source
            videoSourcePlayer.VideoSource = source;
            videoSourcePlayer.Start( );

            // reset stop watch
            stopWatch = null;

            // start timer
            timer.Start( );

            this.Cursor = Cursors.Default;
        }

        // Close video source if it is running
        private void CloseCurrentVideoSource( )
        {
            if ( videoSourcePlayer.VideoSource != null )
            {
                videoSourcePlayer.SignalToStop( );
                
                // wait ~ 3 seconds
                for ( int i = 0; i < 30; i++ )
                {
                    if ( !videoSourcePlayer.IsRunning )
                        break;
                    System.Threading.Thread.Sleep( 100 );
                }

                if ( videoSourcePlayer.IsRunning )
                {
                    videoSourcePlayer.Stop( );
                }
                freeBuffer(uimage_1);
                videoSourcePlayer.VideoSource = null;
            }
        }

        // New frame received by the player
        private void videoSourcePlayer_NewFrame( object sender, ref Bitmap image )
        {
            DateTime now = DateTime.Now;
            Graphics g = Graphics.FromImage( image );
            Bitmap tp = new Bitmap(image);
            tmpBitmap = tp;
            SolidBrush brush = new SolidBrush( Color.Red );
            SolidBrush circle = new SolidBrush(Color.BlueViolet);
            g.DrawString( now.ToString( ), this.Font, brush, new PointF( 5, 5 ) );
            //Pen greenPen = new Pen(Color.Green, 3);
            Pen greenPen = new Pen(Color.CornflowerBlue, 3);
            System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 16);
            g.DrawString( "最佳注册框", drawFont, circle, new PointF(5*image.Width / 12, image.Height / 12) );
            Point point1 = new Point(image.Width/3, image.Height/4);
            Point point2 = new Point(2*image.Width / 3, image.Height / 4);
            Point point3 = new Point(2*image.Width / 3, 3 * image.Height / 4);
            Point point4 = new Point( image.Width / 3, 3 * image.Height / 4);
            Point[] curvePoints = { point1, point2, point3, point4 };
            float tension = 0.8F;
            FillMode aFillMode = FillMode.Alternate;
            g.DrawClosedCurve(greenPen, curvePoints, tension,aFillMode);
            brush.Dispose( );
            g.Dispose( );
        }
        // On timer event - gather statistics
        private void timer_Tick( object sender, EventArgs e )
        {
            IVideoSource videoSource = videoSourcePlayer.VideoSource;

            if ( videoSource != null )
            {
                // get number of frames since the last timer tick
                int framesReceived = videoSource.FramesReceived;

                if ( stopWatch == null )
                {
                    stopWatch = new Stopwatch( );
                    stopWatch.Start( );
                }
                else
                {
                    stopWatch.Stop( );
                    float fps = 1000.0f * framesReceived / stopWatch.ElapsedMilliseconds;
                    fpsLabel.Text = fps.ToString( "F2" ) + " fps";
                    stopWatch.Reset( );
                    stopWatch.Start( );
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            IVideoSource videoSource = videoSourcePlayer.VideoSource;
            if (videoSource != null)
            {
                createBuffer(tmpBitmap, ref uimage_1);
                Bitmap2UImage(tmpBitmap, ref uimage_1);
                Bitmap saveImg = new Bitmap(tmpBitmap);
                int res = judge.isQualified(uimage_1);               
                Console.WriteLine(res);
                if (res == 0)
                {
                    MessageBox.Show("恭喜你，采集成功了", "采集成功");
                    ImageFormat format = ImageFormat.Jpeg;
                    string savestr = now.ToString();
                    savestr = savestr.Replace(":", "_");
                    savestr = savestr.Replace(" ", "_");
                    savestr = savestr.Replace("/", "_");
                    Console.WriteLine(savestr);
                    if (!System.IO.Directory.Exists("image")) {
                        System.IO.Directory.CreateDirectory("image");
                    };
                    saveImg.Save("image/"+savestr + ".jpg", format);
                }
                else if(res==-2){
                    MessageBox.Show("采集失败，未检测到人脸", "采集失败");
                }
                else if (res == -3)
                {
                    MessageBox.Show("采集失败，检测到多张人脸", "采集失败");
                }
                else if (res == -4)
                {
                    MessageBox.Show("采集失败，人脸过小", "采集失败");
                }
                else if (res == -5)
                {
                    MessageBox.Show("采集失败，人脸越界", "采集失败");
                }
                else if (res == -6)
                {
                    MessageBox.Show("采集失败，人脸角度不正常", "采集失败");
                }
                else if (res == -7)
                {
                    MessageBox.Show("采集失败，人脸模糊，或光照不均", "采集失败");
                }
                else {
                    MessageBox.Show("采集失败，请您重新采集", "采集失败");
                }                      
                Console.WriteLine(now.ToString());
                if (videoSourcePlayer.IsRunning)
                {
                    int  image = videoSource.FramesReceived;
                    Console.WriteLine(image);
                }
            }
        }
        static void Bitmap2UImage(Bitmap bitmap, ref UImage uimage)
        {
            for (int i = 0; i < bitmap.Height; i++)
                for (int j = 0; j < bitmap.Width; j++)
                {
                    Marshal.WriteByte(uimage.pixels, (i * bitmap.Width + j) * 3 + 0, bitmap.GetPixel(j, i).B);
                    Marshal.WriteByte(uimage.pixels, (i * bitmap.Width + j) * 3 + 1, bitmap.GetPixel(j, i).G);
                    Marshal.WriteByte(uimage.pixels, (i * bitmap.Width + j) * 3 + 2, bitmap.GetPixel(j, i).R);
                }
        }

        static void createBuffer(Bitmap bitmap, ref UImage uimage)
        {
            uimage.Width = bitmap.Width;
            uimage.Height = bitmap.Height;
            uimage.pixels = Marshal.AllocHGlobal(bitmap.Width * bitmap.Height * 3);
        }

        static void freeBuffer(UImage uimage)
        {
            Marshal.FreeHGlobal(uimage.pixels);
        }

   
    }
}
