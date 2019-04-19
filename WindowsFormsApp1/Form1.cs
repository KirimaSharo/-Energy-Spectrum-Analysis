using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using OpenGL;
using static System.Math;



namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private Counts data;

        private double DataOriginofDrawX;
        private double DataOriginofDrawY;
        private double DrawTimeX;
        private double DrawTimeY;

        private int smoothn;
        private double[] drawsmooth;

        private int PeakFindMode;
        private int ROIStart;
        private int ROIEnd;


        private int P1Width;
        private int P1Height;
        private int MouseX;
        private int MouseY;
        private bool P1MouseDown = false;
        


        public Form1()
        {
            InitializeComponent();
            GLAUX.InitGL(panel1.Handle);
            GL.glClearColor(0, 1, 1, 0);
            DataOriginofDrawX = 0;
            DataOriginofDrawY = 0;
            DrawTimeX = 1;
            DrawTimeY = 1;
            P1Width = panel1.Width;
            P1Height = panel1.Height;

            PeakFindMode = 3;
            原始数据RawDataToolStripMenuItem.Checked = true;
            

            this.panel1.MouseWheel += new MouseEventHandler(panel1_MouseWheel);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Draw()
        {
            if (data == null)
                return;
            double dx = (double)panel1.Width / data.TotalChannel;
            double dy = (double)panel1.Height / data.Max;


            int originX = (int)(DataOriginofDrawX % 1 * DrawTimeX * dx);
            int originY = (int)(DataOriginofDrawY % 1 * DrawTimeY * dy);

            int climit = (int)Ceiling(panel1.Width / DrawTimeX / dx + DataOriginofDrawX % 1);
            int nlimit = (int)Ceiling(panel1.Height / DrawTimeY / dy + DataOriginofDrawY % 1);

            double c = 0;

            GL.glClear(GLCONST.GL_COLOR_BUFFER_BIT);
            GL.glViewport(-originX, -originY, (int)(climit * DrawTimeX * dx), (int)(nlimit * DrawTimeY * dy));
            GL.glPointSize((float)(dx*DrawTimeX));
            GL.glBegin(GLCONST.GL_LINES);
            GL.glColor3(1.0, 0, 0);
            for (int i = (int)(DataOriginofDrawX); i < Min(data.TotalChannel, DataOriginofDrawX + climit); i++)
            {
                if(原始数据RawDataToolStripMenuItem.Checked)
                {
                    c = data.GetNumber(0, i);
                }
                else if(光滑SmoothToolStripMenuItem.Checked)
                {
                    c = drawsmooth[i];
                }
                GL.glVertex3(2.0 * (i - (int)DataOriginofDrawX) / climit - 1, 2.0 * (c - (int)DataOriginofDrawY) / nlimit - 1, 0);
                GL.glVertex3(2.0 * (i - (int)DataOriginofDrawX) / climit - 1, 2.0 * (0 - (int)DataOriginofDrawY) / nlimit - 1, 0);
            }
            GL.glEnd();
            GL.glFlush();
            GLAUX.SwapBuffers(panel1.Handle);
        }


        /// <summary>
        /// True means Data has been fixed
        /// </summary>
        private bool DrawDataReNew(double OriginX, double OriginY, double TimeX, double TimeY)
        { 
            bool R= false;
            double dx = (double)panel1.Width / data.TotalChannel;
            double dy = (double)panel1.Height / data.Max;
            int climit = (int)Ceiling(panel1.Width / TimeX / dx + OriginX % 1);
            int nlimit = (int)Ceiling(panel1.Height / TimeY / dy + OriginY % 1);

            if (TimeX<1)
            {
                R = true;
                TimeX = 1;
                OriginX = 0;
            }
            else if(OriginX+climit>data.TotalChannel)
            {
                R = true;
                OriginX =data.TotalChannel- panel1.Width / TimeX / dx;
                if(OriginX<0)
                {
                    OriginX = 0;
                    TimeX = 1;
                }
            }
            else if(OriginX<0)
            {
                R = true;
                OriginX = 0;
            }

            if(TimeY<1)
            {
                R = true;
                TimeY = 1;
                OriginY = 0;
            }
            else if (OriginY + nlimit > data.Max)
            {
                R = true;
                OriginY =data.Max - panel1.Height / TimeY/dy;
                if (OriginY < 0)
                {
                    OriginY = 0;
                    TimeY = 1;
                }
            }
            else if (OriginY < 0)
            {
                R = true;
                OriginY = 0;
            }

            DataOriginofDrawX = OriginX;
            DataOriginofDrawY = OriginY;
            DrawTimeX = TimeX;
            DrawTimeY = TimeY;


            return R;
        }


        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if(button1.BackColor==Color.LightSkyBlue)
                {
                    if(data!=null)
                    {
                        double dx = (double)panel1.Width / data.TotalChannel;
                        double deltax = e.X / DrawTimeX / dx + DataOriginofDrawX;
                        int x = (int)(deltax + 0.5);
                        if(textBox1.Text.Length < 1)
                        {
                            textBox1.Text = x.ToString();
                            ROIStart = x;
                        }
                        else
                        {
                            if(x < ROIStart)
                            {
                                ROIEnd = ROIStart;
                                textBox2.Text = ROIEnd.ToString();
                                textBox1.Text = x.ToString();
                                ROIStart = x;
                            }
                            else
                            {
                                textBox2.Text = x.ToString();
                                ROIEnd = x;
                            }
                            button1.BackColor = Color.White;
                        }
                    }
                    
                }
                return;
            }

            MouseX = e.X;
            MouseY = e.Y;
            P1MouseDown = true;
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            P1MouseDown = false;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!P1MouseDown)
                return;
            if (data == null)
                return;
            double dx = (double)panel1.Width / data.TotalChannel;
            double dy = (double)panel1.Height / data.Max;


            double deltax = (MouseX - e.X) / DrawTimeX / dx + DataOriginofDrawX;
            double deltay = (e.Y - MouseY) / DrawTimeY / dy + DataOriginofDrawY;

            DrawDataReNew(deltax, deltay, DrawTimeX, DrawTimeY);

            Draw();

            MouseX = e.X;
            MouseY = e.Y;
        }

        private void panel1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (data == null)
                return;
            double deltax;
            double deltay;
            double TimeX;
            double TimeY;

            double dx = (double)panel1.Width / data.TotalChannel;
            double dy = (double)panel1.Height / data.Max;
            double ratio;
            deltax = DataOriginofDrawX % 1 + (double)e.X / dx / DrawTimeX;
            deltay = DataOriginofDrawY % 1 + (panel1.Height-(double)e.Y) / dy / DrawTimeY;

            if (e.Delta > 0)
                ratio = 1.1;
            else
                ratio = 0.9;
            deltax = deltax * (1 - 1 / ratio) + DataOriginofDrawX;
            deltay = deltay * (1 - 1 / ratio) + DataOriginofDrawY;

            TimeX = ratio * DrawTimeX;
            TimeY = ratio * DrawTimeY;

            DrawDataReNew(deltax, deltay, TimeX, TimeY);

            Draw();
        }

        private void panel1_SizeChanged(object sender, EventArgs e)
        {
            if (panel1.Height == 0 || panel1.Width == 0)
                return;

            double TimeX;
            double TimeY;

            if (data != null)
            {
                TimeX = DrawTimeX / panel1.Width * P1Width;
                TimeY = DrawTimeY / panel1.Height * P1Height;

                double dy = (double)panel1.Height / data.Max;
                int nlimit1 = (int)Ceiling(panel1.Height / DrawTimeY / dy + DataOriginofDrawY % 1);
                int nlimit2 = (int)Ceiling(panel1.Height / TimeY / dy + DataOriginofDrawY % 1);
                double OriginY = DataOriginofDrawY - nlimit2 + nlimit1;

                DrawDataReNew(DataOriginofDrawX, OriginY, TimeX, TimeY);
            }

            P1Width = panel1.Width;
            P1Height = panel1.Height;
            Draw();
            return;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.BackColor != Color.LightSkyBlue)
                button1.BackColor = Color.LightSkyBlue;
            else
                button1.BackColor = Color.White;
            textBox1.Text = null;
            textBox2.Text = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ROIEnd <= ROIStart)
                return;
            Peak[] ret=null;
            if (PeakFindMode == 0)
            {
                data.Smooth((ROIEnd - ROIStart) / 6, -1);
                while (data.Maxium(0, ROIStart, ROIEnd) > 1)
                    data.Smooth((ROIEnd - ROIStart) / 6, 1);
                    
                ret = new Peak[1];
                ret[0].U = data.Maxium(1, ROIStart, ROIEnd);
                int i = (int)ret[0].U;
                int j = i + 1;
                int m = (int)(ret[0].U + 0.5);
                double s = 0;
                while (data.GetNumber(1, i - 1) - data.GetNumber(1, i - 2) > data.GetNumber(1, i) - data.GetNumber(1, i - 1) && i >= 0)
                {
                    s += data.GetNumber(0, i);
                    i--; 
                }
                while (data.GetNumber(1, j + 1) - data.GetNumber(1, j + 2) > data.GetNumber(1, j) - data.GetNumber(1, j + 1) && j < data.TotalChannel)
                {
                    s += data.GetNumber(0, j);
                    j++;
                }
                m = j - i;
                ret[0].S = m;
                ret[0].A = s;

            }
            else if (PeakFindMode == 1)
            {
                ret = data.ExpectMax(1, ROIStart, ROIEnd);
            }
            else if (PeakFindMode == 2)
            {
                ret = data.Partial(1, ROIStart, ROIEnd, 200, 1000);
            }
            else if (PeakFindMode == 3)
            {
                ret = data.LevenbergMarquardt(3, ROIStart, ROIEnd, 1000, 1, 0, 0);
            }

            if (ret == null)
                return;
            listView1.BeginUpdate();
            for(int i=0;i<ret.Length;i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = ret[i].U.ToString();
                lvi.SubItems.Add(ret[i].S.ToString());
                lvi.SubItems.Add(ret[i].A.ToString());
                this.listView1.Items.Add(lvi);
            }
            listView1.EndUpdate();
        }




        private void 打开OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Data Files| *.txt;*.dat";
            open.Title = "打开文件";
            if(open.ShowDialog()==DialogResult.OK)
            {
                data = new Counts(open.FileName);
                Draw();

            }
            
        }

        private void 另存位SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Data Files(*.TXT)| *.txt|Data Files(*.DAT)| *.dat";
            save.FileName = "NewSave";
            save.ShowDialog();
        }

        private void 原始数据RawDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            原始数据RawDataToolStripMenuItem.Checked = true;
            光滑SmoothToolStripMenuItem.Checked = false;
        }

        private void 光滑SmoothToolStripMenuItem_Click(object sender, EventArgs e)
        {
            原始数据RawDataToolStripMenuItem.Checked = false;
            光滑SmoothToolStripMenuItem.Checked = true;
            
        }

        private void 参数自动ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (data == null)
                return;
            参数自动ToolStripMenuItem.Checked = true;
            原始数据RawDataToolStripMenuItem.Checked = false;
            光滑SmoothToolStripMenuItem.Checked = true;
            smoothn = data.TotalChannel / 20;
            data.Smooth(smoothn, ref drawsmooth);
            Draw();
        }

        private void 参数设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (data == null)
                return;
            参数自动ToolStripMenuItem.Checked = false;
            原始数据RawDataToolStripMenuItem.Checked = false;
            光滑SmoothToolStripMenuItem.Checked = true;
            int[] n = new int[2];
            n[0] = smoothn;
            n[1] = data.TotalChannel / 3;
            Form f2 = new Form2(n);
            f2.FormClosing +=new FormClosingEventHandler(Form2Closing);
            f2.FormClosed += new FormClosedEventHandler(Form2Closed);
            f2.Show();
        }
        private void Form2Closing(object sender,EventArgs e)
        {
            smoothn = ((Form2)sender).N[0];
        }

        private void Form2Closed(object sender, EventArgs e)
        {
            data.Smooth(smoothn, ref drawsmooth);
            Draw();
        }
    }
}
