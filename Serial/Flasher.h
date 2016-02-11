
#ifndef _Flasher_h
#define _Flasher_h
#endif

#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#else
#include "WProgram.h"
#endif
#include <Adafruit_NeoPixel.h>
#include "Definition.h"

class Flasher {
	private:
		// Class Member Variables
		// These are initialized at startup
		int pixel;      // the number of the LED pin
		long OnTime;     // milliseconds of on-time
		long OffTime;    // milliseconds of off-time

						 // These maintain the current state
		int ledState;             		// ledState used to set the LED
		unsigned long previousMillis;  	// will store last time LED was updated

										// Constructor - creates a Flasher 
										// and initializes the member variables and state
		unsigned long colour;
		int frequency;
		Adafruit_NeoPixel *leds;

	public:
		void init(Adafruit_NeoPixel &led,unsigned long colour, int freq, int duty, int pix);
		void Update();
		void changeFreq(int freq);
	};

