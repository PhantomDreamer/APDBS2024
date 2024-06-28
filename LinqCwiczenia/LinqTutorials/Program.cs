using System;

namespace LinqTutorials
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = LinqTasks.Task1();
            var t2 = LinqTasks.Task2();
            var t3 = LinqTasks.Task3();
            var t4 = LinqTasks.Task4();
            var t5 = LinqTasks.Task5();
            var t6 = LinqTasks.Task6();
            var t7 = LinqTasks.Task7();
            var t8 = LinqTasks.Task8();
            var t9 = LinqTasks.Task9();
            var t10 = LinqTasks.Task10();
            var t11 = LinqTasks.Task11();
            var t12 = LinqTasks.Task12();
            var t13 = LinqTasks.Task13(new int[]{1,1,1,1,1,1,10,1,1,1,1});
            var t14 = LinqTasks.Task14();
            var t15 = LinqTasks.Task15();
            var t16 = LinqTasks.Task16();
            //Console.WriteLine(t13);
            foreach (var i in t16) {
                Console.WriteLine(i);
            }
        }
    }
}
