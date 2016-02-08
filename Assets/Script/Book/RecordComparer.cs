using System.Collections.Generic;

public class RecordComparer : IComparer<Record>
{
	//public int Compare(object o1, object o2)
	public int Compare(Record r1, Record r2)
	{
		//Record r1 = (Record)o1;
		//Record r2 = (Record)o2;
		if(r1 == null)
		{
			if(r2 == null)
			{
				// If x is null and y is null, they're
				// equal. 
				return 0;
			}
			else
			{
				// If x is null and y is not null, y
				// is greater. 
				return -1;
			}
		}
		else
		{
			// If x is not null...
			//
			if (r2 == null)
				// ...and y is null, x is greater.
			{
				return 1;
			}
			else
			{
				return r1.m_nextTime.CompareTo(r2.m_nextTime);
			}
		}
	}
}
