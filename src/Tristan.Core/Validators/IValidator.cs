using System.Collections.Generic;
using Tristan.Core.Models;

namespace Tristan.Core.Validators
{
    public interface IValidator<in T>
    {
          bool Validate(T doc);
    }
}