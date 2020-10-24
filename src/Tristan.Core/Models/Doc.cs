using System;

namespace Tristan.Core.Models
{
    public class Doc : IEquatable<Doc>
    { 
        public Guid Id { get; set; } 
        public int NumberOfRetry { get; set; }
        
        public bool Moved { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }
        public string Path { get; set; }
        
        public string DestinationDir { get; set; }

        public bool Equals(Doc other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Id.Equals(other.Id) && Filename == other.Filename && Extension == other.Extension;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((Doc) obj);
        }

        public override int GetHashCode() => HashCode.Combine(Id, Filename, Extension);

        public static bool operator ==(Doc left, Doc right) => Equals(left, right);

        public static bool operator !=(Doc left, Doc right) => !Equals(left, right);
    }
}