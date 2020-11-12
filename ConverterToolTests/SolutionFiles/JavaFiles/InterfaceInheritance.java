import java.io.*;
import java.lang.*;
import java.util.*;

public class InterfaceInheritance implements InterfaceMethods
{
    private int _speed;
    private int _value;

    public InterfaceInheritance()
    {
        this._speed = 0;
        this._value = 0;
    }

    public void changeCadence(int newValue)
    {
        this._value = newValue;
    }

    public void changeGear(int newValue)
    {
        this._value = newValue;
    }

    public void speedUp(int increment)
    {
        this._speed += increment;
    }

    public void applyBrakes(int decrement)
    {
        this._speed -= decrement;
    }
}