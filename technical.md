# Technical Details
The core of this project is a configuration file (containing the names and URLs of all grabbers), a python file (which downloads all the latest grabs), and a PHP script (which displays the recent grabs via a web interface). As long as the python script runs every 10 minutes, the website will always display the latest grabs from all grabbers, categorizing each as active or inactive respectively.

## Requirements
This project is intended to be light, as it is run on headless web servers and at best manually controlled through a SSH console. It was developed to require Python 2.7 (Python 3 is often not available on inexpensive web hosting plans), and any modern version of PHP will suffice. ImageMagick's [convert utility](https://www.imagemagick.org/script/convert.php) is assumed to be present (used for making thumbnails).

## Grabber List
Grabber information (image URL, website, operator call sign, etc) is stored in [grabbers.csv](grabbers.csv). This format was chosen because it is simple to edit on any computer, including through GitHub's web interface.

## Automatic Update of Grabber List
When the python script is run, the first thing it does is download the latest [grabbers.csv](grabbers.csv) from this GitHub page. It then uses this list of grabbers to download new images for each grabber, and saves the file so [index.php](index.php) always uses the latest call signs, names, and URLs data for each grabber. This also means that updating this 1 file on GitHub will immediately update everybody's grabber website that gets its grabber list from this file (not just QRSS Plus).

## Folder Structure
A download folder contains all downloaded files. Every file has a timestamp in epoch format and also an image hash code. New images are detected as new because its MD5 hash will be different. Every time [QRSSplusUpdate.py](QRSSplusUpdate.py) is run, all files with timestamps older than a certain amount (typically 3 hours) are deleted, and new grabber files are downloaded. If a grabber download fails, an empty file with a ".fail" extension is added to the download folder.

## Web Interface
The web interface is provided by [index.php](index.php) which simply displays all the grabbers listed in the configuration file next to all the relevant images found in the downloads folder. If no images are found in the downloads folder for a certain grabber, that grabber is marked as inactive.

## Detecting Problems
What if QRSS Plus stops downloading new grabs? Every time QRSS Plus is updated, the total duration the script took to download all grabs is saved in a text file (containing only the number of seconds as a floating point number). The PHP script can display how long the last download took by reading the content of this file, and it can determine if QRSS Plus stopped running by looking at the modification time of the file. If an issue is detected, PHP will raise an alarm message at the top of the page indicating the author should be contacted to resolve the issue.