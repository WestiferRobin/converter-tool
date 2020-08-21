using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Values;

namespace HelloWorldSharp
{
    public static class Program
    {
        // this is going to be a problem...
        private static string _randomMessage;

        private static const int CONSTANT_INT = 234;
        
        public static void Main(string[] args)
        {
            try
            {
                this._randomMessage = "This thing is going to be hard"; // or isssss it?!?!?
                PrintMessage(3);    /* this is going to read the message */
                Console.WriteLine(RandomMessage);
                /*
                asdfasdfasdfasdf fdsa
                asdf
                fdsa
                asdf
                fdsa
                asdf
                */
                Values asdf = new Values(123);
                asdf.Value2 = "asdf";
                Console.WriteLine(asdf.Value);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine("ASDFASDF");
            }
            
            int p = 0;
            do
            {
                Console.WriteLine(p++);
            } while (p < 10);
        }

        public static string GetRandomMessage()
        {
            return this._randomMessage;
        }
        public static void SetRandomMessage(string value)
        {
            this._randomMessage = value;
        }

        public static void PrintMessage(int size)
        {
            for (int i = 0; i < size; i++)
            {
                string message = "Hello, World ";
                message += GetNumber();
                Console.WriteLine(message);
            }

            if (size >= 10)
            {
                printWes(size % 2);
            }
            else if (size <= 3 && size < 10)
            {
                printWes(size);
            }
            else
            {
                Console.WriteLine("Encore! E value is " + Math.E);
            }
        }

        public static string GetNumber()
        {
            Random rand = new Random();
            return rand.Next().ToString();
        }

        private static void printWes(int times)
        {
            while (times >= 0)
            {
                Console.WriteLine("Wesley Webb");
                times--;
            }
        }

        public static void SwieTest()
        {
            int asdf = 234;
            switch (asdf)
            {
                case 1:
                    printWes(1);
                    break;
                case 2:
                case 3:
                    print(3);
                    break;
                default:
                    printWes(asdf);
                    break;
            }
        }
    }

    public struct WesMan
    {
        private struct Bean
        {
            public Bean(int asdf, int fdsa)
            {

            }
        }

        public enum BBB
        {
            HAHA,
            HEHE,
        }

        public class Wows
        {
            public Wow()
            {

            }
        }
        public static string Asdf {get; set;}
        private static string Fdsa { get; set; }
        public WesMan(int asdf, int fdsa)
        {

        }

        public void GetMeThatThang()
        { 
            var moString = Asdf + Fdsa;
        }
    }

    public struct Welsey
    {
        public int BeansWesLikes {get;set;}
        public Welsey(int bean)
        {
            this.BeansWesLikes = bean;
        }
    }

    public enum OterEnum {
        FFFF,
        AAAA
    }
}