using System;

namespace Lab2
{
    public class MyMatrix
    {
        public static int MaxRandValue = 10;
        protected int n;
        public int Size
        {
            get { return n; }
        }
        public double[,] matrix;

        public MyMatrix(int n)
        {
            if (n < 0) throw new Exception("Invalid size");
            this.n = n;
            matrix = new double[n, n];
        }

        public double this[int i, int j]
        {
            get
            {
                return matrix[i, j];
            }
            protected set
            {
                matrix[i, j] = value;
            }
        }

        public MyMatrix GetTransposed()
        {
            MyMatrix result = new MyMatrix(n);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    result.matrix[i, j] = matrix[j, i];
                }
            }
            return result;
        }

        public void HandInit(string message) {
            //reads one line in a row like
            //1 2 3
            //4 5 6

            Console.WriteLine("Enter {0}", message);
            for (int i = 0; i < n; i++)
            {
                var values = (Console.ReadLine().Split(' '));
                for (int j = 0; j < n; j++)
                {
                    matrix[i, j] = int.Parse(values[j]);
                }
            }
        }

        public void RandomInitMatrix()
        {
            Random rnd = new Random();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matrix[i, j] = rnd.Next(1, MaxRandValue);
                }
            }
        }

        public void RandomInitVector()
        {
            Random rnd = new Random();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (j == 0)
                    {
                        matrix[i, j] = rnd.Next(0, MaxRandValue);
                    }
                    else
                    {
                        matrix[i, j] = 0;
                    }

                }
            }
        }

        public void ShowMatrix(string message) {
            Console.WriteLine("Matrix: {0}", message);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(string.Format("{0:N2}   ", matrix[i, j]));
                }
                Console.WriteLine();
            }
        }

        public void Create_C2()
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matrix[i, j] = 1.0 / (((i + 1) + (j + 1)) * 2.0);
                }
            }
        }

        public void Create_bi()
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (j == 0)
                    {
                        matrix[i, j] = (((i + 1) % 2) == 0) ? (3.0 / (Math.Pow(i + 1, 2) + 3.0)) : (3.0 / (i + 1));
                    }
                    else
                    {
                        matrix[i, j] = 0;
                    }

                }
            }
        }

        public static MyMatrix operator +(MyMatrix m1, MyMatrix m2)
        {
            if (m1.n != m2.n) return new MyMatrix(2);

            MyMatrix result = new MyMatrix(m1.n);

            for (int i = 0; i < m1.n; i++)
            {
                for (int j = 0; j < m1.n; j++)
                {
                    result[i, j] = m1[i, j] + m2[i, j];
                }
            }

            return result;
        }

        public static MyMatrix operator -(MyMatrix m1, MyMatrix m2)
        {
            if (m1.n != m2.n) return new MyMatrix(2);

            MyMatrix result = new MyMatrix(m1.n);

            for (int i = 0; i < m1.n; i++)
            {
                for (int j = 0; j < m1.n; j++)
                {
                    result[i, j] = m1[i, j] - m2[i, j];
                }
            }

            return result;
        }

        public static MyMatrix operator *(MyMatrix m1, MyMatrix m2)
        {
            if (m1.n != m2.n) return new MyMatrix(2);

            MyMatrix result = new MyMatrix(m1.n);

            for (int i = 0; i < m1.n; i++)
            {
                for (int j = 0; j < m1.n; j++)
                {
                    double tmp = 0;
                    for (int k = 0; k < m1.n; k++)
                    {
                        tmp += m1[i, k] * m2[k, j];
                    }
                    result[i, j] = tmp;
                }
            }
            return result;
        }

        public static MyMatrix operator *(MyMatrix m1, double value)
        {
            MyMatrix result = new MyMatrix(m1.n);

            for (int i = 0; i < m1.n; i++)
            {
                for (int j = 0; j < m1.n; j++)
                {
                    result[i, j] = m1[i, j] * value;
                    
                }
            }
            return result;
        }
    }
}
