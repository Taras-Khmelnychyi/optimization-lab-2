﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Lab2
{
    class Program
    {
        static void Main()
        {
            //MyMatrix A = new MyMatrix(2);
            //MyMatrix B = new MyMatrix(2);
            //A.HandInit("A");
            //B.HandInit("B");
            //(A * B).ShowMatrix("A + B"); 
            MainProcess prc = new MainProcess();
            prc.Start();
        }
    }

    class MainProcess
    {
        int n;
        public MyMatrix A1, A2, B2, C2, A, b1, c1, bi, Y3, y1, y2, Y3squared, Y3cubed;
        public MyMatrix result;

        public void Start() {
            
            Console.WriteLine("Enter n: ");
            n = Convert.ToInt32(Console.ReadLine());
            InitStage();
        }

        private void InitStage() {
            //init stage
            //initialised by formula
            bi = new MyMatrix(n);
            var create_bi = Task.Factory.StartNew(() => { bi.Create_bi(); });
            C2 = new MyMatrix(n);
            var create_C2 = Task.Factory.StartNew(() => { C2.Create_C2(); });

            Console.WriteLine("Initialise random? y/n");
            ConsoleKey answer = Console.ReadKey(true).Key;

            if (answer == ConsoleKey.Y)
            {
                A = new MyMatrix(n);
                A1 = new MyMatrix(n);
                A2 = new MyMatrix(n);
                B2 = new MyMatrix(n);
                b1 = new MyMatrix(n);
                c1 = new MyMatrix(n);
                //creating and starting new task for each initial matrix to run in sync
                var create_A = Task.Factory.StartNew(() => { A.RandomInitMatrix(); });
                var create_A1 = Task.Factory.StartNew(() => { A1.RandomInitMatrix(); });
                var create_A2 = Task.Factory.StartNew(() => { A2.RandomInitMatrix(); });
                var create_B2 = Task.Factory.StartNew(() => { B2.RandomInitMatrix(); });
                var create_b1 = Task.Factory.StartNew(() => { b1.RandomInitVector(); });
                var create_c1 = Task.Factory.StartNew(() => { c1.RandomInitVector(); });
                Task.WaitAll();
            }
            else
            {
                Console.WriteLine("Write row elements in a single line separeted by space");
                A = new MyMatrix(n);
                A1 = new MyMatrix(n);
                A2 = new MyMatrix(n);
                B2 = new MyMatrix(n);
                b1 = new MyMatrix(n);
                c1 = new MyMatrix(n);
                A.HandInit("A");
                A1.HandInit("A1");
                A2.HandInit("A2");
                B2.HandInit("B2");
                b1.HandInit("b1");
                c1.HandInit("c1");
            }
            MainSync();
        }

        private void MainSync() {
            //each element on each stage waits
            //only NEEDED elements for his computation
            //for example Y3 waits only for A2 and (B2 - C2)
            //not for all stage

            //stage 2 processes , B2 - C2, A * bi
            var comp2_1 = Task<MyMatrix>.Factory.StartNew(() =>
            {
                return ((b1 * 3) + c1);
            });
            var comp2_2 = Task<MyMatrix>.Factory.StartNew(() =>
            {
                return B2 + C2;
            });
            var y1 = Task<MyMatrix>.Factory.StartNew(() => { return A * bi; });
            this.y1 = y1.Result;

            
            comp2_1.Result.ShowMatrix("3b1 + c1");
            comp2_2.Result.ShowMatrix("B2+C2");
            //stage 3 processes y2, Y3

            List<Task> tasksTo_y2 = new List<Task> { comp2_1 };
            var y2 = Task<MyMatrix>.Factory.ContinueWhenAll(tasksTo_y2.ToArray(), (some) =>
            {
                return A1 * comp2_1.Result;
            });
            this.y2 = y2.Result;
            List<Task> tasksTo_Y3 = new List<Task> { comp2_2 };
            var Y3 = Task<MyMatrix>.Factory.ContinueWhenAll(tasksTo_Y3.ToArray(), (some) =>
            {
                return A2 * comp2_2.Result;
            });
            this.Y3 = Y3.Result;

            //stage 4 process Y3^2, (y2 * y2'), (y2 * y1')
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
            List<Task> tasksTo_comp4_2 = new List<Task> { y2, y1 };
            var comp4_2 = Task<MyMatrix>.Factory.ContinueWhenAll(tasksTo_comp4_2.ToArray(), (some) =>
            {
                return y2.Result * y1.Result.GetTransposed();
            });

            //stage 5 process (Y3^3),  (Y3 * y2 * y2'), (Y3^2 * y1')
            List<Task> tasksTo_Y3cubed = new List<Task> { Y3_squared };
            var Y3_cubed = Task<MyMatrix>.Factory.ContinueWhenAll(tasksTo_Y3cubed.ToArray(), (some) =>
            {
                return Y3_squared.Result * Y3.Result;
            });
            this.Y3cubed = Y3_cubed.Result;
            List<Task> tasksTo_comp5_2 = new List<Task> { Y3, comp4_1 };
            var comp5_2 = Task<MyMatrix>.Factory.ContinueWhenAll(tasksTo_comp5_2.ToArray(), (some) =>
            {
                return Y3.Result * comp4_1.Result;
            });
            List<Task> tasksTo_comp5_3 = new List<Task> { Y3 };
            var comp5_3 = Task<MyMatrix>.Factory.ContinueWhenAll(tasksTo_comp5_3.ToArray(), (some) =>
            {
                return Y3_squared.Result * y1.Result.GetTransposed();
            });

            //stage 6 process 
            List<Task> tasksTo_comp6_1 = new List<Task> { comp5_2, Y3_cubed };
            var comp6_1 = Task<MyMatrix>.Factory.ContinueWhenAll(tasksTo_comp6_1.ToArray(), (some) =>
            {
                return comp5_2.Result + Y3_cubed.Result;
            });
            List<Task> tasksTo_comp6_2 = new List<Task> { Y3, comp4_2, comp5_3 };
            var comp6_2 = Task<MyMatrix>.Factory.ContinueWhenAll(tasksTo_comp6_2.ToArray(), (some) =>
            {
                return Y3.Result + comp4_2.Result + comp5_3.Result;
            });

            //Last stage
            List<Task> lastTask = new List<Task> { comp6_1, comp6_2 };
            var result = Task<MyMatrix>.Factory.ContinueWhenAll(tasksTo_comp6_2.ToArray(), (some) =>
            {
                return comp6_1.Result - comp6_2.Result;
            });

            Task tLast = Task.Factory.ContinueWhenAll(lastTask.ToArray(), (some) =>
            {
                this.result = result.Result;
            });

            Task.WaitAll();
            ResultStage();
        }

        private void ResultStage() {
            Console.WriteLine("Show matrices? y/n");
            ConsoleKey answer = Console.ReadKey(true).Key;

            //A1, A2, B2, C2, A, b1, c1, bi, Y3, y1, y2, Y3squared, Y3cubed
            if (answer == ConsoleKey.Y)
            {
                Console.WriteLine("Matrices:");
                A1.ShowMatrix("A1");
                A2.ShowMatrix("A2");
                B2.ShowMatrix("B2");
                C2.ShowMatrix("C2");
                A.ShowMatrix("A");
                b1.ShowMatrix("b1");
                c1.ShowMatrix("c1");
                bi.ShowMatrix("bi");
                Y3.ShowMatrix("Y3");
                y1.ShowMatrix("y1");
                y2.ShowMatrix("y2");
                result.ShowMatrix("result");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }   
}
