using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Infrastructure
{
    public class PageData<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _currentItems;
        public int TotalCount { get; private set; }
        public int Page { get; private set; }
        public int Perpage { get; private set; }
        public int TotalPage { get; private set; }

        public bool HasNextPage { get; private set; }
        public bool HasPreviousPage { get; private set; }


        public int NextPage
        {
            get
            {
                if (!HasNextPage) throw new InvalidOperationException();

                return Page + 1;
            }
        }


        public int PreviousPage
        {
            get
            {
                if (!HasPreviousPage) throw new InvalidOperationException();

                return Page - 1;
            }
        }

        public PageData(IEnumerable<T> currentItems, int totalCount, int page, int perPage)
        {
            _currentItems = currentItems;

            TotalCount = totalCount;
            Page = page;
            Perpage = perPage;

            TotalPage = (int)Math.Ceiling((float)TotalCount / Perpage);

            HasNextPage = Page < TotalPage;
            HasPreviousPage = Page > 1;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _currentItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}