using System.IO.Ports;

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
		public void blink(int freq) { _port.Write("F:" + _pixel + "&" + freq + "#"); }

		public void changecolour(int colour)
		{
			_colour = colour;
			_port.Write("S:" + _pixel + "&" + colour);
		}

	}
}
