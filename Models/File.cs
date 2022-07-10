using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smartway_Test.Models
{
    public class File
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Size { get; set; }
        public string Format { get; set; }
        public byte[] Content { get; set; }

        public List<Link> Links { get; set; }
    }

    public class Link
    {
        public int Id { get; set; }
        public string Hash { get; set; }
        public int FileId { get; set; }

        public File File { get; set; }
    }
}
