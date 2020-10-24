using Tristan.Core.Models;

namespace Tristan.Core.Validators
{
    public interface IDocValidator
    {
        bool Validate(Doc doc);
    }
}