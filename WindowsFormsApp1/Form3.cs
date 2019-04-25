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
    public partial class Form3 : Form
    {
        public Counts.Params param,param1,param2;
        public Form3()
        {
            InitializeComponent();
            label6.Hide();
        }



        public Form3(Counts.Params param)
        {
            InitializeComponent();
            label6.Hide();
            this.param = param;
            param2 = param;
            textBox1.Text = param.n.ToString();
            textBox2.Text = param.maxtime.ToString();
            textBox3.Text = param.np.ToString();
            textBox4.Text = param.e1.ToString();
            textBox5.Text = param.e2.ToString();
            this.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            param.n = Convert.ToInt32(textBox1.Text);
            param.maxtime = Convert.ToInt32(textBox2.Text);
            param.np = Convert.ToInt32(textBox3.Text);
            param.e1 = Convert.ToDouble(textBox4.Text);
            param.e2 = Convert.ToDouble(textBox5.Text);
            Counts.paramset(param.n, param.maxtime, param.np, param.e1, param.e2,ref param1);
            if (param.n != param1.n || param.maxtime != param1.maxtime || param.np != param1.np || param.e1 != param1.e1 || param.e2 != param1.e2) 
            {
                param = param1;
                textBox1.Text = param.n.ToString();
                textBox2.Text = param.maxtime.ToString();
                textBox3.Text = param.np.ToString();
                textBox4.Text = param.e1.ToString();
                textBox5.Text = param.e2.ToString();
                label6.Show();
            }
            else
            {
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            param = param2;
            this.Close();
        }
    }
}
