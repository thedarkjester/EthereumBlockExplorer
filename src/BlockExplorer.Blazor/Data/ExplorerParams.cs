using System;

namespace BlockExplorer.Blazor.Data
{
    public class ExplorerParams
    {
        public long Start { get; set; }
        public long End { get; set; }
        public long PreceedingBlocks { get; set; }
    }
}
