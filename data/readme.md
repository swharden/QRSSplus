# Demonstration Data

In practice (deployment), the contents of this folder should be empty and the QRSS Plus python script will populate it properly upon its first run.

During development I find it helpful to have a full folder (and a time-matched grabbers file) to practice coding against. At the time of this writing, my actual data folder is 200 MB. To keep sizes reasonable I converted all images in this folder to JPG with the lowest quality possible. This is the batch script I used (with ImageMagick) to automate the conversion:

```bash
@ECHO OFF
setlocal enabledelayedexpansion
for %%f in (data\*.jpg) do (
  echo %%~nf
  convert data\%%~nf.jpg -quality 1 data2\%%~nf.jpg
)
```
