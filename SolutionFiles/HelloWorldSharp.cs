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

        public static void Main(string[] args)
        {
            _randomMessage = "This thing is going to be hard"; // or isssss it?!?!?
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
            var asdf = new Values(123);
            Console.WriteLine(asdf.Value);
        }

        public static string GetRandomMessage()
        {
            return _randomMessage;
        }
        public static void SetRandomMessage(String value)
        {
            _randomMessage = value;
        }

        public static void PrintMessage(int size)
        {
            for (int i = 0; i < size; i++)
            {
                string message = "Hello, World ";
                message += GetNumber();
                Console.WriteLine(message);
            }
            printWes(size % 2);
            Console.WriteLine("Encore! E value is " + Math.E);
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
    }
}