using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrssPlus
{
    public class GrabberList
    {
        private readonly List<GrabberInfo> Grabbers = new();
        public int Count => Grabbers.Count;
        public string[] GrabberIDs => Grabbers.Select(x => x.ID).ToArray();
        public override string ToString() => $"List of {Count} unique grabbers";

        public void Add(GrabberInfo gi)
        {
            if (GrabberIDs.Contains(gi.ID))
                throw new ArgumentException("Grabber IDs must be unique");

            Grabbers.Add(gi);
        }

        // TODO: deep copy? Allow mutable? Record?
        public GrabberInfo[] GetInfos() => Grabbers.ToArray();
    }
}
