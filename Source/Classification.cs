using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.Windows.Media;


namespace VSIXProject2
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("ClauText")]
    [TagType(typeof(ClassificationTag))]
    public sealed class ViewTaggerProvider : IViewTaggerProvider
    {
        [Import]
        public IClassificationTypeRegistryService Registry;

        [Import]
        internal ITextSearchService TextSearchService { get; set; }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (buffer != textView.TextBuffer)
                return null;

            var classType = Registry.GetClassificationType("region-foreground");
            return new RegionTagger(textView, TextSearchService, classType) as ITagger<T>;
        }
        public static class TypeExports
        {
            [Export(typeof(ClassificationTypeDefinition))]
            [Name("region-foreground")]
            public static ClassificationTypeDefinition OrdinaryClassificationType;
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "region-foreground")]
        [Name("region-foreground")]
        [UserVisible(true)]
        [Order(After = Priority.Low)]
        public sealed class RegionForeground : ClassificationFormatDefinition
        {
            public RegionForeground()
            {
                DisplayName = "Region Foreground";
                ForegroundColor = Colors.Gray;
            }
        }
    }
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("ClauText")]
    [TagType(typeof(ClassificationTag))]
    public sealed class ViewTaggerProvider2 : IViewTaggerProvider
    {
        [Import]
        public IClassificationTypeRegistryService Registry;

        [Import]
        internal ITextSearchService TextSearchService { get; set; }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (buffer != textView.TextBuffer)
                return null;

            var classType = Registry.GetClassificationType("region-foreground2");
            return new RegionTagger2(textView, TextSearchService, classType) as ITagger<T>;
        }
        public static class TypeExports
        {
            [Export(typeof(ClassificationTypeDefinition))]
            [Name("region-foreground2")]
            public static ClassificationTypeDefinition OrdinaryClassificationType;
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "region-foreground2")]
        [Name("region-foreground2")]
        [UserVisible(true)]
        [Order(After = Priority.High)]
        public sealed class RegionForeground : ClassificationFormatDefinition
        {
            public RegionForeground()
            {
                DisplayName = "Region Foreground2";
                ForegroundColor = Colors.LightGreen;
            }
        }
    }
    public sealed class RegionTagger : ITagger<ClassificationTag>
    {
        private readonly ITextView m_View;
        private readonly ITextSearchService m_SearchService;
        private readonly IClassificationType m_Type;
        private NormalizedSnapshotSpanCollection m_CurrentSpans;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged = delegate { };

        public RegionTagger(ITextView view, ITextSearchService searchService, IClassificationType type)
        {
            m_View = view;
            m_SearchService = searchService;
            m_Type = type;

            m_CurrentSpans = GetCommentWordSpans(m_View.TextSnapshot);

            m_View.GotAggregateFocus += SetupSelectionChangedListener;
        }

        private void SetupSelectionChangedListener(object sender, EventArgs e)
        {
            if (m_View != null)
            {
                m_View.LayoutChanged += ViewLayoutChanged;
                m_View.GotAggregateFocus -= SetupSelectionChangedListener;
            }
        }

        private void ViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (e.OldSnapshot != e.NewSnapshot)
            {
                m_CurrentSpans = GetCommentWordSpans(e.NewSnapshot);
                TagsChanged(this, new SnapshotSpanEventArgs(new SnapshotSpan(e.NewSnapshot, 0, e.NewSnapshot.Length)));
           //     m_CurrentSpans = GetCommentWordSpans2(e.NewSnapshot);
           //     TagsChanged(this, new SnapshotSpanEventArgs(new SnapshotSpan(e.NewSnapshot, 0, e.NewSnapshot.Length)));
            }
        }

        private NormalizedSnapshotSpanCollection GetCommentWordSpans(ITextSnapshot snapshot)
        {
            var wordSpans = new List<SnapshotSpan>();
            wordSpans.AddRange(FindAll("#", snapshot).Select(regionLine => regionLine.Start.GetContainingLine().Extent));
            return new NormalizedSnapshotSpanCollection(wordSpans);
        }

        private IEnumerable<SnapshotSpan> FindAll(String searchPattern, ITextSnapshot textSnapshot)
        {
            if (textSnapshot == null)
                return null;

            return m_SearchService.FindAll(
                new FindData(searchPattern, textSnapshot)
                {
                    FindOptions = FindOptions.WholeWord | FindOptions.MatchCase
                });

        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans == null || spans.Count == 0 || m_CurrentSpans.Count == 0)
                yield break;

            ITextSnapshot snapshot = m_CurrentSpans[0].Snapshot;
            spans = new NormalizedSnapshotSpanCollection(spans.Select(s => s.TranslateTo(snapshot, SpanTrackingMode.EdgeExclusive)));

            foreach (var span in NormalizedSnapshotSpanCollection.Intersection(m_CurrentSpans, spans))
            {
                yield return new TagSpan<ClassificationTag>(span, new ClassificationTag(m_Type));
            }
        }

      
        }
    public sealed class RegionTagger2 : ITagger<ClassificationTag>
    {
        private readonly ITextView m_View;
        private readonly ITextSearchService m_SearchService;
        private readonly IClassificationType m_Type;
        private List<SnapshotSpan> m_CurrentSpans;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged = delegate { };

        public RegionTagger2(ITextView view, ITextSearchService searchService, IClassificationType type)
        {
            m_View = view;
            m_SearchService = searchService;
            m_Type = type;

            m_CurrentSpans = GetCommentWordSpans(m_View.TextSnapshot);

            m_View.GotAggregateFocus += SetupSelectionChangedListener;
        }

        private void SetupSelectionChangedListener(object sender, EventArgs e)
        {
            if (m_View != null)
            {
                m_View.LayoutChanged += ViewLayoutChanged;
                m_View.GotAggregateFocus -= SetupSelectionChangedListener;
            }
        }

        private void ViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (e.OldSnapshot != e.NewSnapshot)
            {
                m_CurrentSpans = GetCommentWordSpans(e.NewSnapshot);
                TagsChanged(this, new SnapshotSpanEventArgs(new SnapshotSpan(e.NewSnapshot, 0, e.NewSnapshot.Length)));
            }
        }

        private List<SnapshotSpan> GetCommentWordSpans(ITextSnapshot snapshot)
        {
            var wordSpans = new List<SnapshotSpan>();
            String[] arr = new[]
            {
                "Event", "Main",
            };
            foreach (var i in arr)
            {
                var x = FindAll(i, snapshot);

                foreach (var a in x)
                {
                    wordSpans.Add(new SnapshotSpan(snapshot, a.Start, a.Length));
                }
            }
            return wordSpans;
        }

        private IEnumerable<SnapshotSpan> FindAll(String searchPattern, ITextSnapshot textSnapshot)
        {
            if (textSnapshot == null)
                return null;

            return m_SearchService.FindAll(
                new FindData(searchPattern, textSnapshot)
                {
                    FindOptions = ( FindOptions.WholeWord | FindOptions.MatchCase
                    | FindOptions.Wrap )
                });

        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (m_CurrentSpans.Count == 0)
                yield break;
            foreach (var x in m_CurrentSpans)
            {
                yield return new TagSpan<ClassificationTag>(x, new ClassificationTag(m_Type));
            }
        }

    }
}
