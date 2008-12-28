/***************************************************************************
 * Copyright Andy Brummer 2004-2005
 * 
 * This code is provided "as is", with absolutely no warranty expressed
 * or implied. Any use is at your own risk.
 *
 * This code may be used in compiled form in any way you desire. This
 * file may be redistributed unmodified by any means provided it is
 * not sold for profit without the authors written consent, and
 * providing that this notice and the authors name is included. If
 * the source code in  this file is used in any commercial application
 * then a simple email would be nice.
 * 
 **************************************************************************/

using System;
using System.Collections;

namespace Schedule
{
	/// <summary>This class impelements a filter which is only active between 2 single events.</summary>
	public class WindowWrapper : IScheduledItem
	{
		public WindowWrapper(IScheduledItem item, SingleEvent startEvent, SingleEvent endEvent)
		{
			_Item = item;
			_Begin = startEvent;
			_End = endEvent;
		}

		public void AddEventsInInterval(DateTime Begin, DateTime End, ArrayList List)
		{
			DateTime Next = NextRunTime(Begin, true);
			while (Next < End)
			{
				List.Add(Next);
				Next = NextRunTime(Next, false);
			}
		}

		public DateTime NextRunTime(DateTime time, bool AllowExact)
		{
			DateTime
				temp = _Item.NextRunTime(time, AllowExact),
				begin = _Begin.NextRunTime(time, true),
				end = _End.NextRunTime(time, true);

			//_Begin will return Max value if time is after the beginning of the window
			if (begin != DateTime.MaxValue)
				return begin;
			//_End will return Max value if time is after the end of the window
			if (end == DateTime.MaxValue)
				return end;
			
			//we don't know if temp is after end yet.  We need to use NextRunTime to double check.
			DateTime boundryCheck = _End.NextRunTime(temp, true);
			if (boundryCheck == DateTime.MaxValue)
				return DateTime.MaxValue;

			return temp;
		}

		private IScheduledItem _Item;
		private SingleEvent _Begin, _End;
	}
}