There are two functions for determining room occupancy:

__ProcessRoomImage__ monitors blob storage and will be triggered when a new image is uploaded. It uses the cognitive services computer vision API to get a list of tags. It checks to determine if "person" is a tag associated with the image with a confidence higher than a set threshold. If so, the room is considered to be occupied. If not, unoccupied.The result is written to table storage using the "Room ID". The Room ID is simply the blob name minues the file extension so, for example it might be useful to name images Room1.jpg, Room.jpg or Clyde.jpg, Everest.jpg etc

__CheckIfRoomOccupied__ uses an http trigger with the name of the desired room passed as a parameter. It does a lookup on the table storage to find the last occupation status of the room. This is returned as a JSON payload in the http response along with a timestamp. We could probably do something clever around disregarding entries over a certain age.

ProcessRoomImage requires a CS computer vision API key.

Both functions have a dependency on a storage account to monitor for images (blob) and store the results (table). 

