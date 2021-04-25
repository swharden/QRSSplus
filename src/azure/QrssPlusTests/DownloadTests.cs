using NUnit.Framework;
using System;

namespace QrssPlusTests
{
    public class Tests
    {
        [Test]
        public void Test1()
        {
            var dl = new QrssPlus.Downloader();
            dl.DownloadGrabberList(maxGrabberCount: 10);
            dl.DownloadGrabberImages();
        }
    }
}