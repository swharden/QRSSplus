from ftplib import FTP_TLS
from pathlib import Path
from os.path import basename, dirname, abspath, join
from os import walk

#TODO: replace os calls with pathlib calls

def getCredentials(defaultUser):
    """Ask the user for user/pass using a asterisked textboxes."""
    import tkinter
    root = tkinter.Tk()
    root.eval('tk::PlaceWindow . center')
    root.title('Login')
    uv = tkinter.StringVar(root, value=defaultUser)
    pv = tkinter.StringVar(root, value='')
    userEntry = tkinter.Entry(root, bd=3, width=35, textvariable=uv)
    passEntry = tkinter.Entry(root, bd=3, width=35, show="*", textvariable=pv)
    btnClose = tkinter.Button(root, text="OK", command=root.destroy)
    userEntry.pack(padx=10, pady=5)
    passEntry.pack(padx=10, pady=5)
    btnClose.pack(padx=10, pady=5, side=tkinter.TOP, anchor=tkinter.NE)
    root.mainloop()
    return [uv.get(), pv.get()]


def removeRecursively(ftp, path):
    """recursively remove the ftp target path"""
    for (name, properties) in ftp.mlsd(path=path):
        if name in ['.', '..']:
            continue
        elif properties['type'] == 'file':
            filePath = f"{path}/{name}"
            print(f"DELETING {filePath}")
            ftp.delete(filePath)
        elif properties['type'] == 'dir':
            removeRecursively(ftp, f"{path}/{name}")

    print(f"DELETING {path}/")
    ftp.rmd(path)


def uploadRecursively(ftp, localFolderPath, remoteFolderPath, overwrite=True):
    """Copy a local folder tree to a remote target path."""

    # ensure the target doesn't exist (or delete it)
    if remoteFolderPath in ftp.nlst(dirname(remoteFolderPath)):
        if overwrite:
            print()
            print("### DELETING OLD FOLDER ###")
            removeRecursively(ftp, targetFolder)
        else:
            raise Exception("target already exists")

    print()
    print("### UPLOADING LOCAL FOLDER ###")
    ftp.mkd(remoteFolderPath)
    for root, dirs, files in walk(localFolderPath):
        for dir in dirs:
            localPath = join(root, dir)
            remotePath = remoteFolderPath + \
                localPath.replace(localFolderPath, "").replace("\\", "/")
            print("CREATE " + remotePath)
            ftp.mkd(remotePath)
        for filename in files:
            localPath = join(root, filename)
            remotePath = remoteFolderPath + \
                localPath.replace(localFolderPath, "").replace("\\", "/")
            ftpCommand = f'STOR {remotePath}'
            print(ftpCommand)
            with open(localPath, 'rb') as localBinary:
                ftp.storbinary(ftpCommand, localBinary)


if __name__ == "__main__":
    username, password = getCredentials("swhftp@swharden.com")
    hostname = "swharden.com"

    # predetermine paths (local and remote)
    targetFolder = '/qrss/plus-experimental'
    thisFolder = dirname(__file__)
    relativeLocalTarget = './build'
    localTarget = join(thisFolder, relativeLocalTarget)
    localTarget = abspath(localTarget)

    # connect and upload
    with FTP_TLS(hostname, username, password) as ftp:
        uploadRecursively(ftp, localTarget, targetFolder)

    print("DONE")
