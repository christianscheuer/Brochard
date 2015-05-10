using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OrchardVNext.ContentManagement;
using OrchardVNext.ContentManagement.Records;

namespace OrchardVNext.Data {
    public class ContentStorageProvider : IContentStorageProvider {
        private readonly IEnumerable<IDocumentStore> _contentDocumentStores;
        private readonly IContentIndexProvider _contentIndexProvider;

        public ContentStorageProvider(IEnumerable<IDocumentStore> contentDocumentStores,
            IContentIndexProvider contentIndexProvider) {
            _contentDocumentStores = contentDocumentStores;
            _contentIndexProvider = contentIndexProvider;
        }

        public void Store<T>(T content) where T : IContent {
            _contentDocumentStores.Invoke(store => {
                store.Store(content.ContentItem.Record);
                store.Store(content.ContentItem.VersionRecord);
            });
            _contentIndexProvider.Index(content, content.ContentItem.Record);
        }

        public void Remove<T>(T content) where T : IContent {
            _contentDocumentStores.Invoke(store => {
                store.Remove(content.ContentItem.Record);
                store.Remove(content.ContentItem.VersionRecord);
            });
            _contentIndexProvider.DeIndex(content);
        }

        public IEnumerable<T> Query<T>(
            Expression<Func<T, bool>> map, 
            Func<T, bool> reduce) where T : IContent {
            return Query(
                map,
                null,
                reduce
            );
        }

        public IEnumerable<T> Query<T>(
            Expression<Func<T, bool>> map, 
            Expression<Action<IEnumerable<T>>> sort,
            Func<T, bool> reduce) where T : IContent {

            return _contentIndexProvider.Query(map, sort, reduce);
        }
    }
}