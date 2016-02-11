// 
// 
// 

#include "LED.h"

void LEDClass::init(Adafruit_NeoPixel &led, int pix, unsigned long colour,int freq)
{
	this->leds = &led;
	this->pixel = pix;
	this->colour = colour;
	ledState = LOW;
	this->duty = 70;
	this->leds = &led;
	this->colour = colour;
	this->pixel = pix;
	this->frequency = freq;
	this->OnTime = (duty * 10);
	this->OffTime = ((100 - duty) * 10);
	ledState = LOW;
	previousMillis = 0;
}


void LEDClass::off() {
	if (ledState == HIGH || ledState == BLACKOUT) {
		ledState = LOW;  // Turn it off
		delay(80);
		leds->setPixelColor(pixel, (colour & 0xfefefe) >> 1);
		leds->show();
	}
}

void LEDClass::on() {
	if (ledState == LOW || ledState == BLACKOUT) {
		ledState = HIGH;  // turn it on
		delay(80);
		leds->setPixelColor(pixel, colour);
		leds->show();
	}
}

void LEDClass::blackout() {
	if (ledState == LOW || ledState == HIGH) {
		ledState = BLACKOUT;  // turn it offfff
		delay(80);
		leds->setPixelColor(pixel, BLACK);
		leds->show();
	}
}

void LEDClass::changeColour(unsigned long colour) {
	this->colour = colour;
}

void LEDClass::update() {
	if (frequency != 0) {
		// check to see if it's time to change the state of the LED
		unsigned long currentMillis = millis();

		if ((ledState == HIGH) && (currentMillis - previousMillis >= OnTime/frequency))
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
}
void LEDClass::changefrequency(int freq) {
	this->frequency = freq;
}
