using Mzg.Localization.Abstractions;

namespace Mzg.Web.Api.Models
{
    public class UpdateLocalizedTextModel
    {
        public LanguageCode Language { get; set; }

        public LocalizedTextLabel[] Labels { get; set; }
    }
}