using System;

namespace TaxCalculator.Common.Services.Implementations
{
    public class Clock: IClock
    {
        public DateTime GetCurrentDateTime()
        {
            return DateTime.UtcNow;;
        } 
    }
}
