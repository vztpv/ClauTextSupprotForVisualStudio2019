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
    class ClauTextCompletionSource : ICompletionSource
    {
        private ClauTextCompletionSourceProvider _provider;
        private readonly ITextBuffer _textBuffer;
        public ClauTextCompletionSource(ClauTextCompletionSourceProvider provider, ITextBuffer textBuffer)
        {
            _provider = provider;
            _textBuffer = textBuffer;
        }

        /// <summary>
        /// todo, keyword? list!
        /// </summary>
        private static string[] _clautextKeyword = new[] {
            // special keyword
            "Main", "Event", "id", "TRUE", "FALSE",
            // function Group 1
            "$call", "$make", "$call_by_data",
            "$_getch",
            "$print", "$return", "$local", "$parameter",
            "$if", "$else", "$while",
            "$iterate",
            "$load", "$load_only_data",
            "$save_data_only",
            // function Group 2
            "$AND", "$OR", "$concat",
            // function Group 3 - now not used?
            "AND", "OR", 
        };

        private ITrackingSpan FindTokenSpanAtPosition(ITrackingPoint point, ICompletionSession session)
        {
            SnapshotPoint currentPoint = (session.TextView.Caret.Position.BufferPosition) - 1;
            ITextStructureNavigator navigator = _provider.NavigatorService.GetTextStructureNavigator(_textBuffer);
            TextExtent extent = navigator.GetExtentOfWord(currentPoint);
            return currentPoint.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
        }

        void ICompletionSource.AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            List<Completion> completions = new List<Completion>();

            foreach (var x in _clautextKeyword)
            {
                completions.Add(new Completion(x, x, x, null, null));
            }

            var triggerPoint = session.GetTriggerPoint(_textBuffer);

            var position = triggerPoint.GetPosition(_textBuffer.CurrentSnapshot);

            var span = FindTokenSpanAtPosition(session.GetTriggerPoint(_textBuffer),
            session);
                //_textBuffer.CurrentSnapshot.CreateTrackingSpan(position, 0, SpanTrackingMode.EdgeInclusive);

            completionSets.Add(new ClauTextCompletionSet("", "", span, completions, null));
        }

        class ClauTextCompletionSet : CompletionSet
        {
            public ClauTextCompletionSet(string moniker, string displayName, ITrackingSpan applicableTo, IEnumerable<Completion> completions, IEnumerable<Completion> completionBuilders) :
                base(moniker, displayName, applicableTo, completions, completionBuilders)
            {
                //
            }

            public override void SelectBestMatch()
            {
                SelectBestMatch(CompletionMatchType.MatchInsertionText, true);
                if (!SelectionStatus.IsSelected)
                {
                    SelectBestMatch(CompletionMatchType.MatchInsertionText, false);
                }
            }
        }

        private bool m_isDisposed;
        public void Dispose()
        {
            if (!m_isDisposed)
            {
                GC.SuppressFinalize(this);
                m_isDisposed = true;
            }
        }
    }
}
