using System.Diagnostics;

namespace Quick_CommerceApiForEx.Services
{
    public class PerformanceService
    {
        private readonly ILogger<PerformanceService> _logger;

        public PerformanceService(ILogger<PerformanceService> logger)
        {
            _logger = logger;
        }

        public async Task<T> MeasureAsync<T>(string operationName, Func<Task<T>> operation)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var result = await operation();
                stopwatch.Stop();
                
                if (stopwatch.ElapsedMilliseconds > 1000) // Log slow queries (>1s)
                {
                    _logger.LogWarning("Slow operation detected: {OperationName} took {ElapsedMs}ms", 
                        operationName, stopwatch.ElapsedMilliseconds);
                }
                else if (stopwatch.ElapsedMilliseconds > 100) // Log moderately slow queries (>100ms)
                {
                    _logger.LogInformation("Operation {OperationName} took {ElapsedMs}ms", 
                        operationName, stopwatch.ElapsedMilliseconds);
                }

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Operation {OperationName} failed after {ElapsedMs}ms", 
                    operationName, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}