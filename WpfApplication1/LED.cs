using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    class LED
    {
        public LED(int pixel, long colour, ref SerialPort port)
        {
            this._pixel = pixel;
            this._colour = colour;
            this._port = port;
            
        }
        //public int lednum;
        public int _pixel;
        private SerialPort _port;

        private long _colour;
        public void setupLED()
        {
            _port.Write("S:" + _pixel + "&" + _colour + "#");
        }

        public void turnon() { _port.Write("B:" + _pixel + "&" + "1" + "#"); }
        public void turnoff() { _port.Write("B:" + _pixel + "&" + "0" + "#"); }
        public void blackout() { _port.Write("B:" + _pixel + "&" + "-1" + "#"); }

    }
}
