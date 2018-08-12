
# QRSS Plus

**QRSS Plus is an automatically-updating QRSS grabber website designed to fully operate on a web server.** QRSS Plus watches a list of image URLs (stored in [grabbers.csv](grabbers.csv)), downloading them every 10 minutes, and marking them as "active" if their content changes regularly. QRSS Plus displays a list of active grabbers on a web page making them easy to browse at a glance. To add or modify grabber information in the grabber list, simply edit [grabbers.csv](grabbers.csv).

**View QRSS Plus in action:**\
http://www.SWHarden.com/qrss/plus/

![](QRSSplus.png)

Although the author has run QRSS Plus on his website for several years and intends to continue doing so, **anybody can set-up their own QRSS Plus webpage** using the tools in this repository. Just copy this folder to your web server and call [QRSSplusUpdate.py](QRSSplusUpdate.py) every 10 minutes. Those who decide to independently set-up their own QRSS Plus page are encouraged to improve the code, add new features to the web interface, or incorporating new functionality, as these improvements enhance the QRSS community at large.

A separate page is provided reviewing [technical details](technical.md) of operation,  configuration, and design considerations.

## QRSS Grabber List

**The most recent list of active QRSS Grabbers used by QRSS Plus is here:**
https://raw.githubusercontent.com/swharden/QRSSplus/master/grabbers.csv

This master grabber list is downloaded every 10 minutes by QRSS Plus, so updating this GitHub page immediately updates the live QRSS Plus webpage. If you design your own QRSS Grabber website or program, you are welcome to use (and automatically download) this master grabber list as well.

## Updating the Grabber List

**If you would like to update a grabber listing, take one of these actions:**

* E-mail one of the primary authors of this repository:
  * Scott Harden SWHarden@gmail.com
  * _collaborators can add their own email addresses here!_
* Create an issue on the [QRSS Plus issues page](https://github.com/swharden/QRSSplus/issues) and someone will get it it shortly
* Post a message to the [Knights QRSS Mailing List](https://groups.io/g/qrssknights) and ask that QRSS Plus be updated with your new information
* Make the change yourself to [grabbers.csv](grabbers.csv) and issue a pull request (understandably intimidating for first-time git users, but try it! You won't mess anything up.)
* **Become a collaborator for this repository so you can make direct changes to [grabbers.csv](grabbers.csv) which take effect immediately! You can also help make changes on behalf of others (e.g., monitoring a QRSS mailing lists and adding new grabbers as you learn of them). I encourage any tech-savvy QRSS enthusiast to consider becoming a QRSS Plus curator!**

## Additional Resources
* [Knights QRSS Mailing List](https://groups.io/g/qrssknights) - The QRSS Knights is an extremely active community of QRSS enthusiasts. Join their mailing list to get in on discussions about new grabbers, radio spots, equipment, reception techniques, cosmic and atmospheric anomalies, and efforts in the QRSS world.
* [Knights QRSS Blog](http://knightsqrss.blogspot.com/) - Updated less frequently, but worth noting
* [Getting Started with QRSS](http://knightsqrss.blogspot.com/2010/01/getting-started-with-qrss.html) - A good guide for those new to QRSS
* [Carpe QRSS](https://github.com/strickyak/carpe-qrss) - A QRSS grabber built on the GO language
* [LOPORA](http://www.qsl.net/pa2ohh/11lop.htm) - QRSS reception program in python
* [QRP-Labs](https://www.qrp-labs.com/) - QRSS transmitter kits by [Hans Summers](http://www.hanssummers.com)
* [QrssPiG](https://gitlab.com/hb9fxx/qrsspig) - QRSS Grabber for Raspberry Pi
