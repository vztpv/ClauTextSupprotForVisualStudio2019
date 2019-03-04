using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace VSIXProject1
{
   
    internal static class FileAndContentTypeDefinitions
    {
        [Export]
        [Name("ClauText")]
        [BaseDefinition("text")]
        internal static ContentTypeDefinition hidingContentTypeDefinition;

        [Export]
        [FileExtension(".clautext")]
        [ContentType("ClauText")]
        internal static FileExtensionToContentTypeDefinition hiddenFileExtensionDefinition;
    }

    class FileAndContents
    {

    }
}
