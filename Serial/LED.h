// LED.h

#ifndef _LED_h
#define _LED_h

#define BLACKOUT 0x2

#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#else
#include "WProgram.h"
#endif

#include <Adafruit_NeoPixel.h>
#include "Definition.h"

class LEDClass
{

private:
	Adafruit_NeoPixel *leds;
	int pixel; 
	unsigned long colour;
	int ledState;
protected:


public:
	void init(Adafruit_NeoPixel &led, int pix, unsigned long colour);
	void on();
	void off();
	void blackout();
	void changeColour(unsigned long colour);
};

extern LEDClass LED;

#endif

