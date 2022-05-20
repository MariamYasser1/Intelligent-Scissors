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
        List<KeyValuePair<int, int>> LastPath;
        List<Color> LastPathColors;
        bool LiveWire;
        RGBPixel[,] ImageMatrix;
        Graph ImageGraph;

        public MainForm()   
        {
            LiveWire = false;
            LastPath = new List<KeyValuePair<int, int>>();
            LastPathColors = new List<Color>();
            InitializeComponent();
        }
        
        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                TestingHandling.SetImageFilePath(OpenedFilePath);
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
                ImageGraph = new Graph(ImageMatrix);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

        private void DrawAnchor(MouseEventArgs e)
        {
            Graphics gBmp = Graphics.FromImage(ImageOperations.ImageBMP);
            gBmp.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

            Brush redBrush = new SolidBrush(Color.Red);
            gBmp.FillRectangle(redBrush, e.X - 2, e.Y - 2, 4, 4);

            pictureBox1.Image = ImageOperations.ImageBMP;
        }

        private void DrawShortestPath( int x , int y , bool fix , bool fill)
        {
            List<KeyValuePair<int, int>> Path = ImageGraph.GetShortestPath(ImageGraph.GetIndex(x,y),false);
            if (fill)
            {
                if (LastPath.Count > 0)
                {
                    ImageOperations.Update2(ImageMatrix, LastPath, pictureBox1, LastPathColors);
                }
                LastPathColors.Clear();
                foreach (var node in Path)
                {
                    LastPathColors.Add(ImageOperations.ImageBMP.GetPixel(node.Key, node.Value));
                }
            }
            LastPath.Clear();
            if (Path.Count > 0)
            {
                ImageOperations.Update(ImageMatrix, Path, pictureBox1);
            }
            if (fix)
            {
                foreach (var node in Path)
                    ImageGraph.Fix(node.Key, node.Value);
            }
            else
            {
                LastPath = Path;
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            LiveWire = true;
            ImageGraph.SetCurAnchor(e.X, e.Y,true);
            var pair = ImageGraph.GetCoordinates(ImageGraph.GetLastAnchor());
            DrawShortestPath(pair.Key, pair.Value, true, false);
            DrawAnchor(e);
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            LiveWire = false;
            ImageGraph.SetCurAnchor(e.X, e.Y,true);
            var pair = ImageGraph.GetCoordinates(ImageGraph.GetStartAnchor());
            DrawShortestPath(pair.Key,pair.Value, true, false);
            if (TestingHandling.GetImageFilePath().IndexOf("Complete") != -1 && TestingHandling.GetImageFilePath().IndexOf("Case2") != -1)
            {
                TestingHandling.PrintShortestPath(ImageGraph);
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!LiveWire)
                return;
            DrawShortestPath(e.X, e.Y, false, true);
        }

    }
}