import ftplib
import keyring
import pathlib


def removeRecursively(ftp: ftplib.FTP, remotePath: pathlib.PurePath):
    """
    Remove a folder and all its contents from a FTP server
    """

    def removeFile(remoteFile):
        print(f"DELETING FILE {remoteFile}")
        ftp.delete(str(remoteFile))

    def removeFolder(remoteFolder):
        print(f"DELETING FOLDER {remoteFolder}/")
        ftp.rmd(str(remoteFolder))

    for (name, properties) in ftp.mlsd(remotePath):
        fullpath = remotePath.joinpath(name)
        if name == '.' or name == '..':
            continue
        elif properties['type'] == 'file':
            removeFile(fullpath)
        elif properties['type'] == 'dir':
            removeRecursively(ftp, fullpath)

    removeFolder(remotePath)


def uploadRecursively(ftp: ftplib.FTP, remoteBase: pathlib.PurePath, localBase: pathlib.PurePath):
    """
    Upload a local folder to a remote path on a FTP server
    """

    def remoteFromLocal(localPath: pathlib.PurePath):
        pathParts = localPath.parts[len(localBase.parts):]
        return remoteBase.joinpath(*pathParts)

    def uploadFile(localFile: pathlib.PurePath):
        remoteFilePath = remoteFromLocal(localFile)
        print(f"UPLOADING FILE {remoteFilePath}")
        with open(localFile, 'rb') as localBinary:
            ftp.storbinary(f"STOR {remoteFilePath}", localBinary)

    def createFolder(localFolder: pathlib.PurePath):
        remoteFolderPath = remoteFromLocal(localFolder)
        print(f"CREATING FOLDER {remoteFolderPath}/")
        ftp.mkd(str(remoteFolderPath))

    createFolder(localBase)
    for localFolder in [x for x in localBase.glob("**/*") if x.is_dir()]:
        createFolder(localFolder)
    for localFile in [x for x in localBase.glob("**/*") if not x.is_dir()]:
        uploadFile(localFile)


if __name__ == "__main__":

    hostname = "swharden.com"
    username = "swhftp@swharden.com"
    password = keyring.get_password("system", username)

    remotePath = pathlib.PurePosixPath('/qrss/plus-experimental')
    localPath = pathlib.Path(__file__).parent.joinpath("build")

    with ftplib.FTP_TLS(hostname, username, password) as ftps:
        removeRecursively(ftps, remotePath)
        uploadRecursively(ftps, remotePath, localPath)
