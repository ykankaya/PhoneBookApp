using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReportService.Application.Interfaces.Persistence;
using ReportService.Domain.Entities;
using ReportService.Domain.Enums;
using System.Net.Http.Json;
using ReportService.Application.Features.Reports.DTOs;

namespace ReportService.Application.Features.Reports.Commands.ProcessReport
{
    public class ContactServiceOptions
    {
        public string BaseUrl { get; set; } = string.Empty;
    }

    public class ProcessReportCommandHandler : IRequestHandler<ProcessReportCommand, Unit>
    {
        private readonly IReportDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProcessReportCommandHandler> _logger;
        private readonly ContactServiceOptions _contactServiceOptions;


        public ProcessReportCommandHandler(
            IReportDbContext context,
            IHttpClientFactory httpClientFactory,
            IOptions<ContactServiceOptions> contactServiceOptions,
            ILogger<ProcessReportCommandHandler> logger)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _contactServiceOptions = contactServiceOptions.Value;
        }

        public async Task<Unit> Handle(ProcessReportCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing ReportId: {ReportId}", request.ReportId);


            var report = await _context.Reports
                .Include(r => r.RaporDetaylari)
                .FirstOrDefaultAsync(r => r.Id == request.ReportId && r.Durum == ReportStatus.Hazirlaniyor,
                    cancellationToken);

            if (report == null)
            {
                _logger.LogWarning("ReportId: {ReportId} not found or already processed.", request.ReportId);
                return Unit.Value;
            }

            try
            {
                var personsData = await GetPersonsFromContactService(cancellationToken);
                if (personsData == null || !personsData.Any())
                {
                    _logger.LogWarning("No data received from ContactService for ReportId: {ReportId}",
                        request.ReportId);

                    report.Durum = ReportStatus.Tamamlandi;
                    await _context.SaveChangesAsync(cancellationToken);
                    return Unit.Value;
                }


                var statistics = CalculateStatistics(personsData);


                report.RaporDetaylari.Clear();
                foreach (var stat in statistics)
                {
                    report.RaporDetaylari.Add(new ReportDetail
                    {
                        Id = Guid.NewGuid(),
                        ReportId = report.Id,
                        KonumBilgisi = stat.Key,
                        KisiSayisi = stat.Value.KisiSayisi,
                        TelefonNumarasiSayisi = stat.Value.TelefonNumarasiSayisi
                    });
                }


                report.Durum = ReportStatus.Tamamlandi;


                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("ReportId: {ReportId} processed successfully.", request.ReportId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing ReportId: {ReportId}", request.ReportId);

                try
                {
                    var reportOnError =
                        await _context.Reports.FirstOrDefaultAsync(r => r.Id == request.ReportId,
                            CancellationToken.None);
                    if (reportOnError != null)
                    {
                        reportOnError.Durum = ReportStatus.Hata;
                    }

                    await _context.SaveChangesAsync(CancellationToken.None);
                }
                catch (Exception dbEx)
                {
                    _logger.LogError(dbEx, "Failed to update report status to error for ReportId: {ReportId}",
                        request.ReportId);
                }
            }

            return Unit.Value;
        }

        private async Task<List<ContactPersonDto>?> GetPersonsFromContactService(CancellationToken cancellationToken)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                var requestUri = $"{_contactServiceOptions.BaseUrl.TrimEnd('/')}/api/persons";

                _logger.LogInformation("Requesting data from ContactService: {RequestUri}", requestUri);


                var response = await httpClient.GetAsync(requestUri, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var persons =
                        await response.Content.ReadFromJsonAsync<List<ContactPersonDto>>(
                            cancellationToken: cancellationToken);
                    _logger.LogInformation("Successfully received {Count} records from ContactService.",
                        persons?.Count ?? 0);
                    return persons;
                }
                else
                {
                    _logger.LogError(
                        "Failed to get data from ContactService. Status Code: {StatusCode}, Reason: {ReasonPhrase}",
                        response.StatusCode, response.ReasonPhrase);

                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError("ContactService Error Content: {ErrorContent}", errorContent);
                    return null;
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "HTTP request failed while contacting ContactService.");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting data from ContactService.");
                return null;
            }
        }

        private Dictionary<string, (int KisiSayisi, int TelefonNumarasiSayisi)> CalculateStatistics(
            List<ContactPersonDto> personsData)
        {
            _logger.LogInformation("Calculating statistics for {Count} persons.", personsData.Count);
            var locationStats = new Dictionary<string, (int KisiSayisi, int TelefonNumarasiSayisi)>();


            var locations = personsData
                .SelectMany(p => p.IletisimBilgileri)
                .Where(ci => ci.BilgiTipi.Equals("Konum", StringComparison.OrdinalIgnoreCase))
                .Select(ci => ci.BilgiIcerigi)
                .Distinct()
                .ToList();

            _logger.LogInformation("Found distinct locations: {Locations}", string.Join(", ", locations));


            foreach (var location in locations)
            {
                int personCount = 0;
                int phoneCount = 0;


                var personsInLocation = personsData
                    .Where(p => p.IletisimBilgileri.Any(ci =>
                        ci.BilgiTipi.Equals("Konum", StringComparison.OrdinalIgnoreCase) &&
                        ci.BilgiIcerigi.Equals(location, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                personCount = personsInLocation.Count;


                phoneCount = personsInLocation
                    .SelectMany(p => p.IletisimBilgileri)
                    .Count(ci =>
                        ci.BilgiTipi.Equals("TelefonNumarasi",
                            StringComparison.OrdinalIgnoreCase)); // Enum yerine string

                _logger.LogDebug("Location: {Location}, PersonCount: {PersonCount}, PhoneCount: {PhoneCount}", location,
                    personCount, phoneCount);

                if (personCount > 0)
                {
                    locationStats[location] = (personCount, phoneCount);
                }
            }

            _logger.LogInformation("Statistics calculation completed.");
            return locationStats;
        }
    }
}