#!/bin/bash

#Added into  

IoTHubConnectionString='HostName=team2.azure-devices.net;DeviceId=team2rpi3;SharedAccessKey=Li6yQbKUJfKPcb9tQ7pY4nJUq67VAVQePHOofYIdSK4='
dir='/home/pi/iot-hub-python-raspberrypi-client-app'

/usr/bin/env python $dir/app.py $IoTHubConnectionString