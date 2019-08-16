using System.Collections.Generic;

namespace IndustrialPark
{
    public class LinkClipboard
    {
        public Endianness endianness;
        public List<byte[]> links;

        public LinkClipboard(Endianness endianness, List<byte[]> links)
        {
            this.endianness = endianness;
            this.links = links;
        }
    }
}