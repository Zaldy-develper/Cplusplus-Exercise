using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            //program.day1and2();

            

            Console.WriteLine("\n\n\n");
            Console.WriteLine("\n\nPress any key to continue...");
            Console.ReadKey();
            Environment.Exit(0);
        }

        void day5()
        {
            Console.WriteLine("\n\n\nprob5 - 12.(Difficulty:★)");
            int[,] num4 = new int[3, 3];
            Random rnd = new Random();
            int total = 15;
            int min = total, max = 0;
            // Continue generating a candidate until it is unique.
            for (int i = 0; i < num4.GetLength(0); i++)
            {
                for (int j = 0; j < num4.GetLength(1); j++)
                {
                    int candidate;
                    bool duplicate;

                    // Generate a candidate until it's unique in the already filled cells.
                    do
                    {
                        candidate = rnd.Next(1, total);  // Generates a number from 1 to 10.
                        duplicate = false;

                        // Check all previously filled cells.
                        for (int k = 0; k < num4.GetLength(0); k++)
                        {
                            for (int l = 0; l < num4.GetLength(1); l++)
                            {
                                // Only check cells that are filled: either in a previous row,
                                // or in the current row before the current column.
                                if (k < i || (k == i && l < j))
                                {
                                    if (num4[k, l] == candidate)
                                    {
                                        duplicate = true;
                                        break; // Exit the inner loop early.
                                    }
                                }
                            }
                            if (duplicate)
                                break; // Exit the outer loop early.
                        }
                    } while (duplicate);

                    // Once we have a unique candidate, assign it to the current cell.
                    num4[i, j] = candidate;
                    Console.Write(num4[i, j] + " ");
                    if (num4[i, j] >= max)
                    {
                        max = num4[i, j];
                    }
                    else if (num4[i, j] <= min)
                    {
                        min = num4[i, j];
                    }
                }
                Console.WriteLine("");
            }
            Console.WriteLine("Max: {0} Min: {1}", max, min);

            Console.WriteLine("\n\n\nprob5-7.(Difficulty:★★★)");

            int[] num3 = new int[5];

            for (int i = 0; i < num3.Length; i++)
            {
                num3[i] = rnd.Next(1, 101);
                Console.Write(num3[i] + " ");
            }
            Console.WriteLine("");

            int sum = 0;
            float avg = 0;
            for (int i = 0; i < num3.Length; i++)
            {
                sum += num3[i];
                avg = ((avg * i) + num3[i]) / (i + 1);
            }
            Console.WriteLine("Sum: {0} | Ave: {1}", sum, avg);

            Console.WriteLine("\n\n\nprob5-3.(Difficulty:★★)");

            int[] num2 = new int[10];
            string odd = "", even = "";


            for (int i = 0; i < num2.Length; i++)
            {
                num2[i] = rnd.Next(1, 101);
                if (num2[i] % 2 == 0)
                {
                    odd += (num2[i]).ToString() + " ";
                }
                else
                {
                    even += num2[i].ToString() + " ";
                }

            }
            Console.WriteLine(odd);
            Console.WriteLine(even);

            Console.WriteLine("\n\n\nprob5 - 1.(Difficulty:★)");

            int[] num = new int[7];

            // Use a for loop to iterate over each index in the array.
            for (int i = 0; i < num.Length; i++)
            {
                int candidate;
                bool duplicate;

                // Continue generating a candidate until it is unique.
                do
                {
                    candidate = rnd.Next(1, 11);  // Generates a number from 1 to 10 (11 is exclusive)
                    duplicate = false;

                    // Check the candidate against all previously assigned numbers.
                    for (int j = 0; j < i; j++)
                    {
                        if (num[j] == candidate)
                        {
                            duplicate = true;
                            break;  // No need to check further if a duplicate is found.
                        }
                    }
                } while (duplicate);

                // Assign the unique candidate to the array.
                num[i] = candidate;
                Console.Write("a[{0}]={1} ", i, num[i]);
            }

        }
        void day5_lec()
        {
            // Jagged Array
            int[][] aa = new int[][] { new int[] { 0, 1 }, new int[] { 2 }, new int[] { 3, 4, 5, 6 } };
            //  成分の表示
            for (int mm = 0; mm < aa.Length; mm++)
            {
                for (int nn = 0; nn < aa[mm].Length; nn++)
                {
                    Console.Write(aa[mm][nn] + " ");
                }
                Console.WriteLine();
            }


            int[,] a = new int[3, 4];
            int m, p;
            //  二次元配列に値を代入
            for (m = 0; m < 3; m++)
            {
                for (p = 0; p < 4; p++)
                {
                    a[m, p] = m + p;
                }
            }
            //  二次元配列に値を出力
            for (m = 0; m < 3; m++)
            {
                for (p = 0; p < 4; p++)
                {
                    Console.Write("a[{0},{1}]={2} ", m, p, a[m, p]);
                }
                Console.WriteLine();
            }

            int[] n = { 1, 2, 3, 4 };
            foreach (int i in n)
            {
                Console.Write("{0} ", i);
            }
            Console.WriteLine();

            double[] d = new double[3];
            d[0] = 1.2;
            d[1] = 3.7;
            d[2] = 4.1;    //  変数の代入
            double sum2, avg2; //  合計値、平均値を入れる変数
            sum2 = 0.0;
            for (int i = 0; i < d.Length; i++)
            {
                Console.Write(d[i] + " ");
                sum2 += d[i];
            }
            Console.WriteLine();
            avg2 = sum2 / d.Length;
            Console.WriteLine("合計値：" + sum2);
            Console.WriteLine("平均値：" + avg2);

            double one, two, three;
            double sum, avg; //  合計値、平均値を入れる変数
            one = 1.2;
            two = 3.7;
            three = 4.1;    //  変数の代入
            Console.WriteLine(one + " " + two + " " + three);
            sum = one + two + three;    //  合計値の計算
            avg = sum / 3.0;            //  平均値の計算
            Console.WriteLine("合計値：" + sum);
            Console.WriteLine("平均値：" + avg);
        }
        void day3()
        {
            Console.WriteLine("prob3-1.(Difficulty:★)");
            Console.Write("input an integer value:");
            int num = int.Parse(Console.ReadLine());

            switch (num)
            {
                case 3:
                    Console.WriteLine("It is 3.");
                    break;
                default:
                    Console.WriteLine("It is not a 3.");
                    break;
            }

            Console.WriteLine("\n\n\nprob3-2.(Difficulty:★)");
            Console.Write("input an integer value:");
            int num2 = int.Parse(Console.ReadLine());

            if (num2 != 4)
            {
                Console.WriteLine("It is not a 4");
            }

            Console.Write("input a string1: ");
            string? s1 = Console.ReadLine();
            Console.Write("input a string2: ");
            string? s2 = Console.ReadLine();
            if (s1 == s2)
            {
                Console.WriteLine("the two strings are equal");
            }
            else
            {
                Console.WriteLine("the two strings are not equal.");
            }



            Console.Write("数値を入力: ");
            int num3 = int.Parse(Console.ReadLine());
            if (num3 % 2 == 0)
            {
                Console.WriteLine("Even number");
            }
            else
            {
                Console.WriteLine("Odd number");
            }

        }
        void day3_lec()
        {
            //  キーワードから数値を入力
            Console.Write("整数値を入力:");
            int ab = int.Parse(Console.ReadLine());
            Console.WriteLine("a=" + ab);
            //  入力した値が、正の数かどうかを調べる
            if (ab > 0)
            {
                Console.WriteLine("aは正の数です。");  //  正の数だった場合に実行
            }

            //  キーワードから数値を入力
            Console.Write("整数値を入力:");
            int a = int.Parse(Console.ReadLine());
            Console.WriteLine("a=" + a);
            //  入力した値が、正の数かどうかを調べる
            if (a > 0)
            {
                Console.WriteLine("aは正の数です。");  //  正の数だった場合に実行
            }
            else
            {
                Console.WriteLine("aは正の数ではありません。");
            }

            //  キーボードから数値を入力
            Console.Write("1から3の整数を入力:");
            int num = int.Parse(Console.ReadLine());
            if (num == 1)
            {
                Console.WriteLine("one");    //  numが1だった場合の処理
            }
            else if (num == 2)
            {
                Console.WriteLine("two");    //  numが2だった場合の処理
            }
            else if (num == 3)
            {
                Console.WriteLine("three");  //  numが3だった場合の処理
            }
            else
            {
                Console.WriteLine("不適切な値です。"); //  それ以外の値が入力された場合の処理
            }

            //  サイコロの目を入力
            Console.Write("さいころの目(1～6):");
            //  コンソールから数値を入力
            int dice = int.Parse(Console.ReadLine());
            //  値が、サイコロの目の範囲内かどうかを調べる
            if (1 <= dice && dice <= 6)
            {
                //  さいころの目が、偶数か、奇数かで、処理を分ける。
                if (dice == 2 || dice == 4 || dice == 6)
                {
                    Console.WriteLine("丁（チョウ）です。");  //  偶数ならば丁（チョウ）
                }
                else
                {
                    Console.WriteLine("半（ハン）です。");   //  奇数ならば半（ハン）
                }
            }
            else
            {
                Console.WriteLine("範囲外の数値です。");
            }

            //  キーボードから数値を入力
            Console.Write("1から3の整数を入力:");
            int num2 = int.Parse(Console.ReadLine());
            switch (num2)
            {
                case 1:
                    Console.WriteLine("one");    //  numが1だった場合の処理
                    break;
                case 2:
                    Console.WriteLine("two");    //  numが2だった場合の処理
                    break;
                case 3:
                    Console.WriteLine("three");  //  numが3だった場合の処理
                    break;
                default:
                    Console.WriteLine("不適切な値です。"); //  それ以外の値が入力された場合の処理
                    break;
            }

            //  サイコロの目を入力
            Console.Write("さいころの目(1～6):");
            //  コンソールから数値を入力
            int dice2 = int.Parse(Console.ReadLine());
            switch (dice2)
            {
                case 1:
                case 3:
                case 5:
                    Console.WriteLine("丁（チョウ）です。");  //  偶数ならば丁（チョウ）
                    break;
                case 2:
                case 4:
                case 6:
                    Console.WriteLine("半（ハン）です。");   //  奇数ならば半（ハン）
                    break;
                default:
                    Console.WriteLine("範囲外の数値です。");
                    break;
            }

        }
        void day1and2()
        {
            Console.WriteLine("Hello, World!");
            //  数値の表示
            Console.Write(123);
            Console.WriteLine(456);
            //  文字列の表示
            Console.Write("ABC");
            Console.WriteLine("DEF");

            //prob1 - 1.(Difficulty:★)
            Console.WriteLine("\n\n\nprob1 - 1.(Difficulty:★)");
            Console.WriteLine("Rizaldy de Guzman");
            Console.WriteLine(123456789);
            Console.WriteLine("\n\n\n");

            int a = 5;
            int b = 3;
            Console.WriteLine("{0} + {1} = {2}", a, b, a + b);
            Console.WriteLine("{0} - {1} = {2}", a, b, a - b);
            Console.WriteLine("{0} * {1} = {2}", a, b, a* b);
            Console.WriteLine("{0} / {1} = {2}", a, b, a / b);
            Console.WriteLine("{0} % {1} = {2}", a, b, a % b);

            //prob2 - 2.(Difficulty:★)
            Console.WriteLine("\n\n\nprob2 - 2.(Difficulty:★)");
            a = 4;
            b = 2;
            Console.WriteLine("a = {0}", a);
            Console.WriteLine("b = {0}", b);
            Console.WriteLine("a + b = {0}", a + b);
            Console.WriteLine("a - b = {0}", a - b);
            Console.WriteLine("a * b = {0}", a* b);
            Console.WriteLine("a / b = {0}", a / b);
            Console.WriteLine("a % b = {0}", a % b);

            Console.WriteLine("\n\n\nprob2-2.(Difficulty:★★)");
            a = 1; 
            b = 2; 
            int c = 3, d = 4, e = 5;
            a += 2;  //  aに2を加える
            b -= 1;  //  bから1を引く
            c *= 3;  //  cに3をかける
            d /= 2;  //  dを2で割る
            e %= 2;  //  eに、eを2で割った余りを代入する
            Console.WriteLine("a = {0}", a);
            Console.WriteLine("b = {0}", b);
            Console.WriteLine("c = {0}", c);
            Console.WriteLine("d = {0}", d);
            Console.WriteLine("e = {0}", e);

            Console.WriteLine("\n\n\nprob2-3.(Difficulty:★)");
            // Read the string entered by the user
            // Use this if you want to explicitly allow for the possibility of a null value.
            Console.Write("Enter a string: ");
            string? input = Console.ReadLine();
            // Display the input string exactly as it was entered
            Console.WriteLine("Entered string: " + input);

            Console.WriteLine("\n\n\nprob2-4.(Difficulty:★)");
            string? input2;
            Console.Write("Enter string 1: ");
            input = Console.ReadLine();
            Console.Write("Enter string 2: ");
            input2 = Console.ReadLine();
            Console.WriteLine("String 1 + String 2 = {0} {1}", input, input2);

            const int NUMBER = 100;
            const string STRING = "Hoge";
            Console.WriteLine(NUMBER);
            Console.WriteLine(STRING);
            //  constがついた変数は値を変えられない
            //NUMBER = 100;
            //STRING = "fuga";
        }
}
}
