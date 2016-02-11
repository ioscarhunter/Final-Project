#include "Flasher.h"



void Flasher::init(Adafruit_NeoPixel &led, unsigned long colour, int freq, int duty, int pix) {

	this->leds = &led;
	this->colour = colour;
	this->pixel = pix;
	this->frequency = freq;
	this->OnTime = (duty * 10) / freq;
	this->OffTime = ((100 - duty) * 10);
	ledState = LOW;
	previousMillis = 0;
}


void Flasher::Update()
{
	if (frequency != 0) {
		// check to see if it's time to change the state of the LED
		unsigned long currentMillis = millis();

		if ((ledState == HIGH) && (currentMillis - previousMillis >= OnTime))
		{
			ledState = LOW;  // Turn it off
			previousMillis = currentMillis;  // Remember the time
			leds->setPixelColor(pixel, (colour & 0xfefefe) >> 1);
			leds->show();
		}
		else if ((ledState == LOW) && (currentMillis - previousMillis >= OffTime / frequency))
		{
			ledState = HIGH;  // turn it on
			previousMillis = currentMillis;   // Remember the time
			leds->setPixelColor(pixel, colour);
			leds->show();
		}
	}
};

void Flasher::changeFreq(int freq) {
	this->frequency = freq;
}