#nullable enable
using System;

namespace AVS.CoreLib.Abstractions
{
    /// <summary>
    /// The provider allows to unbound from DateTime.UtcNow when using DI
    /// </summary>
    public interface IDateTimeProvider
    {
        DateTime GetSystemTime();
    }

    public class DateTimeProvider : IDateTimeProvider
    {
        private Func<DateTime>? _getTime = null;
        public DateTime GetSystemTime()
        {
            return _getTime?.Invoke() ?? DateTime.UtcNow;
        }

        public void UseCustomTime(Func<DateTime> getTime)
        {
            _getTime = getTime;
        }

        #region static
        private static IDateTimeProvider? _instance;

        public static IDateTimeProvider Instance
        {
            get => _instance ??= new DateTimeProvider();
            set => _instance = value;
        }

        /// <summary>
        /// returns <see cref="DateTime.UtcNow"/> unless <see cref="DateTimeProvider.Instance"/> is initialized with different time
        /// </summary>
        public static DateTime GetTime()
        {
            return Instance.GetSystemTime();
        }
        #endregion
    }
}