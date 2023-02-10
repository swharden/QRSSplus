
# QRSS Plus

[![Backend CI/CD](https://github.com/swharden/QRSSplus/actions/workflows/backend.yml/badge.svg)](https://github.com/swharden/QRSSplus/actions/workflows/backend.yml)
[![Frontend CI/CD](https://github.com/swharden/QRSSplus/actions/workflows/frontend.yml/badge.svg)](https://github.com/swharden/QRSSplus/actions/workflows/frontend.yml)
[![Validate Grabbers List](https://github.com/swharden/QRSSplus/actions/workflows/grabbers.yml/badge.svg)](https://github.com/swharden/QRSSplus/actions/workflows/grabbers.yml)

**[QRSS Plus](https://www.swharden.com/qrss/plus) is an automatically-updating website that lists active QRSS grabbers around the world.** Every 10 minutes grabber URLs are read from [grabbers.csv](grabbers.csv), the latest grabs are downloaded and analyzed, and only grabbers whose spectrogram images changed recently are marked as "active" on the website.

* **Launch QRSS Plus:** **https://www.swharden.com/qrss/plus**

## QRSS Grabber List

**This list of grabbers is actively maintained by Andy (G0FTD)** and serves as the primary source QRSS Plus uses to get information about grabbers. It is downloaded automatically every 10 minutes.

* [**grabbers.csv**](grabbers.csv) ([raw](https://raw.githubusercontent.com/swharden/QRSSplus/master/grabbers.csv))

## Request a Change

**To submit or modify a grabber listing,** E-mail Andy (G0FTD) punkbiscuit@googlemail.com or post a message to the [Knights QRSS Mailing List](https://groups.io/g/qrssknights) and provide the latest grabber information:

* Callsign
* Location
* URL to the grabber image
* URL to a personal website (optional)

⚠️ **Please do not submit non-functional URLs!** Test URLs on your own computer to ensure they function as expected before submitting them.

⚠️ **Dropbox users** must ensure their URL returns an _image file_, not a _web page_ that displays an image. To fix this, replace `www.dropbox.com` with `dl.dropboxusercontent.com` as shown here:


```
 Bad URL: https://www.dropbox.com/s/35m4m8wn4w5hi7r/HF.jpg
Good URL: http://dl.dropboxusercontent.com/s/35m4m8wn4w5hi7r/HF.jpg
```

If the URL contains a `?`, delete it and all characters following it:

```
 Bad URL: https://www.dropbox.com/s/35m4m8wn4w5hi7r/HF.jpg?dl=0
Good URL: https://www.dropbox.com/s/35m4m8wn4w5hi7r/HF.jpg
```

## Additional Resources
* [Knights QRSS Mailing List](https://groups.io/g/qrssknights) - The QRSS Knights is an extremely active community of QRSS enthusiasts. Join their mailing list to get in on discussions about new grabbers, radio spots, equipment, reception techniques, cosmic and atmospheric anomalies, and efforts in the QRSS world.
* [74!
The Knights QRSS Winter Newsletter](https://swharden.com/qrss/74) - Updated every December by Andy (G0FTD)
* [The New Age of QRSS](https://swharden.com/blog/2020-10-03-new-age-of-qrss/) - A modern introduction to QRSS
* [Knights QRSS Blog](http://knightsqrss.blogspot.com/) - Updated less frequently, but worth noting
* [Getting Started with QRSS](http://knightsqrss.blogspot.com/2010/01/getting-started-with-qrss.html) - A good guide for those new to QRSS
* [Carpe QRSS](https://github.com/strickyak/carpe-qrss) - A QRSS grabber built on the GO language
* [LOPORA](http://www.qsl.net/pa2ohh/11lop.htm) - QRSS reception program in python
* [QRP-Labs](https://www.qrp-labs.com/) - QRSS transmitter kits by [Hans Summers](http://www.hanssummers.com)
* [QrssPiG](https://gitlab.com/hb9fxx/qrsspig) - QRSS Grabber for Raspberry Pi
