using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OrchardVNext.ContentManagement;
using OrchardVNext.ContentManagement.Records;

namespace OrchardVNext.Data {
    public interface IContentIndexProvider : IDependency {
        void Index(IContent content, DocumentRecord document);
        void DeIndex(IContent content);

        IEnumerable<TContent> Query<TContent>(Expression<Func<TContent, bool>> map,
            Expression<Action<IEnumerable<TContent>>> sort,
            Func<TContent, bool> reduce) where TContent : IContent;
    }
}