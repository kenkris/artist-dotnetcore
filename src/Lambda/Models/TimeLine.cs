using System;
using System.Collections.Generic;

namespace Lambda.Models
{
    public class TimeLine
    {
        public TimelineSpan Span;
        public List<TimeLineItem> TimeLineItems;
    }

    public class TimeLineItem
    {
        public PersonModel Person;
        public List<TimelineSpan> Spans;
    }

    public class TimelineSpan
    {
        public DateTime Start;
        public DateTime End;
        public List<TimeLineRole> Roles;
    }

    public class TimeLineRole
    {
        public string Name;
        public string HexColor;
    }

}