namespace Structures.Tree
{
    public interface ISaveable
    {
        public string ToCsv(string delimiter = ",");

        public void ToCsvFile(string filePath, string delimiter = ",");

        public void FromCsv(string csv, string delimiter = ",");

        public void FromCsvFile(string filePath, string delimiter = ",");
    }
}
