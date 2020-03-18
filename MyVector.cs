using System;
using System.Collections.Generic;
using System.Text;

namespace Lab2
{
    class MyVector : MyMatrix
    {
        public MyVector(int size, bool randomInitialised = true) : base(size, randomInitialised) { }

        public override void RandomInit()
        {
            Random rnd = new Random();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
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
        public void Create_bi()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (j == 0)
                    {
                        matrix[i, j] = (j + 1 % 2 == 0) ? (3 / (Math.Pow(j + 1, 2) + 3)) : (3 / (j + 1));
                    }
                    else
                    {
                        matrix[i, j] = 0;
                    }

                }
            }
        }

        public static MyVector operator +(MyVector a, MyVector b)
        {
            MyVector result = new MyVector(a.size);

            for (int i = 0; i < a.size; i++)
            {
                for (int j = 0; j < a.size; j++)
                {
                    result[i, j] = a[i, j] + b[i, j];
                }
            }

            return result;
        }

        public static MyVector operator -(MyVector a, MyVector b)
        {
            MyVector result = new MyVector(a.size);

            for (int i = 0; i < a.size; i++)
            {
                for (int j = 0; j < a.size; j++)
                {
                    result[i, j] = a[i, j] - b[i, j];
                }
            }

            return result;
        }

        public static MyVector operator *(MyVector a, int value)
        {
            MyVector result = new MyVector(a.size);

            for (int i = 0; i < a.size; i++)
            {
                for (int j = 0; j < a.size; j++)
                {
                    result[i, j] = a[i, j] * value;
                }
            }

            return result;
        }
    }
}
