using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace QrssPlus
{
    /// <summary>
    /// This list of grabbers contains every grabber's information and latest grab.
    /// It performs error checking to ensure duplicates do not exist.
    /// </summary>
    public class GrabberList : IEnumerable<Grabber>
    {
        private readonly List<Grabber> Grabbers = new();
        public int Count => Grabbers.Count;
        public string[] GrabberIDs => Grabbers.Select(x => x.Info.ID).ToArray();
        public override string ToString() => $"List of {Count} unique grabbers";

        public void Add(Grabber gi)
        {
            if (GrabberIDs.Contains(gi.Info.ID))
                throw new ArgumentException("Grabber IDs must be unique");

            Grabbers.Add(gi);
        }

        public IEnumerator<Grabber> GetEnumerator()
        {
            foreach (var grab in Grabbers)
                yield return grab;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
