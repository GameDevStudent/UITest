using System.Collections.Generic;

public class ItemComparer : IComparer<BookItem>
{
	//public int Compare(object o1, object o2)
	public int Compare(BookItem item1, BookItem item2)
	{
		return item1.Word.CompareTo(item2.Word);
	}
}
