#!/usr/bin/env python

from time import sleep, time
import datetime
import io
import os

from picamera import PiCamera

from azure.storage.blob import (
    BlockBlobService, 
    ContentSettings,
)

dir = "/home/pi/Pictures"

account_name    = 'team2iotfuncstorage'
account_key     = 'BW8wHjQ6Uk0Xope9PayashhK3hr7wdpsBiJkkf5p6BgkPafQAjyJFH1ivitH9nMLH+XwCN346/xWB2Pp2nzi7Q=='


location    = "TVP_B5"
room        = "Great_Ouse"
frequency   = 5

# Don't run at weekends

if datetime.date.today().isoweekday() > 5:
    print "Weekend. Exiting..."
    exit()

# Don't run before 09:00 or after 17:00

now = datetime.datetime.now()
start_of_day = now.replace(hour=9, minute=0, second=0, microsecond=0)
end_of_day = now.replace(hour=17, minute=0, second=0, microsecond=0) 

if now < start_of_day or now > end_of_day:
    print "Outside of core hours. Exiting..."
    exit()

# Allow the script to be called every minute by cron, but only take a picture if it meets the frequency
# We will base it on the minutes since the start of the hour

if now.minute % frequency != 0:
    print "Frequency criteria not met.  Exiting..."
    exit()

# Take picture and store in /home/pi/Pictures

timestamp = now.strftime("%Y%m%d%H%M")
filename = location + "." + room + "." + timestamp + ".jpg"  
lfilename = dir + '/' + filename
camera = PiCamera()
camera.resolution = (1024, 768)
camera.start_preview()
sleep(2) # Camera warm-up time
camera.capture(lfilename)

# Upload to blob service

azureblob = BlockBlobService(
    account_name    = account_name,
    account_key     = account_key,
    )

azureblob.create_container('images')

azureblob.create_blob_from_path(
    'images', 
    filename, 
    lfilename, 
    content_settings=ContentSettings(content_type='image/png')
    )

print filename + " uploaded to container https://" + account_name + ".core.windows.net/images/." 