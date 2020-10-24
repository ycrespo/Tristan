using Tristan.Core.Models;

namespace Tristan.Core.Validators
{
    public class NumberOfRetryValidator : IValidator<Doc>
    {
        private readonly int _maxNumberOfRetry;

        public NumberOfRetryValidator(int maxNumberOfRetry)
        {
            _maxNumberOfRetry = maxNumberOfRetry;
        }

        public bool Validate(Doc doc) => doc.NumberOfRetry <= _maxNumberOfRetry;
    }
}