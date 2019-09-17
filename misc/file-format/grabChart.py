import os
import glob

import matplotlib.pyplot as plt
from matplotlib.lines import Line2D
import numpy as np

class Grabber:
    def __init__(self, filePath, index):
        self.id = os.path.basename(filePath).split(".")[0]
        self.size = self.GetSize(filePath)
        self.index = index
        self.fileFormat = filePath.split(".")[-1].lower()

    def GetSize(self, filePath):
        fileSizeKb = os.stat(filePath).st_size/1000
        return fileSizeKb

    @property
    def color(self):
        if self.fileFormat == "jpg" or self.fileFormat == "jpeg":
            return "blue"
        elif self.fileFormat == "png":
            return "red"
        elif self.fileFormat == "gif":
            return "green"
        else:
            return orange


def chart_bar(grabbers):
    assert isinstance(grabbers, list)
    assert isinstance(grabbers[0], Grabber)

    plt.figure(figsize=(10, 5))
    for i, grabber in enumerate(grabbers):
        plt.bar(grabber.index, grabber.size, color=grabber.color)

    plt.xticks(range(len(grabbers)), [
               x.id for x in grabbers], rotation='vertical')

    plt.ylabel("Grab Size (kB)", fontsize=16)
    plt.title("Grabber File Sizes", fontsize=16)

    custom_lines = [Line2D([0], [0], color="red", lw=4),
                    Line2D([0], [0], color="blue", lw=4)]
    plt.legend(custom_lines, ['PNG', 'JPG'])

    plt.margins(.01, .1)
    plt.tight_layout()
    # plt.show()
    plt.savefig("bar.png")


def chart_pie(grabbers):
    assert isinstance(grabbers, list)
    assert isinstance(grabbers[0], Grabber)

    plt.figure(figsize=(10, 10))
    slices = plt.pie(
        [x.size for x in grabbers], 
        labels=[x.id for x in grabbers], 
        colors=[x.color for x in grabbers],
        explode=[.1 for x in grabbers],
        radius=.8
        )

    for texts in slices[1]:
        texts.set_size(8)

    custom_lines = [Line2D([0], [0], color="red", lw=4),
                    Line2D([0], [0], color="blue", lw=4)]
    plt.legend(custom_lines, ['PNG', 'JPG'])

    plt.title("Grabber File Size")
    plt.margins(.2, .2)
    plt.tight_layout()
    #plt.show()
    plt.savefig("pie.png")

if __name__ == "__main__":

    grabbers = []
    for filePath in glob.glob("grabs/*.*"):
        grabbers.append(Grabber(filePath, len(grabbers)))

    jpgGrabberSizes = [x.size for x in grabbers if x.color=="blue"]
    pngGrabberSizes = [x.size for x in grabbers if x.color=="red"]

    print("mean JPG size (of %d) is %.02f kB" % (len(jpgGrabberSizes), np.mean(jpgGrabberSizes)))
    print("mean PNG size (of %d) is %.02f kB" % (len(pngGrabberSizes), np.mean(pngGrabberSizes)))

    chart_bar(grabbers)
    chart_pie(grabbers)

    print("DONE")
