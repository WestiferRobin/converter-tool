import java.io.*;
import java.lang.*;
import java.util.*;

interface InterfaceMethods {

    //  wheel revolutions per minute
    void changeCadence(int newValue);

    void changeGear(int newValue);

    void speedUp(int increment);

    void applyBrakes(int decrement);
}