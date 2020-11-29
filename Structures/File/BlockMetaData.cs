namespace Structures.File
{
    internal class BlockMetaData
    {
        public bool IsValid { get; set; }

        public long Address { get; set; }

        public int ValidDataCount { get; set; }

        public int Depth { get; set; }
    }
}
