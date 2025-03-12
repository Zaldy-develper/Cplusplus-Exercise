using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2OOP
{
    class FracCalc
    {
        private
            int a, b, result_a, result_b;

        // Declare and initialize a list of tuples (int, int, int)
        List<int[]> fractions = new List<int[]>();
        public void NumeratorDenominator(int range)
        {
            for(int i = 0; i < 2; i++)
            {
                Random rnd = new Random();
                //Numerator
                do
                {
                    a = rnd.Next(1, range);

                } while (a < 2);
                //Denominator
                do
                {
                    b = rnd.Next(1, range);

                } while (a + b > range && b < 1);

                fractions.Add(new int[] {a, b });
                Console.WriteLine("{0}/{1}", a,b);
            }
        }
        public void SumFrac()
        {
            result_b = fractions[0][1] * fractions[1][1];
            int a1 = (fractions[0][0]*result_b) / fractions[0][1];
            int a2 = (fractions[1][0] * result_b) / fractions[1][1];
            //Console.WriteLine("({0} + {1})/{2}", a1, a2, result_b);
            result_a = a1 + a2;

            // Reduce the fraction by dividing numerator and denominator by their GCD
            int greatestCommonMultiple = GCD(result_a, result_b);
            result_a /= greatestCommonMultiple;
            result_b /= greatestCommonMultiple;

        }

        // Utility method to compute the Greatest Common Divisor (GCD)
        public int GCD(int x, int y)
        {
            while (y != 0)
            {
                int temp = y;
                y = x % y;
                x = temp;
            }
            return x;
        }

        public void DisplayResult()
        {
            if (result_a % result_b == 0)
            {
                Console.WriteLine("Sum is {0}", result_a / result_b);
            }
            if (result_a >= result_b)
            {
                int mixedNum = result_a/ result_b;
                int adjustedNum = result_a% result_b;
                Console.WriteLine("Sum is {0} and {1}/{2}", mixedNum, adjustedNum, result_b);
            }
            else {
                Console.WriteLine("Sum is {0}/{1}", result_a , result_b);
            }
        }

    }
    class PythagoreanTriple
    {
        private
            int a, b, c;
        // Declare and initialize a list of tuples (int, int, int)
        List<(int, int, int)> records = new List<(int, int, int)>();
        public
            void GetCombination(int range = 100)
        {

            for (int i = 1; i <= range; i++)
            {
                for (int j = i; j <= range; j++)
                {
                    int squaresum = i * i + j * j;
                    double sqrtResult = Math.Sqrt(squaresum);
                    int integerSqrt = (int)sqrtResult;
                    // Check if squaresum is a perfect square
                    if (integerSqrt * integerSqrt == squaresum && integerSqrt <= 100)
                    {
                        a = i;
                        b= j;
                        c = integerSqrt;
                        records.Add((a, b, c));
                    }

                }
            }
        }
        public void DisplayCombination()
        {
            // Display the records
            int i = 1;
            foreach (var record in records)
            {
                //Console.WriteLine($"{record.Item1}, {record.Item2}, {record.Item3}");
                Console.WriteLine("Combination {0}: a:{1}, b:{2}, c:{3}", i, record.Item1, record.Item2, record.Item3);
                i++;
            }

        }

    }
    class TwoStrings
    {
        private string string1;
        //  二つ目の文字列
        private string string2;
        //  一つ目の文字列を設定
        public string String1
        {
            set { string1 = value; }
            get { return string1; }
        }
        //  一つ目の文字列を設定
        public string String2
        {
            set { string2 = value; }
            get { return string2; }
        }

        public string GetConnectedString()
        {
            return string1 + string2;
        }
    }
    class Data
    {
        //  メンバ変数number
        private int number = 0;
        //  メンバ変数comment
        private string comment = "";

        public int Number
        {
            get { return number; }
            set { number = value; }
        }
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }
    }
    class Person2
    {
        //  情報の設定
        public void SetAgeAndName(string name, int age)
        {
            Name = name;
            Age = age;
        }
        //  情報の表示（メソッド）
        public void ShowAgeAndName()
        {
            Console.WriteLine("名前：{0} 年齢：{1}", Name, Age);
        }
        //  情報の設定
        public string Name
        {
            private set; get;
        }
        //  情報の設定
        public int Age
        {
            set; get;
        }
    }
    class Access
    {
        //  読み込みオンリーのデータ
        private int data1 = 5;
        //  書き込みオンリーのデータ
        private int data2 = 0;
        //  値の表示
        public void ShowDatas()
        {
            Console.WriteLine("data1={0} data2={1}", data1, data2);
        }
        //  data1のプロパティ（読み込みオンリー）
        public int Data1
        {
            get { return data1; }
        }
        //  data2のプロパティ（書き込みオンリー）
        public int Data2
        {
            set { data2 = value; }
        }
    }
    class Calc2
    {
        public double Add(double a, double b)
        {
            return a + b;
        }
        public double Sub(double a, double b)
        {
            return a - b;
        }
        public double Mul(double a, double b)
        {
            return a * b;
        }
        public double Div(double a, double b)
        {
            return a / b;
        }
    }

    class MinMax
    {
        //  最大値の取得
        public int Max(int n1, int n2)
        {
            if (n1 > n2)
            {
                return n1;
            }
            return n2;
        }
        public int Max(int n1, int n2, int n3)
        {
            if (n1 > n2 && n1 > n3)
            {
                return n1;
            }
            else if (n2 > n1 && n2 > n3)
            {
                return n2;
            }
            return n3;
        }
        //  最大値の取得
        public int Min(int n1, int n2)
        {
            if (n1 < n2)
            {
                return n1;
            }
            return n2;
        }
        public int Min(int n1, int n2, int n3)
        {
            if (n1 < n2 && n1 < n3)
            {
                return n1;
            }
            else if (n2 < n1 && n2 < n3)
            {
                return n2;
            }
            else
                return n3;
        }
    }
    class Person
    {
        //  名前（フィールド）
        public string name = "";
        //  年齢（フィールド）
        public int age = 0;
        //  情報の表示（メソッド）
        public void ShowAgeAndName()
        {
            Console.WriteLine("名前：{0} 年齢：{1}", name, age);
        }
        //  情報の設定
        public void SetAgeAndName(string name, int age)
        {
            this.name = name;
            this.age = age;
        }
    }

    class Calc
    {
        //  二つの整数の引数の和を求める
        public int Add(int a, int b)
        {
            return a + b;
        }
        //  三つの整数の引数の和を求める
        public int Add(int a, int b, int c)
        {
            return a + b + c;
        }
    }
}
