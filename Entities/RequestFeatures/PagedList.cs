using System;

namespace Entities.RequestFeatures
{
    public class PagedList<T> : List<T>
    {
       public MetaData MetaData {get; set;}

       public PagedList(List<T> items, int count, int pageNumber, int PageSize)
       {
        MetaData = new MetaData
        {
            TotalCount = count,
            PageSize = PageSize,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(count / (double)PageSize)    
        };

        AddRange(items);
       }


       public static PagedList<T> ToPagedList(IEnumerable<T> source, int pageNumber, int PageSize ) 
       {
        var count = source.Count();
        var items = source
               .Skip((pageNumber - 1) * PageSize)
               .Take(PageSize).ToList();  

        return new PagedList<T>(items, count, pageNumber, PageSize);          
       }
    }
}