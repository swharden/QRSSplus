namespace QrssPlus
{
    /// <summary>
    /// Information and latest grab for a single grabber
    /// </summary>
    public class Grabber
    {
        public readonly GrabberInfo Info;
        public readonly Grab Grab;

        public Grabber(GrabberInfo info)
        {
            Info = info;
            Grab = new Grab(info);
        }

        public override string ToString() =>
            string.IsNullOrEmpty(Grab.Hash)
                ? $"{Info.ID} [not downloaded]"
                : $"{Info.ID} [{Grab.Hash}]";

        public string GetFileName() => $"{Info.ID} {Grab.DownloadTimeCode} {Grab.Hash}{System.IO.Path.GetExtension(Info.ImageUrl)}";
    }
}
