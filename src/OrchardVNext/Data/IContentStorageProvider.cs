using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OrchardVNext.ContentManagement;

namespace OrchardVNext.Data {
    public interface IContentStorageProvider : IDependency {
        void Store<T>(T content) where T : IContent;
        void Remove<T>(T content) where T : IContent;

        IEnumerable<T> Query<T>(
            Expression<Func<T, bool>> map,
            Func<T, bool> reduce) where T : IContent;

        IEnumerable<T> Query<T>(
            Expression<Func<T, bool>> map,
            Expression<Action<IEnumerable<T>>> sort,
            Func<T, bool> reduce) where T : IContent;
    }
}