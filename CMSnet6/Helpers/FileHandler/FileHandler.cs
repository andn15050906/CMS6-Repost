namespace CMSnet6.Helpers.FileHandler
{
    public class FileHandler
    {
        private const char SEPARATOR = ':';

        /// <summary>
        /// Add a GUID to filename, separated by a '_'
        /// Client process FileName -> RetrieveFileName(string guidName) => guidName.Substring(0, guidName.LastIndexOf('_'));
        /// </summary>
        public string? SaveFile(IFormFile file, UploadDir dir)
        {
            if (file == null)
                return null;

            string guidName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string path = @$"{GetDir(dir)}\{guidName}";
            using (var stream = File.Create(path))
            {
                //Log.Information("Saving " + path);
                file.CopyTo(stream);
            }
            return guidName;
        }

        public string? SaveFiles(List<IFormFile> files, UploadDir dir)
        {
            if (files == null || files.Count == 0)
                return null;
            string concated = "";
            int i;
            for (i = 0; i < files.Count - 1; i++)
                concated += SaveFile(files[i], dir) + SEPARATOR;
            concated += SaveFile(files[files.Count - 1], dir);
            return concated;
        }

        public string[]? GetFiles(string guidNames) => guidNames?.Split(SEPARATOR);

        public void DeleteFile(string guidName, UploadDir dir)
        {
            if (guidName == null)
                return;

            string path = @$"{GetDir(dir)}\{guidName}";
            try
            {
                //Log.Information("Deleting " + path);
                File.Delete(path);
            }
            catch (Exception e)
            {
                //Log.Warning(e.Message);
            }
        }

        public void DeleteFiles(string guidNames, UploadDir dir)
        {
            if (guidNames == null)
                return;

            string[] paths = guidNames.Split(SEPARATOR);
            foreach (string path in paths)
                DeleteFile(path, dir);
        }






        private static string? GetDir(UploadDir dir) {
            return dir switch
            {
                UploadDir.Post => @"wwwroot\upload-post",
                UploadDir.Comment => @"wwwroot\upload-comment",
                _ => null
            };
        }
    }
}
