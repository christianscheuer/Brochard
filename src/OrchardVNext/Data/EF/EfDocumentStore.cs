using System;
using System.Collections.Generic;
using System.Linq;
using OrchardVNext.ContentManagement.Records;

namespace OrchardVNext.Data.EF
{
    public class EfDocumentStore : IDocumentStore {
        private readonly DataContext _dataContext;

        public EfDocumentStore(DataContext dataContext) {
            _dataContext = dataContext;
        }

        public void Store<T>(T document) where T : DocumentRecord {
            _dataContext.Add(document);
        }

        public void Remove<T>(T document) where T : DocumentRecord {
            _dataContext.Remove(document);
        }

        public IEnumerable<T> Query<T>() where T : DocumentRecord {
            return _dataContext.Set<T>();
        }

        public IEnumerable<T> Query<T>(Func<T, bool> filter) where T : DocumentRecord {
            return _dataContext.Set<T>().Where(filter);
        }
    }
}