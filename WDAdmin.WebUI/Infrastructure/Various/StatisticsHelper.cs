using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WDAdmin.WebUI.Infrastructure.Various
{
    /// <summary>
    /// Class StatisticsHelper.
    /// </summary>
    public static class StatisticsHelper
    {
        /// <summary>
        /// Calculates the average score.
        /// </summary>
        /// <param name="scores">The scores.</param>
        /// <returns>System.Double.</returns>
        public static double CalculateAverageScore(IEnumerable<double> scores)
        {
            if (!scores.Any()) return 0d;
            return (double)scores.Sum() / (double)scores.Count();
        }

        /// <summary>
        /// Calculates the passed percent.
        /// </summary>
        /// <param name="scores">The scores.</param>
        /// <returns>System.Double.</returns>
        public static double CalculatePassedPercent(IEnumerable<double> scores)
        {
            if (!scores.Any()) return 0d;
            return ((double)scores.Where(y => y >= Constants.MIN_SCORE_FOR_PASSING).Count() 
                / (double)scores.Count()) * 100d;
        }
    }
}