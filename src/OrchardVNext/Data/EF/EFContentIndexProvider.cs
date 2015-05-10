using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OrchardVNext.ContentManagement;
using OrchardVNext.ContentManagement.Records;

namespace OrchardVNext.Data.EF {
    public class EFContentIndexProvider : IContentIndexProvider {
        private readonly IEnumerable<IContentQueryExpressionHandler> _contentQueryExpressionHandlers;
        private readonly DataContext _dataContext;
        private readonly IDocumentStore _documentStore;

        public EFContentIndexProvider(IEnumerable<IContentQueryExpressionHandler> contentQueryExpressionHandlers,
            DataContext dataContext,
            IDocumentStore documentStore)
        {
            _contentQueryExpressionHandlers = contentQueryExpressionHandlers;
            _dataContext = dataContext;
            _documentStore = documentStore;
        }

        public void Index(IContent content, DocumentRecord document) {
            // Get Lambda and store this content.
            var data = document.Infoset.Data;

            foreach (var handler in _contentQueryExpressionHandlers) {
                var expression = handler.OnCreating<IContent>(content);

                if (expression == null)
                    continue;
                
                Logger.Debug("Adding {0} to index {1}.", typeof(DocumentRecord).Name, expression);

                // TODO: Remove with dynamic tables
                _dataContext.Set<DocumentIndexRecord>().Add(new DocumentIndexRecord {
                    ContentId = document.Id,
                    Map = expression.Map.ToString(),
                    Sort = expression.Sort.ToString(),
                    Value = data
                });

                Logger.Debug("Added {0} to index {1}.", typeof(DocumentRecord).Name, expression);
            }
        }

        public void DeIndex(IContent content) {
            var set = _dataContext.Set<DocumentIndexRecord>();

            set.RemoveRange(set.Where(x => x.ContentId == content.Id));
        }

        public IEnumerable<TContent> Query<TContent>(Expression<Func<TContent, bool>> map, 
            Expression<Action<IEnumerable<TContent>>> sort, 
            Func<TContent, bool> reduce) where TContent : IContent {
            
            var mapValue = map.ToString();
            var sortValue = sort.ToString();

            var indexedRecords = _dataContext
                .Set<DocumentIndexRecord>()
                .Where(x => x.Map == mapValue && x.Sort == sortValue).Select(r => r.ContentId).ToList();

            return _documentStore
                .Query<ContentItemVersionRecord>(ci => ci.Id == 1)
                .Select(x => new ContentItem { VersionRecord = x })
                .Where(x => x != null && reduce(x.As<TContent>()))
                .Cast<TContent>();
        }

        [Persistent]
        private class DocumentIndexRecord {
            public int Id { get; set; }
            public string Map { get; set; }
            public string Sort { get; set; }
            public int ContentId { get; set; }
            public string Value { get; set; }
        }
    }
}