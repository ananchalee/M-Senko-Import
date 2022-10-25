using M_Senko_Import.Library;

namespace M_Senko_Import
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private JobRun JobRun = new JobRun();
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                string message = "M-Senko --> Start Service.";
                Logs.lineNotify(message);
                while (!stoppingToken.IsCancellationRequested)
                {

                    JobRun.Import();

                    await Task.Delay(5 * 60 * 1000, stoppingToken);// Run once every 5 mins
                }
            }
            catch (TaskCanceledException ex)
            {
                string message = "Service async task was cancelled. Service will stop execution.";
                _logger.LogInformation("Service async task was cancelled. Service will stop execution.");

                Logs.lineNotify(message);
            }

        }
    }
}