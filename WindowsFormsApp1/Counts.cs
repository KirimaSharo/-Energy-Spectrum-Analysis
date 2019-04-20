using System;
using System.IO;
using static System.Math;
using OpenGL;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public struct Peak
    {
        public double A;
        public double U;
        public double S;
    }
    public class Counts
    {
        private int totalchannel = 0;
        private int[] count;
        private double[] fix;
        private double[] sim;
        private int max = 0;
        private string filepath = null;
        public int TotalChannel
        {
            get
            {
                return totalchannel;
            }
        }
        public int Max
        {
            get
            {
                return max;
            }
        }




        public Counts(int TotalChannel)
        {
            this.totalchannel = TotalChannel;
            this.count = new int[TotalChannel];
            this.fix = null;
            this.sim = null;
        }


        /// <summary>
        /// MUST Use Try/Catch To Avoid Reading Errors
        /// </summary>
        public Counts(int TotalChannel, int[] count)
        {
            this.totalchannel = TotalChannel;
            this.count = new int[TotalChannel];
            for (int i = 0; i < TotalChannel; i++)
            {
                this.count[i] = count[i];
                if (this.max < this.count[i])
                    this.max = this.count[i];
            }
            this.fix = null;
            this.sim = null;
        }

        public Counts(string filepath)
        {
            this.filepath = filepath;
            if (Path.GetExtension(filepath) == ".dat")
            {
                FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.ReadWrite);
                BinaryReader br = new BinaryReader(fs);
                this.totalchannel = br.ReadInt32();
                this.count = new int[this.totalchannel];
                for (int i = 0; i < this.totalchannel; i++)
                {
                    this.count[i] = br.ReadInt32();
                    if (this.max < this.count[i])
                        this.max = this.count[i];
                }
                this.sim = null;
                fs.Close();
            }
        }

        /// <summary>
        /// MUST Use Try/Catch To Avoid Reading Errors
        /// </summary>
        /// <param name="file">
        /// Be Sure That FileStream Located On "Total Channel" Data
        /// </param>
        public Counts(FileStream file)
        {
            BinaryReader br = new BinaryReader(file);
            this.totalchannel = br.ReadInt32();
            this.count = new int[this.totalchannel];
            for (int i = 0; i < this.totalchannel; i++)
            {
                this.count[i] = br.ReadInt32();
                if (this.max < this.count[i])
                    this.max = this.count[i];
            }
            this.fix = null;
            this.sim = null;
        }


        /// <summary>
        /// Getting numbers from private arrays
        /// Return value -1 stands for ERRORS
        /// </summary>
        /// <param name="source">
        /// 0:count;
        /// 1:fix;
        /// 2:sim;
        /// </param>
        public double GetNumber(byte source, int i)
        {
            if (i >= totalchannel)
                return 0;
            else if (source == 0)
                return count[i];
            else if (source == 1 && this.fix != null)
                return fix[i];
            else if (source == 2 && this.sim != null)
                return sim[i];
            else return -1;
        }

        public bool Merger(Counts c)
        {
            if (this.totalchannel < c.totalchannel)
                return false;
            try
            {
                this.max = 0;
                for (int i = 0; i < c.totalchannel; i++)
                {
                    this.count[i] += c.count[i];
                    if (this.max < this.count[i])
                        this.max = this.count[i];
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        public void Reset()
        {
            for (int i = 0; i < this.totalchannel; i++)
                this.count[i] = 0;
            this.max = 0;
        }


        public bool Set(FileStream file)
        {
            BinaryReader br = new BinaryReader(file);
            try
            {
                for (int i = 0; i < this.totalchannel; i++)
                {
                    this.count[i] = br.ReadInt32();
                    if (this.max < this.count[i])
                        this.max = this.count[i];
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void Smooth(int n, ref double[] aim)
        {
            double[] a = this.fix;
            Smooth(n, -5);
            aim = this.fix;
            this.fix = a;
        }

        /// <param name="t">
        ///  time of smooth
        ///  t<0 for Reset Fix
        /// </param>
        public void Smooth(int n, int t)
        {
            int n1 = (n + 1) / 2;
            if (this.totalchannel == 0)
                return;
            int i, j, k;
            double y;
            double[] f = new double[this.TotalChannel];
            double[] x = new double[n];
            if (this.fix == null)
            {
                this.fix = new double[this.TotalChannel];
                for (i = 0; i < this.TotalChannel; i++)
                {
                    this.fix[i] = this.count[i];
                }
            }
            if (t < 0)
            {
                for (i = 0; i < this.TotalChannel; i++)
                {
                    this.fix[i] = this.count[i];
                }
                t = -t;
            }

            for (i = 0; i < n1; i++)
            {
                x[n1 - 1 + i] = 1 + 15 / (Pow(n, 2) - 4) * ((Pow(n, 2) - 1) / 12 - Pow(i, 2));
                x[n1 - 1 - i] = x[n1 - 1 + i];

            }



            for (int a = 0; a < t; a++)
            {

                for (i = 0; i < this.TotalChannel; i++)
                {
                    f[i] = 0;
                    y = 0;
                    for (j = 0; j < n; j++)
                    {
                        k = i - n1 + j + 1;
                        if (k >= 0 && k < this.totalchannel)
                        {
                            f[i] = f[i] + x[j] * this.fix[k];
                            y = y + x[j];
                        }
                    }
                    f[i] = f[i] / y;

                }
                for (i = 0; i < this.TotalChannel; i++)
                {
                    this.fix[i] = f[i];
                }
            }
        }

        public int Maxium(int i, int start, int end)
        {
            if (this.fix == null)
                Smooth((end - start) / 6, 1);

            int ret = 0;
            int r = 0;

            if (i == 0)
            {
                for (i = start + 1; i < end - 1; i++)
                {
                    if (this.fix[i] >= this.fix[i - 1] && this.fix[i] >= this.fix[i + 1])
                        ret++;
                }
                return ret;
            }
            else if (i == 1)
            {
                for (i = start + 1; i < end - 1; i++)
                {
                    if (this.fix[i] >= this.fix[i - 1] && this.fix[i] >= this.fix[i + 1])
                    {
                        r = i;
                        ret++;
                    }

                }
                if (ret == 1)
                    return r;
                else
                    return -1;

            }
            else
                return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n">
        /// Number of peaks,1 or 2
        /// </param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public Peak[] ExpectMax(int n, int start, int end)
        {
            return ExpectMax(n, start, end, 100);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n">
        /// Number of peaks,1 or 2
        /// </param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public Peak[] ExpectMax(int n, int start, int end, int Maxtime)
        {
            if (n != 1 && n != 2)
                return null;

            int i, j, k, l;
            double[,] Ax = new double[3, 3];
            double[] b = new double[3];

            double[] y = new double[3];

            for (i = 0; i < 3; i++)
                y[i] = 0;

            double[,] Lx = new double[3, 3];
            double[,] Ux = new double[3, 3];

            double[,] c = new double[n, end - start + 1];
            double c0, C;


            double[] A = new double[n];
            double[] U = new double[n];
            double[] S = new double[n];

            double[] a = new double[n];
            double[] u = new double[n];
            double[] s = new double[n];

            double total = 0;
            for (i = start; i <= end; i++)
            {
                total += count[i];
            }
            Smooth((end - start) / 6, -1);
            double unnorm = (fix[start] + fix[end]) * (end - start) / 2;

            unnorm = 0.5 + unnorm / total / 2;

            y[0] = (this.count[end] + this.count[start]) / 2.0;
            j = 0;
            for (i = start; i <= end; i++)
            {
                j += this.count[i];
            }
            c0 = IntegrateX3P(y[0], y[1], y[2], 0, start, end) / j;
            y[0] = c0 / (end - start);

            if (n == 1)
            {
                U[0] = (end + start) / 2;
                S[0] = (end - start) / 3;
                A[0] = (1 - c0) / IntegrateNorm(U[0], S[0], start, end);

                for (i = 0; i < Maxtime; i++)
                {
                    a[0] = 0;
                    u[0] = 0;
                    s[0] = 0;
                    for (j = 0; j < 3; j++)
                    {
                        b[j] = 0;
                        for (k = 0; k < 3; k++)
                        {
                            Ax[j, k] = 0;
                            Lx[j, k] = 0;
                            Ux[j, k] = 0;
                        }
                    }

                    for (j = start, k = 0; j <= end; j++, k++)
                    {
                        c0 = Max(X3(j, y[0], y[1], y[2], 0), 0);
                        c[0, k] = A[0] * Norm(j, U[0], S[0]);
                        C = c0 + c[0, k];
                        c0 = c0 / C;
                        c[0, k] = c[0, k] / C;

                        c0 = c0 * this.count[j];
                        a[0] += c[0, k] * this.count[j];
                        u[0] += j * c[0, k] * this.count[j];

                        Ax[0, 0] += 1;
                        Ax[0, 1] += j;
                        Ax[0, 2] += j * j;
                        Ax[1, 2] += j * j * j;
                        Ax[2, 2] += j * j * j * j;

                        b[0] += c0;
                        b[1] += c0 * j;
                        b[2] += c0 * j * j;
                    }

                    Ax[1, 0] = Ax[0, 1];
                    Ax[2, 0] = Ax[0, 2];
                    Ax[1, 1] = Ax[0, 2];
                    Ax[2, 1] = Ax[1, 2];



                    u[0] = u[0] / a[0];

                    for (j = start, k = 0; j <= end; j++, k++)
                    {
                        s[0] += c[0, k] * this.count[j] * Pow(j - u[0], 2);
                    }
                    s[0] = s[0] / a[0];
                    s[0] = Sqrt(s[0]);

                    a[0] = a[0] / IntegrateNorm(u[0], s[0], start, end);
                    if (double.IsNaN(u[0]) || double.IsInfinity(u[0]))
                    {
                        return null;
                    }
                    U[0] = u[0];
                    A[0] = a[0];
                    S[0] = s[0];


                    for (k = 0; k < 3; k++)
                    {
                        for (j = k; j < 3; j++)
                        {
                            Ux[k, j] = Ax[k, j];
                            for (l = 0; l < k; l++)
                                Ux[k, j] -= Lx[k, l] * Ux[l, j];
                        }

                        for (l = k + 1; l < 3; l++)
                        {
                            Lx[l, k] = Ax[l, k];
                            for (j = 0; j < k; j++)
                                Lx[l, k] -= Lx[l, j] * Ux[j, k];
                            Lx[l, k] = Lx[l, k] / Ux[k, k];
                        }

                    }

                    for (l = 0; l < 3; l++)
                    {
                        for (j = 0; j < l; j++)
                            b[l] -= Lx[l, j] * b[j];
                    }

                    for (l = 2; l >= 0; l--)
                    {
                        for (j = l + 1; j < 3; j++)
                            b[l] -= Ux[l, j] * b[j];
                        b[l] = b[l] / Ux[l, l];
                        y[l] = b[l];
                    }
                    c0 = IntegrateX3P(y[0], y[1], y[2], 0, start, end);
                    C = A[0] * IntegrateNorm(U[0], S[0], start, end);

                    if (c0 > (c0 + C) * unnorm)
                    {
                        for (j = 0; j < 3; j++)
                        {
                            y[j] = y[j] / c0 * C * 0.5;
                        }
                    }

                }
                Peak[] ret = new Peak[1];
                ret[0].U = U[0];
                ret[0].S = S[0];
                ret[0].A = A[0];
                return ret;

            }
            else
            {
                U[0] = start;
                U[1] = end;
                S[0] = (end - start) / 6;
                S[1] = S[0];
                A[0] = (1 - c0) / IntegrateNorm(U[0], S[0], start, end) / 6;
                A[1] = A[0];
                for (i = 0; i < Maxtime; i++)
                {
                    for (j = 0; j < n; j++)
                    {
                        a[j] = 0;
                        u[j] = 0;
                        s[j] = 0;
                    }

                    for (j = 0; j < 3; j++)
                    {
                        b[j] = 0;
                        for (k = 0; k < 3; k++)
                        {
                            Ax[j, k] = 0;
                            Lx[j, k] = 0;
                            Ux[j, k] = 0;
                        }
                    }

                    for (j = start, k = 0; j <= end; j++, k++)
                    {
                        c0 = Max(X3(j, y[0], y[1], y[2], 0), 0);
                        c[0, k] = A[0] * Norm(j, U[0], S[0]);
                        c[1, k] = A[1] * Norm(j, U[1], S[1]);
                        C = c0 + c[0, k] + c[1, k];
                        c0 = c0 / C;
                        c[0, k] = c[0, k] / C;
                        c[1, k] = c[1, k] / C;

                        c0 = c0 * this.count[j];
                        a[0] += c[0, k] * this.count[j];
                        u[0] += j * c[0, k] * this.count[j];

                        a[1] += c[1, k] * this.count[j];
                        u[1] += j * c[1, k] * this.count[j];

                        Ax[0, 0] += 1;
                        Ax[0, 1] += j;
                        Ax[0, 2] += j * j;
                        Ax[1, 2] += j * j * j;
                        Ax[2, 2] += j * j * j * j;

                        b[0] += c0;
                        b[1] += c0 * j;
                        b[2] += c0 * j * j;
                    }

                    Ax[1, 0] = Ax[0, 1];
                    Ax[2, 0] = Ax[0, 2];
                    Ax[1, 1] = Ax[0, 2];
                    Ax[2, 1] = Ax[1, 2];



                    u[0] = u[0] / a[0];
                    u[1] = u[1] / a[1];

                    for (j = start, k = 0; j <= end; j++, k++)
                    {
                        s[0] += c[0, k] * this.count[j] * Pow(j - u[0], 2);
                    }
                    s[0] = s[0] / a[0];
                    s[0] = Sqrt(s[0]);
                    for (j = start, k = 0; j <= end; j++, k++)
                    {
                        s[1] += c[1, k] * this.count[j] * Pow(j - u[1], 2);
                    }
                    s[1] = s[1] / a[1];
                    s[1] = Sqrt(s[1]);

                    a[0] = a[0] / IntegrateNorm(u[0], s[0], start, end);
                    if (double.IsNaN(u[0]) || double.IsInfinity(u[0]) || double.IsNaN(u[1]) || double.IsInfinity(u[1]))
                    {
                        return null;
                    }
                    U[0] = u[0];
                    A[0] = a[0];
                    S[0] = s[0];
                    U[1] = u[1];
                    A[1] = a[1];
                    S[1] = s[1];

                    for (k = 0; k < 3; k++)
                    {
                        for (j = k; j < 3; j++)
                        {
                            Ux[k, j] = Ax[k, j];
                            for (l = 0; l < k; l++)
                                Ux[k, j] -= Lx[k, l] * Ux[l, j];
                        }

                        for (l = k + 1; l < 3; l++)
                        {
                            Lx[l, k] = Ax[l, k];
                            for (j = 0; j < k; j++)
                                Lx[l, k] -= Lx[l, j] * Ux[j, k];
                            Lx[l, k] = Lx[l, k] / Ux[k, k];
                        }

                    }

                    for (l = 0; l < 3; l++)
                    {
                        for (j = 0; j < l; j++)
                            b[l] -= Lx[l, j] * b[j];
                    }

                    for (l = 2; l >= 0; l--)
                    {
                        for (j = l + 1; j < 3; j++)
                            b[l] -= Ux[l, j] * b[j];
                        b[l] = b[l] / Ux[l, l];
                        y[l] = b[l];
                    }
                    c0 = IntegrateX3P(y[0], y[1], y[2], 0, start, end);
                    C = A[0] * IntegrateNorm(U[0], S[0], start, end);

                    if (c0 > (c0 + C) * unnorm)
                    {
                        for (j = 0; j < 3; j++)
                        {
                            y[j] = y[j] / c0 * C * 0.5;
                        }
                    }

                }
                Peak[] ret = new Peak[2];
                ret[0].U = U[0];
                ret[0].S = S[0];
                ret[0].A = A[0];
                ret[1].U = U[1];
                ret[1].S = S[1];
                ret[1].A = A[1];
                return ret;
            }




        }





        private static double IntegrateNorm(double U, double S, int start, int end)
        {
            double ret = 0;
            while (start <= end)
            {
                ret += Norm(start, U, S);
                start++;
            }
            return ret;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="U"></param>
        /// <param name="S">
        /// S refers to Sigma, NOT Sigma^2
        /// </param>
        /// <returns></returns>
        private static double Norm(double x, double U, double S)
        {
            double ret = Exp(-Pow(x - U, 2) / (double)(2 * S * S)) / S / Sqrt(2 * PI);
            return ret;
        }

        private static double IntegrateX3P(double a0, double a1, double a2, double a3, double start, double end)
        {
            double ret = 0;
            while (start <= end)
            {
                ret += Max(X3(start, a0, a1, a2, a3), 0);
                start++;
            }
            return ret;
        }

        private static double X3(double x, double a0, double a1, double a2, double a3)
        {
            return a0 + a1 * x + a2 * x * x + a3 * x * x * x;
        }

        /// <summary>
        /// with External Random for High Concurrency
        /// </summary>
        /// <param name="u"></param>
        /// <param name="s"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        private double GaussRand(double u, double s, Random r)
        {
            double r1 = 1 - r.NextDouble();
            double r2 = 1 - r.NextDouble();
            double R = Sqrt(-2 * Log(r1));
            double Sita = 2 * PI * r2;
            double Z = R * Sin(Sita);
            Z = u + Z * s;
            return Z;
        }

        private double GaussRand(double u, double s)
        {
            Random r = new Random();
            return GaussRand(u, s, r);
        }

        private struct partial
        {
            public double[,] param;
            public double[,] speed;
            public double[,] bestparam;
            public double best;
        }


        public Peak[] Partial(int n, int start, int end, int Maxtime, int np)
        {
            if ((n != 1 && n != 2 && n != 3) || np < 10 || Maxtime < 10 || start >= end)
                return null;

            partial[] p = new partial[np];
            int gbest;

            int i, j, k, l;
            double s, max, min, best;
            double[] parammax, parammin;


            Smooth((end - start) / 6, -1);

            parammax = new double[2];
            parammin = new double[2];

            max = 0;
            for (l = start; l <= end; l++)
            {
                if (this.count[l] > max)
                    max = this.count[l];
            }
            parammax[1] = max;
            parammin[1] = 0;
            parammax[0] = this.fix[end] / (end - start) * 1.1;
            parammin[0] = -this.fix[start] / (end - start) * 1.1;


            for (i = 0; i < np; i++)
            {
                p[i].param = new double[n + 1, 3];
                p[i].bestparam = new double[n + 1, 3];
                p[i].speed = new double[n + 1, 3];
                Random r1 = new Random(int.Parse(DateTime.Now.ToString("HHmmssfff")) + i);

                for (j = 1; j <= n; j++)
                {
                    for (k = 0; k < 3; k++)
                    {
                        Random r = new Random(r1.Next());
                        if (k == 0)
                        {
                            p[i].param[j, 2] = r.NextDouble() * (end - start) / 2.0;
                        }
                        else if (k == 1)
                        {
                            p[i].param[j, 1] = r.NextDouble() * (end - start) + start;
                        }
                        else
                        {
                            p[i].param[j, 0] = r.NextDouble() * parammax[1] / Norm(p[i].param[j, 1], p[i].param[j, 1], p[i].param[j, 2]);
                        }

                    }
                }
                p[i].param[0, 2] = 0;
                p[i].param[0, 1] = r1.NextDouble() * (parammax[0] - parammin[0]) + parammin[0];

                if (p[i].param[0, 1] >= 0)
                {
                    min = -start * p[i].param[0, 1];
                    max = 1.1 * this.fix[end] - end * p[i].param[0, 1];
                }
                else
                {
                    min = -end * p[i].param[0, 1];
                    max = 1.1 * this.fix[start] - start * p[i].param[0, 1];
                }
                p[i].param[0, 0] = r1.NextDouble() * (max - min) + min;
            }

            best = adaptivity(p[0], n, start, end);
            gbest = 0;

            for (i = 0; i < np; i++)
            {
                s = adaptivity(p[i], n, start, end);
                if (s >= 0)
                    p[i].best = s;
                else
                    return null;
                if (s <= best)
                {
                    gbest = i;
                    best = s;
                }

                for (j = 1; j <= n; j++)
                {
                    for (k = 0; k < 3; k++)
                    {
                        p[i].bestparam[j, k] = p[i].param[j, k];
                        p[i].speed[j, k] = 0;
                    }
                }

            }

            double w = 2;
            double c1 = 1;
            double c2 = 1;
            double wp = 1.7 / Maxtime;

            double t = 1000000;
            double c = 0.8;

            for (l = 0; l < Maxtime; l++)
            {
                for (i = 0; i < np; i++)
                {
                    Random r = new Random(int.Parse(DateTime.Now.ToString("HHmmssfff")) + i);
                    for (j = 1; j <= n; j++)
                    {
                        for (k = 0; k < 3; k++)
                        {
                            Random r1 = new Random(r.Next());
                            if (k == 0)
                            {
                                p[i].speed[j, 2] = w * p[i].speed[j, 2] + c1 * (p[i].bestparam[j, 2] - p[i].param[j, 2]) * r1.NextDouble() + c2 * (p[gbest].bestparam[j, 2] - p[i].param[j, 2]) * r1.NextDouble();
                                p[i].param[j, 2] += p[i].speed[j, 2];
                                if (p[i].param[j, 2] > (end - start) / 2.0)
                                {
                                    p[i].param[j, 2] = (end - start) / 2.0;
                                    p[i].speed[j, 2] = 0;
                                }

                            }
                            else if (k == 1)
                            {
                                p[i].speed[j, 1] = w * p[i].speed[j, 1] + c1 * (p[i].bestparam[j, 1] - p[i].param[j, 1]) * r1.NextDouble() + c2 * (p[gbest].bestparam[j, 1] - p[i].param[j, 1]) * r1.NextDouble();
                                p[i].param[j, 1] += p[i].speed[j, 1];
                                if (p[i].param[j, 1] > end)
                                {
                                    p[i].param[j, 1] = end;
                                    p[i].speed[j, 1] = 0;
                                }
                                else if (p[i].param[j, 1] < start)
                                {
                                    p[i].param[j, 1] = start;
                                    p[i].speed[j, 1] = 0;
                                }
                            }
                            else
                            {
                                p[i].speed[j, 0] = w * p[i].speed[j, 0] + c1 * (p[i].bestparam[j, 0] - p[i].param[j, 0]) * r1.NextDouble() + c2 * (p[gbest].bestparam[j, 0] - p[i].param[j, 0]) * r1.NextDouble();
                                p[i].param[j, 0] += p[i].speed[j, 0];
                                if (p[i].param[j, 0] > parammax[1] / Norm(p[i].param[j, 1], p[i].param[j, 1], p[i].param[j, 2]))
                                {
                                    p[i].param[j, 0] = parammax[1] / Norm(p[i].param[j, 1], p[i].param[j, 1], p[i].param[j, 2]);
                                    p[i].speed[j, 0] = 0;
                                }
                            }
                        }

                    }
                    p[i].speed[0, 1] = w * p[i].speed[0, 1] + c1 * (p[i].bestparam[0, 1] - p[i].param[0, 1]) * r.NextDouble() + c2 * (p[gbest].bestparam[0, 1] - p[i].param[0, 1]) * r.NextDouble();
                    p[i].param[0, 1] += p[i].speed[0, 1];
                    if (p[i].param[0, 1] > parammax[0])
                    {
                        p[i].param[0, 1] = parammax[0];
                        p[i].speed[0, 1] = 0;
                    }
                    else if (p[i].param[0, 1] < parammin[0])
                    {
                        p[i].param[0, 1] = parammin[0];
                        p[i].speed[0, 1] = 0;
                    }
                    p[i].speed[0, 0] = w * p[i].speed[0, 0] + c1 * (p[i].bestparam[0, 0] - p[i].param[0, 0]) * r.NextDouble() + c2 * (p[gbest].bestparam[0, 0] - p[i].param[0, 0]) * r.NextDouble();
                    p[i].param[0, 0] += p[i].speed[0, 0];

                    if (p[i].param[0, 1] >= 0)
                    {
                        min = -start * p[i].param[0, 1];
                        max = 1.1 * this.fix[end] - end * p[i].param[0, 1];
                    }
                    else
                    {
                        min = -end * p[i].param[0, 1];
                        max = 1.1 * this.fix[start] - start * p[i].param[0, 1];
                    }

                    if (p[i].param[0, 0] > max)
                    {
                        p[i].param[0, 0] = max;
                        p[i].speed[0, 0] = 0;
                    }
                    else if (p[i].param[0, 0] < min)
                    {
                        p[i].param[0, 0] = min;
                        p[i].speed[0, 0] = 0;
                    }

                    s = adaptivity(p[i], n, start, end);
                    if (s < 0)
                        return null;
                    else if (s < p[i].best)
                    {
                        p[i].best = s;
                        for (j = 1; j <= n; j++)
                        {
                            for (k = 0; k < 3; k++)
                                p[i].bestparam[j, k] = p[i].param[j, k];
                        }
                    }

                    if (s < best)
                    {
                        gbest = i;
                        best = s;
                    }
                }

                //SA_PSO
                Random R = new Random();
                for (i = 0; i < np / 2; i++)
                {
                    int p1 = R.Next(0, np);
                    int p2 = R.Next(0, np);
                    Random r1 = new Random();
                    partial P1, P2;

                    P1.param = new double[n + 1, 3];
                    P1.bestparam = new double[n + 1, 3];
                    P1.speed = new double[n + 1, 3];
                    P1.best = 0;
                    P2.param = new double[n + 1, 3];
                    P2.bestparam = new double[n + 1, 3];
                    P2.speed = new double[n + 1, 3];
                    P2.best = 0;
                    double pr = r1.NextDouble();
                    for (j = 0; j <= n; j++)
                        for (k = 0; k < 3; k++)
                        {
                            P1.param[j, k] = pr * p[p1].param[j, k] + (1 - pr) * p[p2].param[j, k];
                            P2.param[j, k] = pr * p[p2].param[j, k] + (1 - pr) * p[p1].param[j, k];
                        }
                    double S1 = adaptivity(P1, n, start, end);
                    double S2 = adaptivity(P2, n, start, end);
                    double s1 = adaptivity(p[p1], n, start, end);
                    double s2 = adaptivity(p[p2], n, start, end);

                    bool flags1 = false;
                    if (Exp((s1 - S1) / t) > pr)
                    {
                        flags1 = true;
                        p[p1].param = P1.param;
                        if (S1 < p[p1].best)
                        {
                            p[p1].best = S1;
                            for (j = 0; j <= n; j++)
                                for (k = 0; k < 3; k++)
                                    p[p1].bestparam[j, k] = P1.param[j, k];
                            if (S1 < p[gbest].best)
                                gbest = p1;
                        }
                        double vabs = 0;
                        for (j = 0; j <= n; j++)
                            for (k = 0; k < 3; k++)
                            {
                                p[p1].speed[j, k] += p[p2].speed[j, k];
                                vabs += Abs(p[p1].speed[j, k]);
                            }
                        for (j = 0; j <= n; j++)
                            for (k = 0; k < 3; k++)
                                p[p1].speed[j, k] = p[p1].speed[j, k] / vabs;

                    }

                    if (Exp((s2 - S2) / t) > pr)
                    {
                        p[p2].param = P2.param;
                        if (S2 < p[p2].best)
                        {
                            p[p2].best = S2;
                            for (j = 0; j <= n; j++)
                                for (k = 0; k < 3; k++)
                                    p[p2].bestparam[j, k] = P2.param[j, k];
                            if (S2 < p[gbest].best)
                                gbest = p2;
                        }

                        if (flags1)
                        {
                            for (j = 0; j <= n; j++)
                                for (k = 0; k < 3; k++)
                                    p[p2].speed[j, k] = p[p1].speed[j, k];
                        }
                        else
                        {
                            double vabs = 0;
                            for (j = 0; j <= n; j++)
                                for (k = 0; k < 3; k++)
                                {
                                    p[p2].speed[j, k] += p[p1].speed[j, k];
                                    vabs += Abs(p[p2].speed[j, k]);
                                }
                            for (j = 0; j <= n; j++)
                                for (k = 0; k < 3; k++)
                                    p[p2].speed[j, k] = p[p2].speed[j, k] / vabs;
                        }


                    }

                }
                for (i = 0; i < np / 20; i++)
                {
                    partial P1;
                    P1.param = new double[n + 1, 3];
                    P1.bestparam = new double[n + 1, 3];
                    P1.speed = new double[n + 1, 3];
                    P1.best = 0;
                    double G = GaussRand(1, 0.4, R);
                    int p1 = R.Next(0, np);
                    for (j = 0; j <= n; j++)
                        for (k = 0; k < 3; k++)
                        {
                            P1.param[j, k] = p[p1].param[j, k] * G;
                        }
                    double S1 = adaptivity(P1, n, start, end);
                    double s1 = adaptivity(p[p1], n, start, end);
                    double pr = R.NextDouble();
                    if (Exp((s1 - S1) / t) > pr)
                    {
                        p[p1].param = P1.param;
                        if (S1 < p[p1].best)
                        {
                            p[p1].best = S1;
                            for (j = 0; j <= n; j++)
                                for (k = 0; k < 3; k++)
                                    p[p1].bestparam[j, k] = P1.param[j, k];
                            if (S1 < p[gbest].best)
                                gbest = p1;
                        }

                    }

                }
                t = t * c;

                //SA_PSO



                w -= wp;
            }
            Peak[] ret = new Peak[n];
            for(i=0;i<n;i++)
            {
                ret[i].U = p[gbest].bestparam[i + 1, 1];
                ret[i].S = p[gbest].bestparam[i + 1, 2];
                ret[i].A = p[gbest].bestparam[i + 1, 0];
            }
            return ret;

        }

        private double adaptivity(partial p, int n, int start, int end)
        {
            int i, j;
            double s, r;
            r = 0;
            try
            {
                for (i = start; i <= end; i++)
                {
                    s = 0;
                    for (j = 1; j <= n; j++)
                        s += p.param[j, 0] * Norm(i, p.param[j, 1], p.param[j, 2]);
                    s += X3(i, p.param[0, 0], p.param[0, 1], p.param[0, 2], 0);
                    r += Pow(s - this.count[i], 2);
                }
            }
            catch (Exception)
            {
                return -1;
            }
            return r;
        }

        public Peak[] LevenbergMarquardt(int n, int start, int end, int Maxtime, int np, double e1, double e2)
        {
            if (n < 1 || end <= start || np < 1 || e1 < 0 || e2 < 0 || Maxtime < 1)
                return null;
            bool flag = false;
            int nb = 3 * n + 2, nc = end - start + 1;
            double v = 2;

            Matrix b = new Matrix(nb, 1);
            Matrix bnew = new Matrix(nb, 1);
            Matrix h = new Matrix(nb, 1);
            Matrix J = new Matrix(nc, nb);
            Matrix JT = J.Transpose();
            Matrix A = new Matrix(nb, nb);
            Matrix Inb = new Matrix(nb, nb);
            Matrix g = new Matrix(nb, 1);
            Matrix f = new Matrix(nc, 1);
            Matrix L;
            double u = (double)nb, p, F, Fnew;

            int i, j, k, l;

            Smooth(end - start / 2 / n, -1);
            for (i = 0; i < nb; i++)
                for (j = 0; j < nb; j++)
                {
                    if (i == j)
                        Inb[i, j] = 1;
                    else
                        Inb[i, j] = 0;
                }


            j = 0;
            for (i = start; i <= end; i++)
            {
                j += this.count[i];
            }


            for (i = 0; i < n; i++)
            {
                b[3 * i + 1, 0] = start + i * (end - start) / Math.Max(1, n - 1);
                b[3 * i + 2, 0] = (end - start) / 2 / n;
                b[3 * i, 0] = j / n;
            }
            b[3 * n + 1, 0] = (this.fix[end] - this.fix[start]) / (end - start);
            b[3 * n, 0] = this.fix[start] - b[3 * n + 1, 0] * start;


            for (i = 0; i < Maxtime; i++)
            {
                for (j = 0; j < nc; j++)
                {
                    int x = start + j;
                    f[j, 0] = -count[x];
                    for (k = 0; k < n; k++)
                    {
                        J[j, 3 * k] = Norm(x, b[3 * k + 1, 0], b[3 * k + 2, 0]);
                        J[j, 3 * k + 1] = b[3 * k, 0] * J[j, 3 * k] * (x - b[3 * k + 1, 0]) / Pow(b[3 * k + 2, 0], 2);
                        J[j, 3 * k + 2] = -b[3 * k, 0] * J[j, 3 * k] / b[3 * k + 2, 0] + J[j, 3 * k + 1] * (x - b[3 * k + 1, 0]) / b[3 * k + 2, 0];
                        f[j, 0] += b[3 * k, 0] * J[j, 3 * k];
                    }
                    J[j, 3 * n] = 1;
                    J[j, 3 * n + 1] = x;
                    f[j, 0] += x * b[3 * n + 1, 0] + b[3 * n, 0];

                }
                JT = J.Transpose();
                f = -f;
            Loop: A = JT * J;
                g = JT * f;
                if (g.InfinityNorm() < e1)
                {
                    flag = true;
                    break;
                }
                A = A + u * Inb;
                h = A.Inverse() * g;
                if (h.InfinityNorm() < e2 * (b.InfinityNorm() + e2))
                {
                    flag = true;
                    break;
                }
                bnew = b + h;
                F = 0;
                Fnew = 0;
                for (j = 0; j < nc; j++)
                {
                    int x = start + j;
                    F += f[j, 0];
                    for (k = 0; k < n; k++)
                    {
                        Fnew += bnew[3 * k, 0] * Norm(x, bnew[3 * k + 1, 0], bnew[3 * k + 2, 0]);
                    }
                    Fnew += x * bnew[3 * n + 1, 0] + bnew[3 * n, 0];
                }
                p = F - Fnew;
                L = -h.Transpose() * JT * f - 0.5 * h.Transpose() * JT * J * h;
                p = p / L[0, 0];
                if (p > 0)
                {
                    b = bnew;
                    u = Math.Max(1 / 3.0, 1 - Pow(2 * p - 1, 3));
                    v = 2;
                }
                else
                {
                    u = u * v;
                    v = 2 * v;
                    goto Loop;
                }


            }

            Peak[] ret = new Peak[n];
            for (i = 0; i < n; i++)
            {
                ret[i].U = b[3 * i + 1, 0];
                ret[i].S = b[3 * i + 2, 0];
                ret[i].A = b[3 * i, 0];
            }
            return ret;


        }
        public void Peakfind(int PeakFindMode,int ROIStart,int ROIEnd, ListView listView1)
        {
            Peak[] ret = null;
            if (PeakFindMode == 0)
            {
                this.Smooth((ROIEnd - ROIStart) / 6, -1);
                while (this.Maxium(0, ROIStart, ROIEnd) > 1)
                    this.Smooth((ROIEnd - ROIStart) / 6, 1);

                ret = new Peak[1];
                ret[0].U = this.Maxium(1, ROIStart, ROIEnd);
                int i = (int)ret[0].U;
                int j = i + 1;
                int m = (int)(ret[0].U + 0.5);
                double s = 0;
                while (this.GetNumber(1, i - 1) - this.GetNumber(1, i - 2) > this.GetNumber(1, i) - this.GetNumber(1, i - 1) && i >= 0)
                {
                    s += this.GetNumber(0, i);
                    i--;
                }
                while (this.GetNumber(1, j + 1) - this.GetNumber(1, j + 2) > this.GetNumber(1, j) - this.GetNumber(1, j + 1) && j < this.TotalChannel)
                {
                    s += this.GetNumber(0, j);
                    j++;
                }
                m = j - i;
                ret[0].S = m;
                ret[0].A = s;

            }
            else if (PeakFindMode == 1)
            {
                ret = this.ExpectMax(1, ROIStart, ROIEnd);
            }
            else if (PeakFindMode == 2)
            {
                ret = this.Partial(1, ROIStart, ROIEnd, 200, 1000);
            }
            else if (PeakFindMode == 3)
            {
                pf3 = new Peakfind3(this.LevenbergMarquardt);
                pf3.BeginInvoke(3, ROIStart, ROIEnd, 1000, 1, 0, 0, new AsyncCallback(Peakfind3Finished), null);
            }
            
        }
        delegate Peak[] Peakfind3(int n, int start, int end, int Maxtime, int np, double e1, double e2);
        static Peakfind3 pf3;
        public delegate void ListViewUpdate(Peak[] ret);
        public static ListViewUpdate listviewup;
        static void Peakfind3Finished(IAsyncResult result)
        {
            Peak[] ret = pf3.EndInvoke(result);
            if (ret == null)
                return;
            listviewup.Invoke(ret);
        }
    }
}


