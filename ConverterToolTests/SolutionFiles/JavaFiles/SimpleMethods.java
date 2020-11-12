import java.io.*;
import java.lang.*;
import java.util.*;

public class SimpleMethods
{
    private String _name;
    private int _value;

    public SimpleMethods()
    {
        _name = "Hello World";
        _value = 0;
    }

    private void setName(String newValue)
    {
        this._name = newValue;
    }

    private String getName()
    {
        return this._name;
    }

    public void addNumber(int targetValue)
    {
        this._value += targetValue;
    }

    public void getValue()
    {
        return this._value;
    }
}