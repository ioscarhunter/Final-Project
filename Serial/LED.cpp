// 
// 
// 

#include "LED.h"

void LEDClass::init(Adafruit_NeoPixel &led, int pix, unsigned long colour)
{
	this->leds = &led;
	this->pixel = pix;
	this->colour = colour;
	ledState = LOW;
}


void LEDClass::off() {
	if (ledState == HIGH || ledState == BLACKOUT) {
		ledState = LOW;  // Turn it off
		leds->setPixelColor(pixel, (colour & 0xfefefe) >> 1);
		leds->show();
	}
}

void LEDClass::on() {
	if (ledState == LOW || ledState == BLACKOUT) {
		ledState = HIGH;  // turn it on
		leds->setPixelColor(pixel, colour);
		leds->show();
	}
}

void LEDClass::blackout() {
	if (ledState == LOW || ledState == HIGH) {
		ledState = BLACKOUT;  // turn it offfff
		leds->setPixelColor(pixel, BLACK);
		leds->show();
	}
}

void LEDClass::changeColour(unsigned long colour) {
	this->colour = colour;
}