import java.io.*;
import java.lang.*;
import java.util.*;

public abstract class AbstractMethods
{
    private int _x;
    private int _y;
    void moveTo(int newX, int newY) {
        this._x = newX;
        this._y = newY;
    }
    abstract void draw();
    abstract void resize();
}