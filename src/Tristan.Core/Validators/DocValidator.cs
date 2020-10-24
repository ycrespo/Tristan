using System.Collections.Generic;
using System.Linq;
using Tristan.Core.Models;

namespace Tristan.Core.Validators
{
    public class DocValidator : IDocValidator
    {
        private readonly IEnumerable<IValidator<Doc>> _validators;

        public DocValidator(IEnumerable<IValidator<Doc>> validators)
        {
            _validators = validators;
        }

        public bool Validate(Doc doc)
        {
            return _validators.All(v => v.Validate(doc));
        }
    }
}