using System;
using System.Collections.Generic;
using System.Text;

namespace ConverterToolTests.SolutionFiles.CsharpFiles
{
    public abstract class AbstractMethods
    {
        private int _x = 0;
        private int _y = 0;
        void moveTo(int newX, int newY)
        {
            this._x = newX;
            this._y = newY;
        }
        public abstract void Draw();
        public abstract void Resize();
    }
}
