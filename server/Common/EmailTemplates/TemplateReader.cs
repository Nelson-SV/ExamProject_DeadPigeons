namespace Common.EmailTemplates;

public class TemplateReader
{
        private readonly string _templateDirectory;

        public TemplateReader()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var serverDirectory = Path.GetFullPath(Path.Combine(currentDirectory, "..")); 

            _templateDirectory = Path.Combine(serverDirectory, "Common", "EmailTemplates");
            
            if (!Directory.Exists(_templateDirectory))
            {
                throw new DirectoryNotFoundException($"EmailTemplates directory not found at: {_templateDirectory}");
            }
        }

        public string LoadTemplate(string templateName)
        {
            var templatePath = Path.Combine(_templateDirectory, templateName);

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Template '{templateName}' not found at: {templatePath}");
            }

            return File.ReadAllText(templatePath);
        }
}
