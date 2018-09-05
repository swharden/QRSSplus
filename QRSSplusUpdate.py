"""
QRSS Plus - Automatic QRSS Grabber by Scott Harden
This script is intended to be run on a web server every 10 minutes.
It was developed for Python 2.7 (on Linux through a SSH terminal)
It was last tested/verified to work on Python-2.7.10 (WinPython)
"""

import sys
PYTHON_VERSION = sys.version_info[0]
assert PYTHON_VERSION == 2

import os
import csv
import hashlib
import time
import glob
import subprocess
import urllib2
import traceback


def loadGrabbers(fname="grabbers.csv"):
    """
    Read a CSV file and return contents as a list of dictionaries.
    """
    columns = ["ID", "call", "title", "name", "loc", "site", "url"]
    grabbers = []
    with open(fname) as f:
        reader = csv.reader(f, delimiter=",", quotechar='"')
        for row in reader:
            row = [x.strip() for x in row]
            row = [x.strip('"') for x in row]
            if len(row) < 5 or row[0].startswith("#"):
                continue
            grabber = {}
            for i, key in enumerate(columns):
                grabber[key] = row[i]
            grabbers.append(grabber)
    return grabbers


class QrssPlus:

    def __init__(self, configFile="grabbers.csv", downloadFolder="data/"):
        self.timeStart = time.time()
        self.logLines = []
        assert os.path.exists(configFile)
        self.configFile = configFile
        assert os.path.exists(downloadFolder)
        self.downloadFolder = downloadFolder

        self.loadGrabbers()
        self.deleteOld()
        self.scanGrabFolder()

    def log(self, msg=""):
        """call this instead of calling print()."""
        if len(msg):
            msg = "[%.03fs] %s" % (time.time()-self.timeStart, msg)
        print(msg)
        self.logLines.append(msg)

    def timeCode(self):
        """return YYMMDDHHMM timecode of now."""
        timeStamp = time.strftime("%y%m%d%H%M", time.localtime())
        timeInt = int(timeStamp)
        timeCode = str(timeInt-timeInt % 10)
        return timeCode

    def loadGrabbers(self):
        """load a list of known grabbers from the database."""
        self.grabbers = loadGrabbers()
        self.log("loaded %d grabbers from %s" % (len(self.grabbers),
                                                 self.configFile))

    def scanGrabFolder(self):
        """update list of known grabber images."""
        fnames = sorted(os.listdir(self.downloadFolder))
        self.seenIDs, self.seenTimes, self.seenHashes = [], [], []
        for fname in fnames:
            fname = fname.split(".")
            if len(fname) != 4:
                continue
            self.seenIDs.append(fname[0])
            self.seenTimes.append(int(fname[1]))
            self.seenHashes.append(fname[2])

    def minutesSinceLastUpdate(self):
        """determine how long it's been since we last ran."""
        if self.seenTimes == []:
            return 0
        latestTime = max(self.seenTimes)
        return int(self.timeCode())-int(latestTime)

    def hashOfTempFile(self):
        """return the md5 hash of a file."""
        if os.path.exists("temp.dat"):
            with open("temp.dat", 'r') as f:
                data = f.read()
            return hashlib.md5(data).hexdigest()
        else:
            return "noTempFile"

    def downloadTempGrab(self, url):
        """download a url to temp.dat using wget"""
        if os.path.exists("temp.dat"):
            os.remove("temp.dat")
        cmd = "wget -q -T 3 -t 1"  # 1 attempt (no retries)
        cmd += " -O %s %s" % ("temp.dat", url)
        self.log(cmd)
        process = subprocess.Popen(cmd, shell=True, stdout=subprocess.PIPE)
        process.wait()

    def renameTempGrab(self, url, tag):
        """rename temp.dat based on the url, date, tag, and hash."""
        hsh = self.hashOfTempFile()
        ext = url.split(".")[-1]
        fname = "%s.%s.%s.%s" % (tag, self.timeCode(), hsh[:10], ext)
        if os.path.exists("temp.dat"):
            self.log("renaming temp file to: "+fname)
            os.rename("temp.dat", "data/"+fname)
        return fname, hsh, ext

    def downloadGrab(self, url, tag):
        """
        Download an image and save it with time and hash in the filename.
        Many web hosts block odd clients, so look like wget and you'll do better.
        """
        self.log("downloading: "+url)
        try:
            self.downloadTempGrab(url)
            fname, hsh, ext = self.renameTempGrab(url, tag)
            if hsh in self.seenHashes:
                self.log("DUPLICATE!")
                return "DUP"
            else:
                self.thumbnail(fname, fname.replace("."+ext, ".thumb."+ext))
                return "OK"
        except Exception as e:
            traceback.print_exc()
            fname = "%s.%s.fail.fail" % (tag, self.timeCode())
            with open(self.downloadFolder+fname, 'w') as f:
                f.write("fail")
            return "FAIL (%s)" % str(e)

    def downloadAll(self, force=False):
        """Download the latest image from every grabber."""
        if self.minutesSinceLastUpdate() == 0 and force == False:
            self.log("TOO SOON SINCE LAST DOWNLOAD!")
            return
        for grabber in self.grabbers:
            self.downloadGrab(grabber["url"], grabber["ID"])+"\n"

    def deleteOld(self, maxAgeMinutes=60*4):
        """Download all data files older than a certain age."""
        fnames = sorted(glob.glob(self.downloadFolder+"/*.*"))
        for fname in fnames:
            bn = os.path.basename(fname).split(".")
            if len(bn) < 4:
                continue
            age = int(self.timeCode())-int(bn[1])
            if age > maxAgeMinutes:
                self.log("deleting old", fname)
                os.remove(fname)

    def thumbnail(self, fnameIn, fnameOut):
        """given the filename of an original image, create its thumbnail."""
        cmd = "convert -define jpeg:size=500x150 "
        cmd += '"%s" ' % os.path.join(self.downloadFolder, fnameIn)
        cmd += "-auto-orient -thumbnail 250x150 "
        cmd += '"%s" ' % os.path.join(self.downloadFolder, fnameOut)
        self.log("creating thumbnail ...")
        self.log(cmd)
        process = subprocess.Popen(cmd, shell=True, stdout=subprocess.PIPE)
        process.wait()

    def updateGrabberListFromGitHub(self):
        """Update the local grabbers.csv by getting the latest one from GitHub."""
        url = "https://raw.githubusercontent.com/swharden/QRSSplus/master/grabbers.csv"
        headers = {'User-Agent': 'Wget/1.12 (linux-gnu)'}
        req = urllib2.Request(url, headers=headers)
        r = urllib2.urlopen(req, timeout=3)
        raw = r.read()
        raw = raw.split("\n")
        raw = [x.strip() for x in raw]
        raw = [x for x in raw if len(x)]
        raw = "\n".join(raw)
        f = open("grabbers.csv", 'w')
        f.write(raw)
        f.close()
        self.log("Downloaded the latest grabbers.csv")


if __name__ == "__main__":
    qp = QrssPlus()
    qp.updateGrabberListFromGitHub()
    qp.downloadAll(True)
    qp.deleteOld()
    with open("data/status.txt", 'w') as f:
        f.write("<br>\n".join(qp.logLines))
