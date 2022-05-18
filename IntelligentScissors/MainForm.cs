using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IntelligentScissors
{
    public partial class MainForm : Form
    {
        int mouseX , mouseY ;
        List<KeyValuePair<int, int>> lastPath;
        bool LiveWire;
        public MainForm()   
        {
            LiveWire = false;
            lastPath = new List<KeyValuePair<int, int>>();
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix;
        Graph ImageGraph;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
                ImageGraph = new Graph(ImageMatrix);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value ;
            ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            LiveWire = true;
            /*mouseX = e.X;
            mouseY = e.Y;
            pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);*/
            lastPath.Clear();
            ImageGraph.SetCurAnchor(e.X, e.Y);
            List<KeyValuePair<int, int>> Path = ImageGraph.GetShortestPath(ImageGraph.GetLastAnchor());
            if (Path.Count > 0)
            {
                ImageOperations.Update(ImageMatrix, Path, pictureBox1);
            }
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ImageGraph.SetCurAnchor(e.X, e.Y);
            List<KeyValuePair<int, int>> Path = ImageGraph.GetShortestPath(ImageGraph.GetStartAnchor());
            if (Path.Count > 0)
            {
                ImageOperations.Update(ImageMatrix, Path, pictureBox1);
            }
            LiveWire = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!LiveWire || ImageGraph == null || ImageGraph.GetStartAnchor() == -1)
                return;
            if (lastPath.Count > 0)
            {
                ImageOperations.Update2(ImageMatrix, lastPath, pictureBox1);
            }
            List<KeyValuePair<int, int>> Path = ImageGraph.GetShortestPath(ImageGraph.GetIndex(e.X,e.Y));
            if (Path.Count > 0)
            {
                ImageOperations.Update(ImageMatrix, Path, pictureBox1);
            }
            lastPath = Path;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Pen blackPen = new Pen(Color.Red, 10);
            // Create location and size of rectangle.
            int width = 50;
            int height = 50;

            // Draw rectangle to screen.
            e.Graphics.DrawRectangle(blackPen, mouseX, mouseY, width, height);
            pictureBox1.Refresh();
        }
    }
}