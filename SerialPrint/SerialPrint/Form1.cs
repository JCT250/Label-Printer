using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;
using System.IO.Ports;
using System.Drawing.Imaging;
using System.Threading;


namespace SerialPrint
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        

        public SerialPort _serialPort = new SerialPort("COM3", 115200, Parity.None, 8, StopBits.One);

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        private delegate void SetTextDeleg(string text);

        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(500);
            string data = _serialPort.ReadLine();
            this.BeginInvoke(new SetTextDeleg(si_DataReceived), new object[] { data });
        }

        private void si_DataReceived(string data) { textBox1.Text = data.Trim(); }

        static byte Reverse(byte b)
        {
            byte b0 = b;
            byte b1 = b;
            byte b2 = b;
            byte b3 = b;
            byte b4 = b;
            byte b5 = b;
            byte b6 = b;
            byte b7 = b;
            int result = 0x00;

            if ((b0 & 0x01) == 0) result = result + 0x01;
            if ((b1 & 0x02) == 0) result = result + 0x02;
            if ((b2 & 0x04) == 0) result = result + 0x04;
            if ((b3 & 0x08) == 0) result = result + 0x08;
            if ((b4 & 0x10) == 0) result = result + 0x10;
            if ((b5 & 0x20) == 0) result = result + 0x20;
            if ((b6 & 0x40) == 0) result = result + 0x40;
            if ((b7 & 0x80) == 0) result = result + 0x80;

            byte result2;

            result2 = (byte)(result);

            return result2;
        }

        private void button1_Click(object sender, EventArgs e)
        {

           
            Image bmp1 = Bitmap.FromFile(@"C:\in\maze1a.bmp");

            Bitmap bmp = bmp1 as Bitmap ;

            //Image bmp2 = bmp1;

            

            

            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
        
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            
            

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap. 
            int bytes  = bmpData.Stride * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            bmp.UnlockBits(bmpData);

            pictureBox1.Width = bmp.Width;
            pictureBox1.Height = bmp.Height;
            pictureBox1.Image = bmp;

            int m = 0;

            for (m = 0; m < rgbValues.Length; m++)
            {

                rgbValues[m] = Reverse(rgbValues[m]);
            }

            byte[] test = new byte[6400];

           int row;

            int column;

            int toggle = 0;
            for (row=0;row<80;row++)
            {
                
                if (toggle == 0)
                    {
                    for (column = 0; column < 80; column++)
                        {
                    
                            test[(row * 80) + column] = 0xFF;
                        }
                    toggle = 1;
                }
                else if(toggle == 1)
                {    
                    for (column = 0; column < 80; column++)
                
                    {
                        test[(row * 80) + column] = 0x00;
                    }
                    toggle = 0;
                        
                }
                
            }  

            try
            {
                if (!(_serialPort.IsOpen))
                    _serialPort.Open();

                Byte[] initialise = { 0x1B, 0x40 };
                _serialPort.Write(initialise, 0, 2);

                Byte[] status_return = { 0x1B, 0x69, 0x53 };
                _serialPort.Write(status_return, 0, 3);

               Byte[] print_information = { 0x1B, 0x69, 0x7A, 0x0E, 0x0B, 0x34, 0x1D, 0x0F, 0x01, 0x00, 0x00, 0x00, 0x00 };
                _serialPort.Write(print_information, 0, 13);

                Byte[] each_mode = { 0x1B, 0x69, 0x4D, 0x00 };
                _serialPort.Write(each_mode, 0, 4);

                Byte[] auto_cut_n = { 0x1B, 0x69, 0x41, 0x01 };
                _serialPort.Write(auto_cut_n, 0, 4);

                Byte[] expanded_mode = { 0x1B, 0x69, 0x4B, 0x00 };
                _serialPort.Write(expanded_mode, 0, 4);

                Byte[] compression_mode = { 0x4D, 0x00 };
                _serialPort.Write(compression_mode, 0, 2);

                Byte[] print_header = { 0x67, 0x00, 0x5A, 0x00, 0x00, 0x00, 0x00, 0x00};
            
                Byte[] print_footer = { 0x00, 0x00, 0x00, 0x00, 0x00 };

               
                int i = 0;
                //MessageBox.Show(dog2.Length.ToString());
                while (i < bmp.Height)
                {


                    _serialPort.Write(print_header, 0,8);
                    _serialPort.Write(rgbValues, (i * 80), 80);
                    _serialPort.Write(print_footer, 0, 5);

                    i++;
                }
                

                Byte[] print_command = { 0x1A};
                _serialPort.Write(print_command, 0, 1);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex.Message, "Error!");
            }

            //_serialPort.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _serialPort.Handshake = Handshake.None;
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
        }
    }
}
