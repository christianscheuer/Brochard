using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OrchardVNext.ContentManagement;

namespace OrchardVNext.Data {
    public interface IContentQueryExpressionHandler : IDependency {
        ContentQueryExpression OnCreating<TContent>(IContent content) where TContent : IContent;
    }

    public class ContentQueryExpression {
        public ContentQueryExpression(
            Expression<Func<IContent, bool>> map,
            Expression<Action<IEnumerable<IContent>>> sort) {
            Map = map;
            Sort = sort;
        }

        public Expression<Func<IContent, bool>> Map { get; }
        public Expression<Action<IEnumerable<IContent>>> Sort { get; }

        public override string ToString() {
            return $"Map: {Map}, Sort: {Sort}";
        }
    }
}