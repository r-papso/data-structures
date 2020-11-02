namespace Structures.Tree
{
    public interface ISaveable
    {
        public string ToCsv(string delimiter = ",");

        public void FromCsv(string csv, string delimiter = ",");
    }
}
