using System;

namespace TaxCalculator.Common.Services
{
    public interface IClock
    {
        DateTime GetCurrentDateTime();
    }
}
