import java.io.*;
import java.lang.*;
import java.util.*;
import values;

public class HelloWorldJ
{
    // this is going to be a problem...
    private static String _randomMessage;
    private static int _randomNumber = 234;
    private static const int CONST_NUMBER = 777;
    
    public static String getRandomMessage()
    {
        return _randomMessage;
    }
    
    public static void setRandomMessage(String value)
    {
        _randomMessage = value;
    }
    
    public static void main(String[] args)
    {
        setRandomMessage("This thing is going to be hard"); // or isssss it?!?!?
        printMessage(5);    /* this is going to read the message */
        System.out.println(getRandomMessage());
        /*
            asdfasdfasdfasdf fdsa
            asdf
            fdsa
            asdf
            fdsa
            asdf
            */
    }

    public static void printMessage(int size)
    {
        for (int i = 0; i < size; i++)
        {
            String message = "Hello, World ";
            message += getNumber();
            System.out.println(message);
        }
        printWes((size % 2) + 1);
        System.out.println("Encore! E value is " + Math.E);
    }

    public static String getNumber()
    {
        Random rand = new Random();
        return rand.nextInt() + "";
    }

    public static void printWes(int times)
    {
        while (times > 0)
        {
            System.out.println("Wesley Webb");
            times--;
        }
    }

    public static void switchTest()
    {
        int asdf = 234;
        switch (asdf)
        {
            case 1:
                printWes(1);
                break;
            case 2:
            case 3:
                print(3):
                break;
            default:
                printWes(asdf);
                break;
        }
    }
}