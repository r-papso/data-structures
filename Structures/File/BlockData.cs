namespace Structures.File
{
    internal class BlockData
    {
        public int Address { get; set; }

        public int ValidDataCount { get; set; }

        public BlockData NextBlock { get; set; }
    }
}
