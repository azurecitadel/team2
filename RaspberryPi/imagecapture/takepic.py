#!/usr/bin/env python

from time import sleep, time
import datetime
from picamera import PiCamera

dir = "/home/pi/Pictures/"
location = "TVP_B5"
room = "Great_Ouse"
filename = location + "." + room + "." + datetime.datetime.now().strftime("%Y%m%d.%H%M") + ".jpg"  

camera = PiCamera()
camera.resolution = (1024, 768)
camera.start_preview()
sleep(2) # Camera warm-up time
camera.capture(dir + filename)