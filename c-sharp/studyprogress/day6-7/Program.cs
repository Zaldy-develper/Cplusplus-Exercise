using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsoleApp2OOP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\n\n\nprob8-6.(Difficulty ★★★) (Fraction calculation 1)");
            FracCalc fracCalc = new FracCalc();
            fracCalc.NumeratorDenominator(10);
            fracCalc.SumFrac();
            fracCalc.DisplayResult();

            Console.WriteLine("\n\n\nprob8-1.(Difficulty ★★) (Pythagorean triple)");
            PythagoreanTriple pythagoreanTriple = new PythagoreanTriple();
            pythagoreanTriple.GetCombination();
            pythagoreanTriple.DisplayCombination();



            Console.WriteLine("\n\n\nDay7: prob7-1.(Difficulty ★)");
            TwoStrings s = new TwoStrings();
            s.String1 = "Hello";
            s.String2 = "World";
            Console.WriteLine("一つ目の文字列は" + s.String1);
            Console.WriteLine("二つ目の文字列は" + s.String2);
            Console.WriteLine("二つの文字列を合成したものは" + s.GetConnectedString());


            Console.WriteLine("\n\n\nDay7: prob7-1.(Difficulty ★)");
            Data d = new Data();
            d.Number = 100;
            d.Comment = "Programming C#";
            Console.WriteLine("number = " + d.Number);
            Console.WriteLine("comment = " + d.Comment);

            Console.WriteLine("\n\n\nDay7 Access 2");
            Person2 p = new Person2();
            //  名前と年齢を設定
            p.SetAgeAndName("山田太郎", 26);
            //  年齢の変更
            p.Age = 32;
            //  名前の変更（できない）
            //  p.Name = 36;
            //  名前と年齢の表示
            Console.WriteLine("名前：{0}　年齢:{1}", p.Name, p.Age);

            Console.WriteLine("\n\n\nDay7 Access 1");
            Access access = new Access();
            //a.Data1 = 1;
            access.Data2 = 2;
            access.ShowDatas();
            Console.WriteLine("a.data1 = {0}", access.Data1);
            //Console.WriteLine("a.data2 = {0}", a.Data2);

            Console.WriteLine("\n\n\nDay6 Exercise 2");
            Calc2 ccc = new Calc2();
            double aaa = 4.1, bbb = 2.3;
            Console.WriteLine("{0} + {1} = {2:F2}", aaa, bbb, ccc.Add(aaa, bbb));
            Console.WriteLine("{0} - {1} = {2:F2}", aaa, bbb, ccc.Sub(aaa, bbb));
            Console.WriteLine("{0} * {1} = {2:F2}", aaa, bbb, ccc.Mul(aaa, bbb));
            Console.WriteLine("{0} / {1} = {2:F2}", aaa, bbb, ccc.Div(aaa, bbb));

            Console.WriteLine("\n\n\nDay6 Exercise 1");
            MinMax m = new MinMax();
            int aa = 4, bb = 2, cc = 7;
            Console.WriteLine("{0}と{1}と{2}のうち、最大のものは{3}", aa, bb, cc, m.Max(aa, bb, cc));
            Console.WriteLine("{0}と{1}と{2}のうち、最小のものは{3}", aa, bb, cc, m.Min(aa, bb, cc));

            Console.WriteLine("\n\n\nDay6 Lecture 2");
            Calc calc = new Calc();
            int a = 1, b = 2, c = 3;
            int ans1 = calc.Add(a, b);
            int ans2 = calc.Add(a, b, c);
            Console.WriteLine("{0} + {1} = {2}", a, b, ans1);
            Console.WriteLine("{0} + {1} + {2} = {3}", a, b, c, ans2);


            Console.WriteLine("\n\n\nDay6 Lecture 1");
            Person p1, p2;
            p1 = new Person();  //  一つ目のPersonクラスのメソッドのインスタンスを生成
            p2 = new Person();  //  二つ目のPersonクラスのメソッドのインスタンスを生成
            p1.name = "山田太郎";   //  フィールドnameに値を代入
            p1.age = 19;            //  フィールドageに値を代入
            p2.SetAgeAndName("佐藤花子", 23);   //  setAgeAndName()メソッドで、nameとageを設定
            //  showAgeAndName()メソッドで、それぞれのインスタンスのnameとageを表示
            p1.ShowAgeAndName();
            p2.ShowAgeAndName();

            Console.WriteLine("\n\n\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
