/*
 Name:		Serial.ino
 Created:	9/16/2015 9:01:48 PM
 Author:	Karav
*/
#include <Adafruit_NeoPixel.h>
#include "LED.h"
#include <arduino.h>
#include <stdlib.h> 
#include "LED.h"
#include "Definition.h"

#define PIN 3
#define LED_COUNT 8

using namespace std;

Adafruit_NeoPixel leds = Adafruit_NeoPixel(LED_COUNT, PIN, NEO_GRB + NEO_KHZ800);
String incomingByte = " ";

bool isSet[LED_COUNT];
LEDClass ledarray[LED_COUNT];
// the setup function runs once when you press reset or power the board
void setup() {

	Serial.begin(460800);           // set up Serial library at 460800 bps
	Serial.setTimeout(5);
	Serial.println("Hello world!");
	
	leds.begin();  // Call this to start up the LED strip.
	clearLEDs();   // This function, defined below, turns all LEDs off...
	leds.setBrightness(250);

	leds.show();   // ...but the LEDs don't actually update until you call this.

}

// the loop function runs over and over again until power down or reset
void loop() {
	if (Serial.available() > 0) {

		// read the incoming byte:
		for (int i = 1; i < 2; i++) {
			String command = Serial.readStringUntil(':');
			
			if (command != "") {

				//Serial.println("command " + command);
				//here you could check the servo number
				String pos = Serial.readStringUntil('&');
				int pixel = pos.toInt();

				//Serial.print("Pos ");
				//Serial.println(pixel);
				String hex = Serial.readStringUntil('#');
				
				if (command.equals("S")) {
					if (!isSet[pixel]) {
						unsigned long colour = hex.toInt();
						//Serial.print("colour ");
						//Serial.println(colour);
						isSet[pixel] = true;
						ledarray[pixel].init(leds, pixel, colour);
					}
					else {
						unsigned long colour = hex.toInt();
						ledarray[pixel].changeColour(colour);
					}
				}

				else if (command.equals("B")&&isSet[pixel]) {

					if (hex.equals("1")) {
						ledarray[pixel].on();
						//Serial.println("on");
					}

					else if (hex.equals("0")) {
						ledarray[pixel].off();
						//Serial.println("off");
					}
					else if (hex.equals("-1")) {
						ledarray[pixel].blackout();
					}

				}
			}
		}
	}
}

// Sets all LEDs to off, but DOES NOT update the display;
// call leds.show() to actually turn them off after this.
void clearLEDs()
{
	for (int i = 0; i<LED_COUNT; i++)
	{
		leds.setPixelColor(i, 0);
	}
}


