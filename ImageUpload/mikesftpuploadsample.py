#
# Python 2.7
#
# Walks all the files in cameraPath and uploads to Azure blob storage if they are over a certain size.
# Files are also archived locally to archivePath
# The size criteria is a very simplistic way to exclude images with no content (as the jpg compression 
# results in a very small file size)
#
# Mike Ormond 15/1/18
#
# 1/ Set Azure blob storage account name (azureAccountName)
# 2/ Set account key (azureAccountKey)
#

import os
import errno
import azure.storage
from azure.storage.blob import BlockBlobService
from azure.storage.blob import ContentSettings

azureAccountName = ""
azureAccountKey = ""
cameraPath = "/pi/FTP/Camera/"
archivePath = "/pi/Archive/Camera/"
jpgContentType = "img/jpg"

def walkFilesInDirectory(path):
        flatFiles = []
        for root, dirs, files in os.walk(path):
                for name in files:
                        flatFiles.append([root, name])
                        #print os.path.join(root, name)
        return flatFiles

def archiveFile(filepath, archivepath, filename):
        print filepath + " : " + archivepath + " : " + filename
        if (not os.path.exists(archivepath)):
                os.makedirs(archivepath)
        os.rename(filepath, os.path.join(archivepath, filename))

def uploadToAzure(files):
        block_blob_service = BlockBlobService(account_name = azureAccountName, account_key = azureAccountKey)

        totalFiles = str(len(files))
        fileCount = 0

        for file in files:
                fileCount += 1
                print "Processing: " + file[1] + " (" + str(fileCount) + "/" + totalFiles + ")"
                filepath = os.path.join(file[0], file[1])
                archivepath = file[0].replace(cameraPath, archivePath)
                if (filepath[-4:] == ".jpg") and (os.path.getsize(filepath) > 10485):
                        try:
                                containerName = file[0][20:28]
                                block_blob_service.create_container(containerName)
                                block_blob_service.create_blob_from_path(
                                        container_name = containerName,         # Extract the date
                                        blob_name = file[1],                    # filename
                                        file_path = filepath,
                                        content_settings = ContentSettings(content_type = jpgContentType)
                                        )
                                try:
                                        archiveFile(filepath, archivepath, file[1])
                                except OSError as e:
                                        print e.errno
                        except:
                                pass
                else:
                        print "Incorrect extension or file size below threshold of 10MB. Archiving."
                        archiveFile(filepath, archivepath, file[1])

files = walkFilesInDirectory("/home/pi/FTP/Camera/")

print "Found " + str(len(files)) + " to process..."

#if (len(files) < 50 )
# 5mins * 60s / 5s capture period * margin factor
        #Alert someone

uploadToAzure(files)

print "success"
