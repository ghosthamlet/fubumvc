using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Http
{
    public class CurrentMimeType
    {
        public CurrentMimeType()
        {
        }

        // If contenttype is null, use the url encoded thing
        public CurrentMimeType(string contentType, string acceptType)
        {
            contentType = contentType ?? MimeType.HttpFormMimetype;
            if (contentType.Contains(";"))
            {
                var parts = contentType.ToDelimitedArray(';');
                contentType = parts.First();

                if (parts.Last().Contains("charset"))
                {
                    Charset = parts.Last().Split('=').Last();
                }
            }

            ContentType = contentType;

            AcceptTypes = new MimeTypeList(acceptType);
        }

        public string ContentType { get; set; }
        public MimeTypeList AcceptTypes { get; set; }

        public string Charset { get; set; }
    }
}