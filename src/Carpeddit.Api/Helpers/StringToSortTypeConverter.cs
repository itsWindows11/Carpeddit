using Carpeddit.Api.Enums;

namespace Carpeddit.Api.Helpers
{
    public static class StringToSortTypeConverter
    {
        public static string ToSortString(this SortMode value, string fallbackValue = "Best")
        {
            return value switch
            {
                SortMode.Best => "Best",
                SortMode.Hot => "Hot",
                SortMode.New => "New",
                SortMode.TopNow => "Top (Now)",
                SortMode.TopToday => "Top (Today)",
                SortMode.TopWeek => "Top (Week)",
                SortMode.TopMonth => "Top (Month)",
                SortMode.TopYear => "Top (Year)",
                SortMode.TopAllTime => "Top (All Time)",
                SortMode.ControversialNow => "Controversial (Now)",
                SortMode.ControversialToday => "Controversial (Today)",
                SortMode.ControversialWeek => "Controversial (Week)",
                SortMode.ControversialMonth => "Controversial (Month)",
                SortMode.ControversialYear => "Controversial (Year)",
                SortMode.ControversialAllTime => "Controversial (All Time)",
                SortMode.Rising => "Rising",
                _ => fallbackValue,
            };
        }

        public static string ToAPISort(this SortMode value, string fallbackValue = "best")
        {
            return value switch
            {
                SortMode.Best => "best",
                SortMode.Hot => "hot",
                SortMode.New => "new",
                SortMode.TopNow => "top",
                SortMode.TopToday => "top",
                SortMode.TopWeek => "top",
                SortMode.TopMonth => "top",
                SortMode.TopYear => "top",
                SortMode.TopAllTime => "top",
                SortMode.ControversialNow => "controversial",
                SortMode.ControversialToday => "controversial",
                SortMode.ControversialWeek => "controversial",
                SortMode.ControversialMonth => "controversial",
                SortMode.ControversialYear => "controversial",
                SortMode.ControversialAllTime => "controversial",
                SortMode.Rising => "rising",
                _ => fallbackValue,
            };
        }

        public static SortMode ToSortMode(this string value, SortMode fallbackValue = SortMode.Best)
        {
            return value switch
            {
                "Best" => SortMode.Best,
                "Hot" => SortMode.Hot,
                "New" => SortMode.New,
                "Top (Now)" => SortMode.TopNow,
                "Top (Today)" => SortMode.TopToday,
                "Top (Week)" => SortMode.TopWeek,
                "Top (Month)" => SortMode.TopMonth,
                "Top (Year)" => SortMode.TopYear,
                "Top (All Time)" => SortMode.TopAllTime,
                "Controversial (Now)" => SortMode.ControversialNow,
                "Controversial (Today)" => SortMode.ControversialToday,
                "Controversial (Week)" => SortMode.ControversialWeek,
                "Controversial (Month)" => SortMode.ControversialMonth,
                "Controversial (Year)" => SortMode.ControversialYear,
                "Controversial (All Time)" => SortMode.ControversialAllTime,
                "Rising" => SortMode.Rising,
                _ => fallbackValue,
            };
        }
    }
}
