using System;
using Tristan.Core.Models;
using Tristan.Data.Models;

namespace Tristan.Adapters
{
    public static class Mapper
    {
        public static TblDoc Map(Doc doc)
        {
            return new TblDoc
            {
                Id = doc.Id,
                NumberOfRetry = doc.NumberOfRetry,
                Filename = doc.Filename,
                Extension = doc.Extension,
                Path = doc.Path,
                DestinationDir = doc.DestinationDir,
                Moved = doc.Moved
            };
        }

        public static Doc Map(TblDoc tblDoc)
        {
            return new Doc
            {
                Id = tblDoc.Id,
                NumberOfRetry = tblDoc.NumberOfRetry,
                Filename = tblDoc.Filename,
                Extension = tblDoc.Extension,
                Path = tblDoc.Path,
                DestinationDir = tblDoc.DestinationDir,
                Moved = tblDoc.Moved
            };
        }
        
        public static TblDoc Map(TblDoc tblDoc, Doc doc)
        {
            tblDoc.Id = doc.Id;
            tblDoc.NumberOfRetry = doc.NumberOfRetry;
            tblDoc.Filename = doc.Filename;
            tblDoc.Extension = doc.Extension;
            tblDoc.Path = doc.Path;
            tblDoc.DestinationDir = doc.DestinationDir;
            tblDoc.Moved = doc.Moved;

            return tblDoc;
        }
        

    }
}