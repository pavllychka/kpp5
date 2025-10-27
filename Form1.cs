using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Math = System.Math;


namespace dodatkove5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        
        double f(double x, ref int k1)
        {
            switch (k1)
            {
                case 0: return x * x - 4;
                case 1: return 3 * x - 4 * Math.Log(x) - 5;
                case 2: return x * x * x - 8; 
            }
            return 0;
        }

        
        double fp(double x, double d, ref int k1)
        {
            return (f(x + d, ref k1) - f(x, ref k1)) / d;
        }

       
        double f2p(double x, double d, ref int k1)
        {
            return (f(x + d, ref k1) + f(x - d, ref k1) - 2 * f(x, ref k1)) / (d * d);
        }

       
        void MDP(double a, double b, double Eps, ref int k1, ref int L, out double root)
        {
            double c = 0, Fc;
            while (b - a > Eps)
            {
                c = 0.5 * (b - a) + a;
                L++;
                Fc = f(c, ref k1);
                if (Math.Abs(Fc) < Eps)
                {
                    root = c;
                    return;
                }
                if (f(a, ref k1) * Fc > 0) a = c;
                else b = c;
            }
            root = c;
        }

        
        void MN(double a, double b, double Eps, ref int k1, int Kmax, ref int L, out double root, out bool success)
        {
            double x, Dx, D;
            int i;
            Dx = 0.0;
            D = Eps / 100.0;
            x = b;
            if (f(x, ref k1) * f2p(x, D, ref k1) < 0) x = a;
            if (f(x, ref k1) * f2p(x, D, ref k1) < 0)
                MessageBox.Show("Для цього рівняння збіжність ітерацій не гарантована");

            for (i = 1; i <= Kmax; i++)
            {
                Dx = f(x, ref k1) / fp(x, D, ref k1);
                x = x - Dx;
                if (Math.Abs(Dx) < Eps)
                {
                    L = i;
                    root = x;
                    success = true; 
                    return;
                }
            }
            MessageBox.Show("За задану кількість ітерацій кореня не знайдено");
            root = -1000.0; 
            success = false; 
        }

        
        private void rbMetodMDP_CheckedChanged(object sender, EventArgs e)
        {
           
            bool isNewton = rbMetodMN.Checked;

            
            label7.Visible = isNewton;
            textBox4.Visible = isNewton;

            
            textBox1.Clear();
            textBox2.Clear();
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            int L = 0, k = -1, Kmax = 0, m = -1;
            double D = 0, Eps = 0, a = 0, b = 0;

            try
            {
                
                if (rbMetodMDP.Checked) m = 0;
                else if (rbMetodMN.Checked)
                {
                    m = 1;
                    label7.Visible = true;
                    textBox4.Visible = true;
                    textBox4.Enabled = true;
                }

                if (m == -1)
                {
                    MessageBox.Show("Оберіть метод !");
                    return;
                }

                textBox1.Enabled = true;
                textBox2.Enabled = true;

                
                if (rbEq1.Checked) k = 0;
                else if (rbEq2.Checked) k = 1;
                else if (rbEq3.Checked) k = 2; 

                if (k == -1)
                {
                    MessageBox.Show("Оберіть рівняння !");
                    return;
                }

                
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Введіть число в textBox1");
                    textBox1.Focus();
                    return;
                }
                a = Convert.ToDouble(textBox1.Text);
                if (string.IsNullOrEmpty(textBox2.Text))
                {
                    MessageBox.Show("Введіть число в textBox2");
                    textBox2.Focus();
                    return;
                }
                b = Convert.ToDouble(textBox2.Text);
                if (a > b)
                {
                    D = a; a = b; b = D;
                    textBox1.Text = a.ToString();
                    textBox2.Text = b.ToString();
                }
                if (string.IsNullOrEmpty(textBox3.Text))
                {
                    MessageBox.Show("Введіть число в textBox3");
                    textBox3.Focus();
                    return;
                }
                Eps = Convert.ToDouble(textBox3.Text);
                if (Eps > 0.1 || Eps <= 0)
                {
                    Eps = 0.0001;
                    textBox3.Text = Eps.ToString();
                }
                if (m == 0 && f(a, ref k) * f(b, ref k) > 0)
                {
                    MessageBox.Show("Введіть правильний інтервал [a, b]!");
                    textBox1.Text = "";
                    textBox2.Text = "";
                    textBox1.Focus();
                    return;
                }
                if (Math.Abs(f(a, ref k)) < Eps)
                {
                    textBox5.Text = a.ToString();
                    textBox6.Text = L.ToString();
                    return;
                }
                if (Math.Abs(f(b, ref k)) < Eps)
                {
                    textBox5.Text = b.ToString();
                    textBox6.Text = L.ToString();
                    return;
                }

                
                double resultRoot; 

                switch (m)
                {
                    case 0: 
                        MDP(a, b, Eps, ref k, ref L, out resultRoot);
                        textBox5.Text = resultRoot.ToString();
                        textBox6.Text = L.ToString();
                        label10.Text = "К-ть поділів =";
                        break;
                    case 1: 
                        if (string.IsNullOrEmpty(textBox4.Text))
                        {
                            MessageBox.Show("Введіть число в textBox4");
                            textBox4.Focus();
                            return;
                        }
                        Kmax = Convert.ToInt32(textBox4.Text);

                        bool success;
                        MN(a, b, Eps, ref k, Kmax, ref L, out resultRoot, out success);

                        if (success)
                        {
                            textBox5.Text = resultRoot.ToString();
                            textBox6.Text = L.ToString();
                            label10.Text = "К-ть ітерац.=";
                        }
                        else
                        {
                            
                            textBox5.Text = "Помилка";
                            textBox6.Text = "N/A";
                        }
                        break;
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Некоректне введення чисел!");
                textBox1.Focus();
            }
        }

       
        private void button3_Click(object sender, EventArgs e)
        {

        }


        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

       
    }
}