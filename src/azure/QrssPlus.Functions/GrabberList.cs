using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QrssPlus.Functions
{
    public class GrabberList : IEnumerable<GrabberInfo>
    {
        private readonly List<GrabberInfo> Grabbers = new();

        /// <summary>
        /// Add the grabber to the list if it's not already there
        /// </summary>
        public void Add(GrabberInfo grabber)
        {
            if (!Grabbers.Where(x => x.ID == grabber.ID).Any())
                Grabbers.Add(grabber);
        }

        public void Remove(GrabberInfo grabber)
        {
            Grabbers.Remove(grabber);
        }

        public IEnumerator<GrabberInfo> GetEnumerator()
        {
            foreach (var grabber in Grabbers)
                yield return grabber;
        }

        IEnumerator IEnumerable.GetEnumerator() => Grabbers.GetEnumerator();

        public bool Contains(string id) => Grabbers.Where(x => x.ID == id).Any();

        public int Count { get => Grabbers.Count; }
    }
}
