using System;

namespace Carpeddit.Common.Converters
{
    public class LocalTimestampConverter : BaseTimestampConverter
    {
        public override long ConvertToSeconds(DateTime dateTime)
            => new DateTimeOffset(dateTime + new DateTimeOffset(dateTime).Offset).ToUnixTimeSeconds();

        public override DateTime ParseDateFromSeconds(long seconds)
            => DateTimeOffset.FromUnixTimeSeconds(seconds).LocalDateTime;
    }
}
