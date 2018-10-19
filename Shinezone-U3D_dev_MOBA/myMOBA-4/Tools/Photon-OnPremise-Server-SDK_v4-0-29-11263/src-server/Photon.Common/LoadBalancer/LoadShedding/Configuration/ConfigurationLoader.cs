using System;
using System.Xml;

namespace Photon.Common.LoadBalancer.LoadShedding.Configuration
{
    internal class ConfigurationLoader 
    {
        public static bool TryLoadFromFile(string fileName, out FeedbackControlSystemSection section, out string message)
        {
            section = null;
            message = string.Empty;

            try
            {
                section = LoadFromFile(fileName);
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public static FeedbackControlSystemSection LoadFromFile(string fileName)
        {
            using (var fileStream = System.IO.File.Open(fileName, System.IO.FileMode.Open))
            {
                var xmlReader = XmlReader.Create(fileStream);
                xmlReader.MoveToContent();

                var graphSection = new FeedbackControlSystemSection();
                graphSection.Deserialize(xmlReader, false);

                fileStream.Close();

                return graphSection;
            }
        }
    }
}
