using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        public int[] N;
        public Form2()
        {
            InitializeComponent();
        }
        public Form2(int[] n)
        {
            InitializeComponent();
            this.textBox1.Text = n[0].ToString();
            N = n;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int i = (int)Convert.ToDouble(textBox1.Text);
            if (N[1] < 5)
                N[1] = 5;
            if(i<5)
            {
                i = 5;
                this.textBox1.Text = i.ToString();
            }
            else if (i > N[1])
            {
                i=N[1];
                this.textBox1.Text = i.ToString();
            } 
            else 
            {
                N[0] = i;
                this.Close();
            }
                
        }

    }
}
