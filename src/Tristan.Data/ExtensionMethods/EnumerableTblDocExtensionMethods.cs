using System;
using System.Collections.Generic;
using System.Linq;
using Tristan.Data.Models;

namespace Tristan.Data.ExtensionMethods
{
    public static class EnumerableTblDocExtensionMethods
    {
        public static IEnumerable<TblDoc> SetOccurredOn(this IEnumerable<TblDoc> source)
        {
            var docs = source.ToList();
            foreach (var doc in docs) 
                doc.OccurredOn = DateTime.Now;
            return docs;
        }
    }
}