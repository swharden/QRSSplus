using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace QrssPlus
{
    public class GrabberList : IEnumerable<GrabberInfo>
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

        public IEnumerator<GrabberInfo> GetEnumerator()
        {
            foreach (var grab in Grabbers)
                yield return grab;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
