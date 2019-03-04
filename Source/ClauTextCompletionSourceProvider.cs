using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

namespace VSIXProject1
{
    [Export(typeof(ICompletionSourceProvider)), ContentType("ClauText"), Order, Name("ClauTextCompletionProvider")]
    internal class ClauTextCompletionSourceProvider : ICompletionSourceProvider
    {
        [Import]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }
        internal readonly IGlyphService _glyphService;

        [ImportingConstructor]
        public ClauTextCompletionSourceProvider(IGlyphService glyphService)
        {
            _glyphService = glyphService;
        }

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new ClauTextCompletionSource(this, textBuffer);
        }
    }
}
