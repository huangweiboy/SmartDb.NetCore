using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDb.NetCore
{
    public class PageListResultEntity
    {
        public int PageSize { get; set; }

        public int CurrentPageIndex { get; set; }

        public int TotalCount { get; set; }

        public int TotalPageIndex { get; set; }

        public object Data{ get; set; }

        public PageListResultEntity()
        {
            PageSize = 10;
            CurrentPageIndex = 1;
            TotalCount = 0;
            TotalPageIndex = 0;
            Data = null;
        }

    }
}
