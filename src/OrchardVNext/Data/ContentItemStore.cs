using System;
using System.Collections.Generic;
using System.Linq;
using OrchardVNext.ContentManagement;
using OrchardVNext.ContentManagement.Records;

namespace OrchardVNext.Data {
    public class ContentItemStore : IContentItemStore {
        private readonly IContentStorageProvider _contentStorageProvider;

        public ContentItemStore(
            IContentStorageProvider contentStorageProvider) {
            _contentStorageProvider = contentStorageProvider;
        }

        private readonly Func<ContentItemVersionRecord, VersionOptions, bool> _query = (versionRecord, options) => {
            if (options.IsPublished) {
                return versionRecord.Published;
            }
            if (options.IsLatest || options.IsDraftRequired) {
                return versionRecord.Latest;
            }
            if (options.IsDraft) {
                return versionRecord.Latest && !versionRecord.Published;
            }
            if (options.VersionNumber != 0) {
                return versionRecord.Number == options.VersionNumber;
            }
            // Shortcut for Version All?
            return versionRecord != null;
        };

        public void Store(IContent contentItem) {
            _contentStorageProvider.Store(contentItem);
        }

        public IContent Get(int id) {
            return Get(id, VersionOptions.Published);
        }

        public IContent Get(int id, VersionOptions options) {
            return _contentStorageProvider
                .Query<IContent>(
                m => m.ContentItem != null,
                s => s.OrderBy(x => x.ContentItem.VersionRecord.Number),
                r => r.Id == id && _query(r.ContentItem.VersionRecord, options))
                .LastOrDefault();
        }

        public IEnumerable<IContent> GetMany(IEnumerable<int> ids) {
            return _contentStorageProvider
                .Query<IContent>(
                    m => m.ContentItem != null,
                    s => s.OrderBy(x => x.ContentItem.VersionRecord.Number),
                    r => ids.Contains(r.Id) && _query(r.ContentItem.VersionRecord, VersionOptions.Published));
        }
    }

    public class ContentQueryExpressionHandler : IContentQueryExpressionHandler {
        ContentQueryExpression IContentQueryExpressionHandler.OnCreating<TContent>(IContent content) {
            return new ContentQueryExpression(
                m => m.ContentItem != null,
                s => s.OrderBy(x => x.ContentItem.VersionRecord.Number)
            );
        }
    }
}