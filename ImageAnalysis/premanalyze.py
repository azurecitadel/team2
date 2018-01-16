#
# Hacked from https://docs.microsoft.com/en-us/azure/cognitive-services/Computer-vision/quickstarts/python 
# Python 2.7
#
# Takes a local file and analyses for the presence of people using the Cognitive Services vision API
#
# Mike Ormond 15/1/18
#
# 1/ Set the subscription key
# 2/ Change the API endpoint (if required - must match the location of the subscription key)
# 3/ Set the path to the file to be analysed
#

import requests, base64, json

headers = {
    # Request headers.
    'Content-Type': 'application/octet-stream',

    # NOTE: Replace the "Ocp-Apim-Subscription-Key" value with a valid subscription key.
    'Ocp-Apim-Subscription-Key': '',
}

params = {}
    # Request parameters. All of them are optional.
#    'visualFeatures': 'Categories',
#    'details': 'Celebrities',
#    'language': 'en',
#}

# Replace the three dots below with the full file path to a JPEG image of a celebrity on your computer or network.
image = open('/mnt/c/Users/username/filename.jpg','rb').read() # Read image file in binary mode

try:
    # NOTE: You must use the same location in your REST call as you used to obtain your subscription keys.
    #   For example, if you obtained your subscription keys from westus, replace "westcentralus" in the 
    #   URL below with "westus".
    response = requests.post(url = 'https://northeurope.api.cognitive.microsoft.com/vision/v1.0/tag',
                             headers = headers,
                             params = params,
                             data = image)

    data = response.json()

    personProbability = 0

    for x in range(0, len(data["tags"])):
      if (data["tags"][x]['name'] == 'person'):
        personProbability = data["tags"][x]['confidence']
        break

    print(personProbability)

except Exception as e:
    print("{0}".format(e))

