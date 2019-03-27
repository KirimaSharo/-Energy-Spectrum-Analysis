using System;
using System.IO;
using static System.Math;
using OpenGL;
using System.Windows.Forms;

public class Counts
{
    private int totalchannel = 0;
    private int[] count;
    private double[] fix;
    private int[] sim;
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
        if(Path.GetExtension(filepath)==".dat")
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
    /// 1:count;
    /// 2:fix;
    /// 3:sim;
    /// </param>
    public double GetNumber(byte source,int i)
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
        for (int i=0;i<this.totalchannel;i++)
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
        catch(Exception)
        {
            return false;
        }
         return true;
    }

    /// <param name="t">
    ///  time of smooth
    ///  -1 for Reset Fix
    /// </param>
    public void Smooth(int n,int t)
    {
        int n1 = (n + 1) / 2;
        if (this.totalchannel == 0)
            return;
        int i, j, k;
        double y;
        double[] f = new double[this.TotalChannel];
        double[] x = new double[n];
        if(this.fix==null)
        {
            this.fix = new double[this.TotalChannel];
            for (i = 0; i < this.TotalChannel; i++)
            {
                this.fix[i] = this.count[i];
            }
        }
        if(t == -1)
        {
            for (i = 0; i < this.TotalChannel; i++)
            {
                this.fix[i] = this.count[i];
            }
            t = 1;
        }

        for(i=0;i<n1;i++)
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

    public int Maxium(int i,int start,int end)
    {
        if (this.fix == null)
            Smooth((end - start) / 3, 1);

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
    public double ExpectMax(int n, int start, int end)
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
    public double ExpectMax(int n,int start,int end,int Maxtime)
    {
        if (n != 1 && n != 2)
            return -1;

        int i, j, k, l;
        double[,] Ax = new double[4, 4];
        double[] b = new double[4];

        double[] y = new double[4];
        
        for (i = 1; i < 4; i++)
            y[i] = 0;

        double[,] Lx = new double[4, 4];
        double[,] Ux = new double[4, 4];

        double[,] c = new double[n, end - start + 1];
        double c0,C;
        

        double[] A = new double[n];
        double[] U = new double[n];
        double[] S = new double[n];

        double[] a = new double[n];
        double[] u = new double[n];
        double[] s = new double[n];

        y[0] = 0.1 / (end - start + 1);
        if (n == 1)
        {
            U[0] = (end + start) / 2;
            S[0] = (end - start) / 2;
            A[0] = 0.9/IntegrateNorm(U[0], S[0], start, end);

            for (i = 0; i < Maxtime; i++) 
            {
                a[0] = 0;
                u[0] = 0;
                s[0] = 0;
                for (j = 0; j < 4; j++) 
                {
                    b[j] = 0;
                    for (k = 0; k < 4; k++) 
                    {
                        Ax[j, k] = 0;
                        Lx[j, k] = 0;
                        Ux[j, k] = 0;
                    }
                }

                for (j = start, k = 0; j <= end; j++, k++) 
                {
                    c0 = Max(X3(j, y[0], y[1], y[2], y[3]),0);
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
                    Ax[0, 3] += j * j * j;
                    Ax[1, 3] += j * j * j * j;
                    Ax[2, 3] += j * j * j * j * j;
                    Ax[3, 3] += j * j * j * j * j * j;
                    b[0] += c0;
                    b[1] += c0 * j;
                    b[2] += c0 * j * j;
                    b[3] += c0 * j * j * j;
                }

                Ax[1, 0] = Ax[0, 1];
                Ax[2, 0] = Ax[0, 2];
                Ax[1, 1] = Ax[0, 2];
                Ax[3, 0] = Ax[0, 3];
                Ax[2, 1] = Ax[0, 3];
                Ax[1, 2] = Ax[0, 3];
                Ax[3, 1] = Ax[1, 3];
                Ax[2, 2] = Ax[1, 3];
                Ax[3, 2] = Ax[2, 3];
                

                u[0] = u[0] / a[0];

                for (j = start, k = 0; j <= end; j++, k++)
                {
                    s[0] += c[0, k] * this.count[j] * Pow(j - u[0], 2);
                }
                s[0] = s[0] / a[0];
                s[0] = Sqrt(s[0]);

                a[0] = a[0] / IntegrateNorm(u[0], s[0], start, end);
                if(double.IsNaN(u[0])||double.IsInfinity(u[0]))
                {
                    return -1;
                }
                U[0] = u[0];
                A[0] = a[0];
                S[0] = s[0];


                for (k = 0; k < 4; k++)
                {
                    for (j = k; j < 4; j++)
                    {
                        Ux[k, j] = Ax[k, j];
                        for (l = 0; l < k; l++)
                            Ux[k, j] -= Lx[k, l] * Ux[l, j];
                    }

                    for (l = k + 1; l < 4; l++) 
                    {
                        Lx[l, k] = Ax[l, k];
                        for (j = 0; j < k; j++)
                            Lx[l, k] -= Lx[l, j] * Ux[j, k];
                        Lx[l, k] = Lx[l, k] / Ux[k, k];
                    }
                    
                }

                for (l = 0; l < 4; l++) 
                {
                    for (j = 0; j < l; j++)
                        b[l] -= Lx[l, j] * b[j];
                }

                for (l = 3; l >= 0; l--)
                {
                    for (j = l + 1; j < 4; j++)
                        b[l] -= Ux[l, j] * b[j];
                    b[l] = b[l] / Ux[l, l];
                    y[l] = b[l];
                }
                c0 = IntegrateX3P(y[0], y[1], y[2], y[3], start, end);
                C = A[0] * IntegrateNorm(U[0], S[0], start, end);

                if (c0 > 0.5 * C) 
                {
                    for (j = 0; j < 4; j++)
                    {
                        y[j] = y[j] / c0 * C * 0.5;
                    }
                }
            
            }

            return U[0];

        }
        else
        {
            U[0] = start;
            U[1] = end;
            S[0] = (end - start) / 2;
            S[1] = S[0];
            A[0] = 0.45 / IntegrateNorm(U[0], S[0], start, end);
            A[1] = A[0];

            return U[0];
        }




    }





    private static double IntegrateNorm(double U, double S,int start,int end)
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
    private static double Norm(double x,double U,double S)
    {
        double ret= Exp(-Pow(x - U, 2) / (double)(2 * S * S)) /S/Sqrt(2*PI);
        return ret;
    }

    private static double IntegrateX3P(double a0, double a1, double a2, double a3, double start, double end)
    {
        double ret = 0;
        while (start <= end)
        {
            ret += Max(X3(start, a0, a1, a2, a3),0);
            start++;
        }
        return ret;
    }

    private static double X3(double x,double a0, double a1,double a2,double a3)
    {
        return a0 + a1 * x + a2 * x * x + a3 * x * x * x;
    }




}


