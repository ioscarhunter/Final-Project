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
#include "Flasher.h"

class LEDClass
{

private:
	Adafruit_NeoPixel *leds;
	int pixel; 
	unsigned long colour;
	int ledState;
	int frequency;
	long OnTime;     // milliseconds of on-time
	long OffTime;    // milliseconds of off-time
	unsigned long previousMillis;  	// will store last time LED was updated
	int duty;
protected:


public:
	void init(Adafruit_NeoPixel &led, int pix, unsigned long colour,int freq=0);
	void on();
	void off();
	void blackout();
	void changeColour(unsigned long colour);
	void update();
	void changefrequency(int freq);
};

extern LEDClass LED;

#endif

