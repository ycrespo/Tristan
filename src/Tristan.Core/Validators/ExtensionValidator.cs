using System.Collections.Generic;
using System.Linq;
using Tristan.Core.Models;

namespace Tristan.Core.Validators
{
    public class ExtensionValidator : IValidator<Doc>
    {
        private readonly IEnumerable<string> _extensions;

        public ExtensionValidator(IEnumerable<string> extensions)
        {
            _extensions = extensions;
        }
        public bool Validate(Doc doc)
        {
            return _extensions.Contains(doc.Extension);
        }
    }
}