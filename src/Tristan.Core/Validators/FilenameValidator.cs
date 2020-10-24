using System.Text.RegularExpressions;
using Tristan.Core.Models;

namespace Tristan.Core.Validators
{
    public class FilenameValidator : IValidator<Doc>
    {
        public bool Validate(Doc doc) => !string.IsNullOrEmpty(doc.Filename) && Regex.IsMatch(doc.Filename, "^[0-9]{9,12}-.*$");
    }
}