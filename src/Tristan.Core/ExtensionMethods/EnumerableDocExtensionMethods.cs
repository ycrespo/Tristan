using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tristan.Core.Models;

namespace Tristan.Core.ExtensionMethods
{
    public static class EnumerableDocExtensionMethods
    {
        public static IEnumerable<Doc> GetValidDestinations(this IEnumerable<Doc> validDocs, string destinationDirectory)
        {
            var docs = validDocs.ToList();
            
            foreach (var doc in docs)
            {
                var index = doc.Filename.IndexOf(value: '-');
                var dirName = doc.Filename.Substring(startIndex: 0, index - 1);
                doc.DestinationDir = Path.Combine(destinationDirectory, dirName);
            }
            return docs;
        }
        
        public static IEnumerable<Doc> GetInValidDestinations(this IEnumerable<Doc> inValidDocs, string errorsDirectory)
        {
            var docs = inValidDocs.ToList();
            
            foreach (var doc in docs) 
                doc.DestinationDir = errorsDirectory;

            return docs;
        }

        public static IEnumerable<Doc> IncrementNumberOfRetry(this IEnumerable<Doc> source)
        {
            var docs = source.ToList();
            foreach (var doc in docs) 
                doc.NumberOfRetry += 1;
            return docs;
        }
    }
}