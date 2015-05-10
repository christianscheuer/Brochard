using System;
using System.Linq;
using System.Linq.Expressions;
using OrchardVNext.ContentManagement;
using OrchardVNext.Data;

namespace OrchardVNext.Demo.Models {
    public class TestContentPartA : ContentPart {
        public string Line {
            get { return this.Retrieve(x => x.Line); }
            set { this.Store(x => x.Line, value); }
        }
    }

    public class ContentQueryExpressionHandler : IContentQueryExpressionHandler {
        ContentQueryExpression IContentQueryExpressionHandler.OnCreating<TContent>(IContent content) {
            if (!content.Has<TestContentPartA>())
                return null;

            return new ContentQueryExpression(
                m => m.As<TestContentPartA>().Line == "Foo",
                s => s.OrderBy(x => x.As<TestContentPartA>().Line)
            );
        }
    }
}