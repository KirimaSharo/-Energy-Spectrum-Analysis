﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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

        private Counts.Params Params;


        public Form1()
        {
            InitializeComponent();
            GLAUX.InitGL(panel1.Handle);
            GL.glClearColor(1, 1, 1, 0);
            DataOriginofDrawX = 0;
            DataOriginofDrawY = 0;
            DrawTimeX = 1;
            DrawTimeY = 1;
            P1Width = panel1.Width;
            P1Height = panel1.Height;
            PeakFindMode = 3;
            levenbergMaToolStripMenuItem.Checked = true;
            原始数据RawDataToolStripMenuItem.Checked = true;
            progressBar1.Hide();

            Counts.paramset(ref Params);

            this.panel1.MouseWheel += new MouseEventHandler(panel1_MouseWheel);
            Counts.peakfound += new Counts.PeakFound(ListView1Update);
            Counts.peakfinding += new Counts.PeakFinding(ProgressBarUpdate);
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
            GL.glBegin(GLCONST.GL_POINTS);
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
            

            double d1 = DataOriginofDrawX + climit;
            double d2 = DataOriginofDrawY + nlimit;
            int StrPxC = 12;
            int StrPxN = 14;
            if(d1<10000000)
                while (d1 >= 10)
                {
                    d1 = d1 / 10;
                    StrPxC += 6;
                }
            else
            {
                    StrPxC = 48;
            }

            int Xaxistime = 1;
            int Yaxistime = 1;
            while (Xaxistime * DrawTimeX * dx < StrPxC)
            {
                Xaxistime *= 2;
                if (Xaxistime * DrawTimeX * dx < StrPxC)
                {
                    Xaxistime = Xaxistime / 2;
                    Xaxistime *= 5;
                }
                if (Xaxistime * DrawTimeX * dx < StrPxC)
                    Xaxistime *= 2;
            }
            while (Yaxistime * DrawTimeY * dy < StrPxN)
            {
                Yaxistime *= 2;
                if (Yaxistime * DrawTimeY * dy < StrPxN)
                {
                    Yaxistime = Yaxistime / 2;
                    Yaxistime *= 5;
                }
                if (Yaxistime * DrawTimeY * dy < StrPxN)
                    Yaxistime *= 2;
            }


            Graphics G = panel2.CreateGraphics();

            Pen P = new Pen(Color.Black);
            Brush B = new SolidBrush(Color.Black);
            G.Clear(Color.White);
            G.DrawLine(P, panel1.Location.X - 1, 0, panel1.Location.X - 1, panel1.Height + 5);
            G.DrawLine(P, panel1.Location.X - 6, panel1.Height, panel2.Width, panel1.Height);
            string str;
            for (int i = Xaxistime * (int)(DataOriginofDrawX / Xaxistime); i < DataOriginofDrawX + climit; i += Xaxistime) 
            {
                for (int k = 1; k < 10; k++)
                {
                    float x1 = (float)(panel1.Location.X - 1 + (i + k * Xaxistime / 10.0 - DataOriginofDrawX) * (DrawTimeX * dx));
                    if (x1 < panel1.Location.X)
                        continue;
                    if (k != 5)
                        G.DrawLine(P, x1, panel1.Height, x1, panel1.Height + 2);
                    else
                        G.DrawLine(P, x1, panel1.Height, x1, panel1.Height + 4);
                }
                float x = (float)(panel1.Location.X - 1 + (i - DataOriginofDrawX) * (DrawTimeX * dx));
                if (x < panel1.Location.X) 
                    continue;
                G.DrawLine(P, x, panel1.Height, x, panel1.Height + 5);
                if (x < panel1.Location.X + 18 + 0.5 * StrPxC) 
                    continue;
                if(i<10000000)
                {
                    str = i.ToString();
                }
                else
                {
                    double i1 = i;
                    int j = 0;
                    while (i1 >= 10)
                    {
                        i1 = i1 / 10;
                        j++;
                    }
                    string str1;
                    str1 = j.ToString();
                    if (str1.Length >= 6)
                        return;
                    str = i1.ToString();
                    str = str.Substring(0, 6 - str1.Length);
                    str += "E";
                    str += str1;
                }
                G.DrawString(str, new Font("Calibri", 9), B, x - str.Length * 3 - 2, panel1.Height + 5);
            }


            for (int i = Yaxistime * (int)(DataOriginofDrawY / Yaxistime); i < DataOriginofDrawY + nlimit; i += Yaxistime)
            {
                for (int k = 1; k < 5; k++)
                {
                    float y1 = (float)(panel1.Height - (i + k * Yaxistime / 5.0 - DataOriginofDrawY) * (DrawTimeY * dy));
                    if (y1 > panel1.Height)
                        continue;
                    G.DrawLine(P, panel1.Location.X - 1, y1, panel1.Location.X - 3, y1);
                }
                float y = (float)(panel1.Height - (i - DataOriginofDrawY) * (DrawTimeY * dy));
                if (y > panel1.Height)
                    continue;
                G.DrawLine(P, panel1.Location.X - 1, y, panel1.Location.X - 6, y);

                if (y > panel1.Height - 7) 
                    continue;
                if (i < 10000000)
                {
                    str = i.ToString();
                }
                else
                {
                    double i1 = i;
                    int j = 0;
                    while (i1 >= 10)
                    {
                        i1 = i1 / 10;
                        j++;
                    }
                    string str1;
                    str1 = j.ToString();
                    if (str1.Length >= 6)
                        return;
                    str = i1.ToString();
                    str = str.Substring(0, 6 - str1.Length);
                    str += "E";
                    str += str1;
                }
                G.DrawString(str, new Font("Calibri", 9), B, panel1.Location.X - 6-6*str.Length, y-6);
            }

            float ii= (float)DataOriginofDrawX;
            if (ii < 100000)
            {
                str = ii.ToString();
                if (str.Length > 7)
                {
                    str = str.Substring(0, 7);
                }
            }
            else
            {
                double i1 = ii;
                int j = 0;
                while (i1 >= 10)
                {
                    i1 = i1 / 10;
                    j++;
                }
                string str1;
                str1 = j.ToString();
                if (str1.Length >= 6)
                    return;
                str = i1.ToString();
                str = str.Substring(0, 6 - str1.Length);
                str += "E";
                str += str1;
            }
            G.DrawString(str, new Font("Calibri", 9), B, panel1.Location.X - str.Length * 3 - 2, panel1.Height + 5);

            ii = (float)DataOriginofDrawY;
            if (ii < 100000)
            {
                str = ii.ToString();
                if (str.Length > 7)
                {
                    str = str.Substring(0, 7);
                }
            }
            else
            {
                double i1 = ii;
                int j = 0;
                while (i1 >= 10)
                {
                    i1 = i1 / 10;
                    j++;
                }
                string str1;
                str1 = j.ToString();
                if (str1.Length >= 6)
                    return;
                str = i1.ToString();
                str = str.Substring(0, 6 - str1.Length);
                str += "E";
                str += str1;
            }
            G.DrawString(str, new Font("Calibri", 9), B, panel1.Location.X - 6 - 6 * str.Length, panel1.Height-6);

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
            if (ROIEnd <= ROIStart || data == null)
                return;
            progressBar1.Show();
            data.Peakfind(PeakFindMode,ROIStart,ROIEnd, Params,Params.n);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (data == null)
                return;
            data.Peakfind(PeakFindMode, Params);
        }



        private delegate void Listup(Peak[] ret);
        private void ListView1Update(Peak[] ret)
        {
            if(this.InvokeRequired)
            {
                Listup listup = new Listup(ListView1Update);
                this.Invoke(listup,ret);
            }
            else
            {
                progressBar1.Hide();
                listView1.BeginUpdate();
                for (int i = 0; i < ret.Length; i++)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = ret[i].U.ToString();
                    lvi.SubItems.Add((ret[i].S*2.335).ToString());
                    lvi.SubItems.Add(ret[i].A.ToString());
                    listView1.Items.Add(lvi);
                }
                listView1.EndUpdate();
            }
            
        }
        private delegate void Progressup(int current, int total);
        private void ProgressBarUpdate(int current,int total)
        {
            if(this.InvokeRequired)
            {
                Progressup progressup = new Progressup(ProgressBarUpdate);
                this.Invoke(progressup, new object[] { current, total });
            }
            else
            {
                progressBar1.Maximum = total;
                progressBar1.Value = current;
                progressBar1.Show();
            }
        }



        private void 打开OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Data Files| *.txt;*.dat";
            open.Title = "打开文件";
            if(open.ShowDialog()==DialogResult.OK)
            {
                data = new Counts(open.FileName);
                smoothn = data.TotalChannel / 20;
                DrawDataReNew(0, 0, 1, 1);
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
            Draw();
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

        private void 期望最大算法ExpectMaxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PeakFindMode = 1;
            期望最大算法ExpectMaxToolStripMenuItem.Checked = true;
            粒子群算法ParticalToolStripMenuItem.Checked = false;
            levenbergMaToolStripMenuItem.Checked = false;
            导数寻峰ToolStripMenuItem.Checked = false;
        }

        private void 粒子群算法ParticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PeakFindMode = 2;
            期望最大算法ExpectMaxToolStripMenuItem.Checked = false;
            粒子群算法ParticalToolStripMenuItem.Checked = true;
            levenbergMaToolStripMenuItem.Checked = false;
            导数寻峰ToolStripMenuItem.Checked = false;
        }

        private void levenbergMaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PeakFindMode = 3;
            期望最大算法ExpectMaxToolStripMenuItem.Checked = false;
            粒子群算法ParticalToolStripMenuItem.Checked = false;
            levenbergMaToolStripMenuItem.Checked = true;
            导数寻峰ToolStripMenuItem.Checked = false;
        }

        private void 导数寻峰ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PeakFindMode = 0;
            期望最大算法ExpectMaxToolStripMenuItem.Checked = false;
            粒子群算法ParticalToolStripMenuItem.Checked = false;
            levenbergMaToolStripMenuItem.Checked = false;
            导数寻峰ToolStripMenuItem.Checked = true;
        }

        private void 参数调整ParamsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Form3 f3 = new Form3(Params);
            f3.FormClosing += new FormClosingEventHandler(Form3Closing);

        }
        private void Form3Closing(object sender, EventArgs e)
        {
            Params = ((Form3)sender).param;
        }


    }
}
