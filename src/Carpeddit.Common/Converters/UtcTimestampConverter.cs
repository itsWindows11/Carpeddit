using System;

namespace Carpeddit.Common.Converters
{
    public class UtcTimestampConverter : BaseTimestampConverter
    {
        public override long ConvertToSeconds(DateTime dateTime)
            => new DateTimeOffset(dateTime).ToUnixTimeSeconds();

        public override DateTime ParseDateFromSeconds(long seconds)
            => DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
    }
}
