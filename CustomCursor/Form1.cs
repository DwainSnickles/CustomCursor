using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

//Visit http://sourcecodedepot.com for more progamming needs

namespace CustomCursor
{
    public partial class Form1 : Form
    {
        public PictureBox selected { get; private set; }
        public bool DragDropOK { get; private set; }

        int ImageHeight; int ImageWidth;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set image for cursor or use defaultcursor
        /// </summary>
        /// <param name="image"></param>
        /// <param name="UseCustomImage"></param>
        void DragImage(Image image, Boolean UseCustomImage)
        {
           
            //Create a new Image 
            Bitmap bmp = new Bitmap(image.Width, image.Height);

            //draw the source image to new image
            picSource.DrawToBitmap(bmp, new Rectangle(Point.Empty, bmp.Size));

            if (ckbScaleImage.Checked)
            {
                bmp = (Bitmap)ResizeImage(bmp, new Size(Convert.ToInt32(txtScaleImage.Text), Convert.ToInt32(txtScaleImage.Text)));

            }

            //in order to check if drag drop ok have to include width and height
            ImageWidth = bmp.Width;
            ImageHeight = bmp.Height;

            //optionally define a transparent color
            //bmp.MakeTransparent(Color.White);

            //create new cursor to drag
            Cursor cur = new Cursor(bmp.GetHicon());
            Cursor.Current = cur;
        }

        /// <summary>
        /// On mouse down we set custom cursor if left button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picSource_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                DragImage(picSource.Image, true) ;
            };
        }

        private void picSource_MouseUp(object sender, MouseEventArgs e)
        {
            //Reset Cursor to default
            Cursor = Cursors.Default;

            if (DragDropOK)
            {
                //Set dest image
                picDestination.Image = picSource.Image;
                DragDropOK = false;
                picDestination.Refresh();
            }
        }

        /// <summary>
        /// Resizes Image keepping Aspect
        /// </summary>
        /// <param name="imgToResize"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private static Image ResizeImage(System.Drawing.Image imgToResize, Size size)
        {
            // Get the image current width
            int sourceWidth = imgToResize.Width;
            // Get the image current height
            int sourceHeight = imgToResize.Height;
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            // Calculate width and height with new desired size
            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);
            nPercent = Math.Min(nPercentW, nPercentH);
            // New Width and Height
            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);
            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // Draw image with new width and height
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return (Image)b;
        }

        /// <summary>
        /// Only allow integers in textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtScaleImage_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }


        private void picSource_MouseMove(object sender, MouseEventArgs e)
        {
           //rect 1 and 2 collide allow image to be dropped
            var rect1 = new Rectangle(picDestination.Location, picDestination.Size);

            var rect2 = new Rectangle(e.Location, picSource.Size);

            //Set size differnt if custom cursor is used
            if (ckbScaleImage.Checked)
            {
                rect2 = new Rectangle(e.Location, new Size(ImageWidth, ImageHeight)); 
            }

            if (rect1.IntersectsWith(rect2))
            {
                //code when collided
                DragDropOK = true;
                picDestination.Refresh();
                lblDropImageOK.Visible = true;
            }
            else
            {
                DragDropOK=false;
            }
        }

        private void picDestination_Paint(object sender, PaintEventArgs e)
        {
            if (DragDropOK)
            {
                //Create yelloyish border if ok to drop image
                ControlPaint.DrawBorder(e.Graphics, picDestination.ClientRectangle,
                  Color.GreenYellow, 4, ButtonBorderStyle.Solid,  // Left
                  Color.GreenYellow, 4, ButtonBorderStyle.Solid,  // Top
                  Color.GreenYellow, 4, ButtonBorderStyle.Solid,  // Right
                  Color.GreenYellow, 4, ButtonBorderStyle.Solid); // Bottom
            }
            else
            {
                lblDropImageOK.Visible = false;

                //Remove border after drop or any time drag and drop not permitted
                ControlPaint.DrawBorder(e.Graphics, picDestination.ClientRectangle,
                 Color.Transparent, 0, ButtonBorderStyle.None,  // Left
                 Color.Transparent, 0, ButtonBorderStyle.None,  // Top
                 Color.Transparent, 0, ButtonBorderStyle.None,  // Right
                 Color.Transparent, 0, ButtonBorderStyle.None); // Bottom
            }
           
        }

        //Used For Refreshing picDestination only
        private void button1_Click(object sender, EventArgs e)
        {
            picDestination.Image = null;
        }

    }
}
