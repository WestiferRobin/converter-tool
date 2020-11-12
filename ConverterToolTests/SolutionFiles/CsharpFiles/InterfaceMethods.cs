using System;
using System.Collections.Generic;
using System.Text;

namespace ConverterToolTests.SolutionFiles.CsharpFiles
{
    interface IInterfaceMethods
    {
        void ChangeCadence(int newValue);

        void ChangeGear(int newValue);

        void SpeedUp(int increment);

        void ApplyBrakes(int decrement);
    }
}
