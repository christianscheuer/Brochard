using System.Collections.Generic;
using OrchardVNext.ContentManagement;

namespace OrchardVNext.Data {
    public interface IContentItemStore : IDependency {
        void Store(IContent contentItem);
        IContent Get(int id);
        IContent Get(int id, VersionOptions options);
        IEnumerable<IContent> GetMany(IEnumerable<int> ids);
    }
}