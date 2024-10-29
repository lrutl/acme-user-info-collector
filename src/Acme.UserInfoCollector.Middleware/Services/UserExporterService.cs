using Microsoft.Extensions.Logging;

namespace Acme.UserInfoCollector.Middleware.Services
{
    public class UserExporterService
    {
        private ILogger _logger;

        public UserExporterService(ILogger logger)
        {
            _logger = logger;
        }
    }
}
