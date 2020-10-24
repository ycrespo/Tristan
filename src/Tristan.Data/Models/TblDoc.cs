using System;

namespace Tristan.Data.Models
{
    public class TblDoc
    {
        public Guid Id { get; set; } 
        
        public int NumberOfRetry { get; set; }
        
        public bool Moved { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }
        public string Path { get; set; }
        public string DestinationDir { get; set; }
        public DateTime OccurredOn  { get; set; }
    }
}