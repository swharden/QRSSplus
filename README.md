
# QRSS Plus

**QRSS Plus is an automatically-updating QRSS grabber website designed to fully operate on a web server.** QRSS Plus watches a list of image URLs (stored in [grabbers.csv](grabbers.csv)), downloading them every 10 minutes, and marking them as "active" if their content changes regularly. QRSS Plus displays a list of active grabbers on a web page making them easy to browse at a glance. 

**View QRSS Plus in action:**\
http://www.SWHarden.com/qrss/plus/

![](/misc/QRSSplus.png)


## QRSS Grabber List

**The most recent list of active QRSS Grabbers used by QRSS Plus is here:**
https://raw.githubusercontent.com/swharden/QRSSplus/master/grabbers.csv

This master grabber list is downloaded every 10 minutes by QRSS Plus, so updating this GitHub page immediately updates the live QRSS Plus webpage. If you design your own QRSS Grabber website or program, you are welcome to use (and automatically download) this master grabber list as well.

## Updating the Grabber List

### Request a Change
**If you would like to update a grabber listing, take one of these actions:**

* E-mail Andy (G0FTD) punkbiscuit@googlemail.com with information about the grabber
* Post a message to the [Knights QRSS Mailing List](https://groups.io/g/qrssknights) requesting the change

### A Note for DropBox Users
Many people upload grabs automatically using a DropBox account. The URLs used to access these files require special attention. If the URL is incorrectly formatted, it will return a _web page_ displaying a file. When the URL is properly formatted, it will return the _file_ itself. It is critically important that the URL given to QRSS Plus is the URL for the file, not for the webpage. To do this, simply replace `www.dropbox.com` with `dl.dropboxusercontent.com` in the URL:

* `https://www.dropbox.com/s/35m4m8wn4w5hi7r/HF.jpg` **<- bad**
* `http://dl.dropboxusercontent.com/s/35m4m8wn4w5hi7r/HF.jpg` **<- good**

### Become a Collaborator
If you are a dedicated supporter of the QRSS community, consider becoming a collaborator so you can make direct changes to [grabbers.csv](grabbers.csv) which take effect immediately! You can then also make changes on behalf of others (e.g., monitoring a QRSS mailing lists and adding new grabbers as you learn of them). I encourage any tech-savvy QRSS enthusiast to consider becoming a QRSS Plus curator!

## Creating Your Own QRSS Plus Website

Although the author has run QRSS Plus on his website for several years and intends to continue doing so, **anybody can set-up their own QRSS Plus webpage** using the tools in this repository. Just copy this folder to your web server and call [QRSSplusUpdate.py](QRSSplusUpdate.py) every 10 minutes. Those who decide to independently set-up their own QRSS Plus page are encouraged to improve the code, add new features to the web interface, or incorporating new functionality, as these improvements enhance the QRSS community at large.

A separate page is provided reviewing [technical details](/misc/technical.md) of operation,  configuration, and design considerations.

## Additional Resources
* [Knights QRSS Mailing List](https://groups.io/g/qrssknights) - The QRSS Knights is an extremely active community of QRSS enthusiasts. Join their mailing list to get in on discussions about new grabbers, radio spots, equipment, reception techniques, cosmic and atmospheric anomalies, and efforts in the QRSS world.
* [Knights QRSS Blog](http://knightsqrss.blogspot.com/) - Updated less frequently, but worth noting
* [Getting Started with QRSS](http://knightsqrss.blogspot.com/2010/01/getting-started-with-qrss.html) - A good guide for those new to QRSS
* [Carpe QRSS](https://github.com/strickyak/carpe-qrss) - A QRSS grabber built on the GO language
* [LOPORA](http://www.qsl.net/pa2ohh/11lop.htm) - QRSS reception program in python
* [QRP-Labs](https://www.qrp-labs.com/) - QRSS transmitter kits by [Hans Summers](http://www.hanssummers.com)
* [QrssPiG](https://gitlab.com/hb9fxx/qrsspig) - QRSS Grabber for Raspberry Pi
