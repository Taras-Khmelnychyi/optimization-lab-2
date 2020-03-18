using System;
using System.Collections.Generic;
using System.Text;

namespace Lab2
{
    public class MyMatrix
    {
        public static int MaxRandValue = 10;
        protected int size;
        public int Size
        {
            get { return size; }
        }
        public double[,] matrix;

        public MyMatrix(int size, bool randomInitialised = true)
        {
            if (size < 0) throw new Exception("Invalid size");
            this.size = size;
            matrix = new double[size, size];
            if (randomInitialised) RandomInit();
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
            MyMatrix result = new MyMatrix(size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    result.matrix[i, j] = matrix[j, i];
                }
            }
            return result;
        }

        public virtual void RandomInit()
        {
            Random rnd = new Random();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = rnd.Next(1, MaxRandValue);
                }
            }
        }

        public void Create_C2()
        {

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = 1 / (((i + 1) + (j + 1)) * 2);
                }
            }
        }

        public static MyMatrix operator +(MyMatrix m1, MyMatrix m2)
        {
            if (m1.size != m2.size) return new MyMatrix(2);

            MyMatrix result = new MyMatrix(m1.size);

            for (int i = 0; i < m1.size; i++)
            {
                for (int j = 0; j < m1.size; j++)
                {
                    result[i, j] = m1[i, j] + m2[i, j];
                }
            }

            return result;
        }

        public static MyMatrix operator -(MyMatrix m1, MyMatrix m2)
        {
            if (m1.size != m2.size) return new MyMatrix(2);

            MyMatrix result = new MyMatrix(m1.size);

            for (int i = 0; i < m1.size; i++)
            {
                for (int j = 0; j < m1.size; j++)
                {
                    result[i, j] = m1[i, j] - m2[i, j];
                }
            }

            return result;
        }

        public static MyMatrix operator *(MyMatrix m1, MyMatrix m2)
        {
            if (m1.size != m2.size) return new MyMatrix(2);

            MyMatrix result = new MyMatrix(m1.size);

            for (int i = 0; i < m1.size; i++)
            {
                for (int j = 0; j < m1.size; j++)
                {
                    double tmp = 0;
                    for (int k = 0; k < m1.size; k++)
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
            MyMatrix result = new MyMatrix(m1.size);

            for (int i = 0; i < m1.size; i++)
            {
                for (int j = 0; j < m1.size; j++)
                {
                    double tmp = 0;
                    for (int k = 0; k < m1.size; k++)
                    {
                        tmp += m1[i, k] * value;
                    }
                    result[i, j] = tmp;
                }
            }
            return result;
        }
    }
}
