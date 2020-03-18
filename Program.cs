using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using Lab2;

namespace Lab2
{
    class Program
    {

        public static void PrintMatrix(MyMatrix m1, string s)
        {
            Console.WriteLine("Matrix: {0}", s);
            for (int i = 0; i < m1.Size; i++)
            {
                for (int j = 0; j < m1.Size; j++)
                {
                    Console.Write(string.Format("{0} ", m1[i, j]));
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
        }
        static void Main()
        {
            MainProcessing app = new MainProcessing(4);
            app.CompleteProcess();
            //PrintMatrix(app.A1, "A1");
            //PrintMatrix(app.b1, "b1");
            //PrintMatrix(app.c1, "c1");
            //PrintMatrix(app.A2, "A2");
            //PrintMatrix(app.B2, "B2");
            //PrintMatrix(app.C2, "C2");
            //PrintMatrix(app.A, "A");
            //PrintMatrix(app.bi, "bi");

            //PrintMatrix(app.y1, "y1");
            //PrintMatrix(app.y2, "y2");
            //PrintMatrix(app.Y3, "Y3");

            //PrintMatrix(app.Y3squared, "Y3squared");
            //PrintMatrix(app.Y3cubed, "Y3cubed");


            PrintMatrix(app.result, "result");
        }
    }


    class MainProcessing
    {
        int n;
        
        public MyMatrix A1, A2, B2, C2, A;
        public MyVector b1, c1, bi;

        public double K1 = 1;
        public double K2 = 50;

        public MyMatrix Y3, y1, y2;
        public MyMatrix Y3squared, Y3cubed;
        public MyMatrix result;

        public MainProcessing(int n)
        {
            this.n = n;
        }

        public void CompleteProcess() {

            //init stage
            var A1 = Task<MyMatrix>.Factory.StartNew(() => { return new MyMatrix(n); });
            this.A1 = A1.Result;
            var A2 = Task<MyMatrix>.Factory.StartNew(() => { return new MyMatrix(n); });
            this.A2 = A2.Result;

            List<Task> tasks_b1_c1 = new List<Task>();
            tasks_b1_c1.Add(Task.Factory.StartNew(() => { b1 = new MyVector(n); }));
            tasks_b1_c1.Add(Task.Factory.StartNew(() => { c1 = new MyVector(n); }));

            List<Task> tasks_B2_C2 = new List<Task>();
            tasks_B2_C2.Add(Task.Factory.StartNew(() => { B2 = new MyMatrix(n); }));
            tasks_B2_C2.Add(Task.Factory.StartNew(() => 
            { 
                C2 = new MyMatrix(n,false);
                C2.Create_C2();
            }));

            List<Task> tasks_A_bi = new List<Task>();
            tasks_A_bi.Add(Task.Factory.StartNew(() => { A = new MyMatrix(n); }));
            tasks_A_bi.Add(Task.Factory.StartNew(() => 
            { 
                bi = new MyVector(n, false);
                bi.Create_bi();
            }));

            //stage 2 processes (3b1 + c1), B2 - C2, A * bi
            var comp2_1 = Task<MyVector>.Factory.ContinueWhenAll(tasks_b1_c1.ToArray(), (some) => 
            {
                return ((b1 * 3) + c1);
            });
            var comp2_2 = Task<MyMatrix>.Factory.ContinueWhenAll(tasks_B2_C2.ToArray(), (some) => 
            {
                return B2 - C2; 
            });
            var y1 = Task<MyMatrix>.Factory.ContinueWhenAll(tasks_A_bi.ToArray(), (some) => { return A * bi; });
            this.y1 = y1.Result;

            //stage 3 processes y2, Y3

            List<Task> tasksTo_y2 = new List<Task> { A1, comp2_1 };
            var y2 = Task<MyMatrix>.Factory.ContinueWhenAll(tasksTo_y2.ToArray(), (some) =>
            {
                return A1.Result * comp2_1.Result;
            });
            this.y2 = y2.Result;


            List<Task> tasksTo_Y3 = new List<Task> { A2, comp2_2 };
            var Y3 = Task<MyMatrix>.Factory.ContinueWhenAll(tasksTo_Y3.ToArray(), (some) => 
            {
                return A2.Result * comp2_2.Result;
            });
            this.Y3 = Y3.Result;

            //stage 4 process Y3^2, y2 * y2'

            List<Task> tasksTo_Y3squared = new List<Task> { Y3 };
            var Y3_squared = Task<MyMatrix>.Factory.ContinueWhenAll(tasksTo_Y3squared.ToArray(), (some) =>
            {
                return Y3.Result * Y3.Result;
            });
            this.Y3squared = Y3_squared.Result;
            List<Task> tasksTo_comp4_1 = new List<Task> { y2 };
            var comp4_1 = Task<MyMatrix>.Factory.ContinueWhenAll(tasksTo_comp4_1.ToArray(), (some) =>
            {
                return y2.Result * y2.Result.GetTransposed();
            });


            //stage 5 process (Y3^3), (y2 * y1'), (Y3 * y2 * y2'), (Y3^2 * y1')

            List<Task> tasksTo_Y3cubed = new List<Task> { Y3_squared };
            var Y3_cubed = Task<MyMatrix>.Factory.ContinueWhenAll(tasksTo_Y3cubed.ToArray(), (some) =>
            {
                return Y3_squared.Result * Y3.Result;
            });
            this.Y3cubed = Y3_cubed.Result;
            List<Task> tasksTo_comp5_1 = new List<Task> { y2, y1 };
            var comp5_1 = Task<MyMatrix>.Factory.ContinueWhenAll(tasksTo_comp5_1.ToArray(), (some) =>
            {
                return y2.Result * y1.Result.GetTransposed();
            });
            List<Task> tasksTo_comp5_2 = new List<Task> { Y3, comp4_1 };
            var comp5_2 = Task<MyMatrix>.Factory.ContinueWhenAll(tasksTo_comp5_2.ToArray(), (some) =>
            {
                return Y3.Result * K1 * comp4_1.Result;
            });
            List<Task> tasksTo_comp5_3 = new List<Task> { Y3 };
            var comp5_3 = Task<MyMatrix>.Factory.ContinueWhenAll(tasksTo_comp5_3.ToArray(), (some) =>
            {
                return Y3_squared.Result * K2 * y1.Result.GetTransposed();
            });

            //stage 6 process 

            List<Task> tasksTo_comp6_1 = new List<Task> { comp5_2, Y3_cubed };
            var comp6_1 = Task<MyMatrix>.Factory.ContinueWhenAll(tasksTo_comp6_1.ToArray(), (some) =>
            {
                return comp5_2.Result + Y3_cubed.Result;
            });
            Program.PrintMatrix(comp6_1.Result, "left one");
            List<Task> tasksTo_comp6_2 = new List<Task> { Y3, comp5_1, comp5_3};
            var comp6_2 = Task<MyMatrix>.Factory.ContinueWhenAll(tasksTo_comp6_2.ToArray(), (some) =>
            {
                return Y3.Result + comp5_1.Result + comp5_3.Result;
            });
            Program.PrintMatrix(comp6_2.Result, "right one");

            //Last stage

            List<Task> lastTask = new List<Task> { comp6_1, comp6_2 };
            var result = Task<MyMatrix>.Factory.ContinueWhenAll(tasksTo_comp6_2.ToArray(), (some) =>
            {
                return comp6_1.Result - comp6_2.Result;
            });

            var tLast = Task.Factory.ContinueWhenAll(lastTask.ToArray(), (some) => 
            {
                this.result = result.Result;
                Console.WriteLine("All done");
            });

            tLast.Wait();
        }
    }   
}
