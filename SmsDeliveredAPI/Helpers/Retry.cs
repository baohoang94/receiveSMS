using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmsDeliveredAPI.Helpers
{
    public class Retry
    {
        /// <summary>
        /// Generic Retry
        /// </summary>
        /// <typeparam name="TResult">return type</typeparam>
        /// <param name="action">Method needs to be executed</param>
        /// <param name="retryInterval">Retry interval</param>
        /// <param name="retryCount">Retry Count</param>
        /// <param name="expectedResult">Expected Result</param>
        /// <param name="isExpectedResultEqual">true/false to check equal 
        /// or not equal return value</param>
        /// <param name="isSuppressException">
        /// Suppress exception is true / false</param>
        /// <returns></returns>
        public static TResult Execute<TResult>(
          Func<TResult> action,
          TimeSpan retryInterval,
          int retryCount,
          TResult expectedResult,
          bool isExpectedResultEqual = true,
          bool isSuppressException = true
           )
        {
            TResult result = default;

            bool succeeded = false;
            var exceptions = new List<Exception>();

            for (int retry = 0; retry < retryCount; retry++)
            {
                try
                {
                    if (retry > 0)
                        Thread.Sleep(retryInterval);
                    // Execute method
                    result = action();

                    if (isExpectedResultEqual)
                        succeeded = result.Equals(expectedResult);
                    else
                        succeeded = !result.Equals(expectedResult);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }

                if (succeeded)
                    return result;
            }

            if (!isSuppressException)
                throw new AggregateException(exceptions);
            else
                return result;
        }
    }
}
