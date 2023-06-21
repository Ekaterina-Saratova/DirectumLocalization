namespace Repository
{
    public static class CommonHelper
    {
        public static void ValidateFilePath(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл не найден.", filePath);
            }
        }
    }
}
