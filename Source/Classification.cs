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

    [Export(typeof(IViewTaggerProvider))]
    [ContentType("ClauText")]
    [TagType(typeof(ClassificationTag))]
    public sealed class ViewTaggerProvider3 : IViewTaggerProvider
    {
        [Import]
        public IClassificationTypeRegistryService Registry;

        [Import]
        internal ITextSearchService TextSearchService { get; set; }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (buffer != textView.TextBuffer)
                return null;

            var classType = Registry.GetClassificationType("region-foreground3");
            return new RegionTagger3(textView, TextSearchService, classType) as ITagger<T>;
        }
        public static class TypeExports
        {
            [Export(typeof(ClassificationTypeDefinition))]
            [Name("region-foreground3")]
            public static ClassificationTypeDefinition OrdinaryClassificationType;
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "region-foreground3")]
        [Name("region-foreground3")]
        [UserVisible(true)]
        [Order(After = Priority.High)]
        public sealed class RegionForeground : ClassificationFormatDefinition
        {
            public RegionForeground()
            {
                DisplayName = "Region Foreground3";
                ForegroundColor = Colors.LightYellow;
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
            
            var x = FindAll("#[^\r\n]*", snapshot);

            foreach (var a in x)
            {
                wordSpans.Add(new SnapshotSpan(snapshot, a.Start, a.Length));
            }

            return new NormalizedSnapshotSpanCollection(wordSpans);
        }

        private IEnumerable<SnapshotSpan> FindAll(String searchPattern, ITextSnapshot textSnapshot)
        {
            if (textSnapshot == null)
                return null;

            return m_SearchService.FindAll(
                new FindData(searchPattern, textSnapshot)
                {
                    FindOptions = FindOptions.MatchCase | FindOptions.UseRegularExpressions
                });

        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (m_CurrentSpans.Count == 0)
                yield return new TagSpan<ClassificationTag>(new SnapshotSpan(), null);
            foreach (var x in m_CurrentSpans)
            {
                yield return new TagSpan<ClassificationTag>(x, new ClassificationTag(m_Type));
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
                yield return new TagSpan<ClassificationTag>(new SnapshotSpan(), null);
            foreach (var x in m_CurrentSpans)
            {
                yield return new TagSpan<ClassificationTag>(x, new ClassificationTag(m_Type));
            }
        }

    }

    public sealed class RegionTagger3 : ITagger<ClassificationTag>
    {
        private readonly ITextView m_View;
        private readonly ITextSearchService m_SearchService;
        private readonly IClassificationType m_Type;
        private List<SnapshotSpan> m_CurrentSpans;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged = delegate { };

        public RegionTagger3(ITextView view, ITextSearchService searchService, IClassificationType type)
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
            var x = FindAll("\"([^\r\n\"]|\\\")*\"", snapshot);

            foreach (var a in x)
            {
                wordSpans.Add(new SnapshotSpan(snapshot, a.Start, a.Length));
            }

            x = FindAll("[$]parameter[.][^ \r\n]+", snapshot);
            foreach (var a in x)
            {
                wordSpans.Add(new SnapshotSpan(snapshot, a.Start, a.Length));
            }

            x = FindAll("[$]local[.][^ \r\n]+", snapshot);
            foreach (var a in x)
            {
                wordSpans.Add(new SnapshotSpan(snapshot, a.Start, a.Length));
            }
            x = FindAll("/[^ \t\r\n]+", snapshot);
            foreach (var a in x)
            {
                wordSpans.Add(new SnapshotSpan(snapshot, a.Start, a.Length));
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
                    FindOptions = (FindOptions.MatchCase
                    | FindOptions.UseRegularExpressions)
                });

        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (m_CurrentSpans.Count == 0)
                yield return new TagSpan<ClassificationTag>(new SnapshotSpan(), null);
            foreach (var x in m_CurrentSpans)
            {
                yield return new TagSpan<ClassificationTag>(x, new ClassificationTag(m_Type));
            }
        }

    }
}
