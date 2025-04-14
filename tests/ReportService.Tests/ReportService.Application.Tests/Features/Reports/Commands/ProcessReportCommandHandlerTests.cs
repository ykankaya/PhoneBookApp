using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ReportService.Application.Features.Reports.Commands.ProcessReport;
using ReportService.Application.Features.Reports.DTOs;
using ReportService.Application.Interfaces.Persistence;
using ReportService.Application.Tests.Mocks;
using ReportService.Domain.Entities;
using ReportService.Domain.Enums;
using Xunit;

namespace ReportService.Application.Tests.Features.Reports.Commands
{
    public class ProcessReportCommandHandlerTests
    {
        private readonly Mock<IReportDbContext> _mockContext;
        private readonly Mock<DbSet<Report>> _mockReportDbSet;
        private readonly Mock<DbSet<ReportDetail>> _mockReportDetailDbSet;
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<IOptions<ContactServiceOptions>> _mockContactServiceOptions;
        private readonly Mock<ILogger<ProcessReportCommandHandler>> _mockLogger;
        private readonly ProcessReportCommandHandler _handler;

        private readonly ContactServiceOptions _contactOptions;
        private readonly Guid _testReportId = Guid.NewGuid();


        public ProcessReportCommandHandlerTests()
        {
            _mockContext = new Mock<IReportDbContext>();
            _mockReportDbSet = new Mock<DbSet<Report>>();
            _mockReportDetailDbSet = new Mock<DbSet<ReportDetail>>();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockContactServiceOptions = new Mock<IOptions<ContactServiceOptions>>();
            _mockLogger = new Mock<ILogger<ProcessReportCommandHandler>>();

            // Mock DbContext Setup
            _mockContext.Setup(c => c.Reports).Returns(_mockReportDbSet.Object);
            _mockContext.Setup(c => c.ReportDetails).Returns(_mockReportDetailDbSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(1);

            // Mock Options Setup
            _contactOptions = new ContactServiceOptions { BaseUrl = "http://test-contact-service" };
            _mockContactServiceOptions.Setup(o => o.Value).Returns(_contactOptions);


            _handler = new ProcessReportCommandHandler(
                _mockContext.Object,
                _mockHttpClientFactory.Object,
                _mockContactServiceOptions.Object,
                _mockLogger.Object);
        }

       
        private void SetupMockDbSetData(List<Report> reports, List<ReportDetail> details)
        {
           
            _mockReportDbSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Report, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((System.Linq.Expressions.Expression<Func<Report, bool>> predicate, CancellationToken token) => reports.FirstOrDefault(predicate.Compile())); 

            var mockDetailsQueryable = details.AsQueryable();
             _mockReportDetailDbSet.As<IQueryable<ReportDetail>>().Setup(m => m.Provider).Returns(mockDetailsQueryable.Provider);
             _mockReportDetailDbSet.As<IQueryable<ReportDetail>>().Setup(m => m.Expression).Returns(mockDetailsQueryable.Expression);
             _mockReportDetailDbSet.As<IQueryable<ReportDetail>>().Setup(m => m.ElementType).Returns(mockDetailsQueryable.ElementType);
             _mockReportDetailDbSet.As<IQueryable<ReportDetail>>().Setup(m => m.GetEnumerator()).Returns(() => mockDetailsQueryable.GetEnumerator());

            
             _mockReportDetailDbSet.Setup(d => d.RemoveRange(It.IsAny<IEnumerable<ReportDetail>>()))
                                  .Callback<IEnumerable<ReportDetail>>(entitiesToRemove => {
                                       foreach (var entity in entitiesToRemove.ToList())
                                       {
                                           details.Remove(entity);
                                       }
                                   });

             _mockReportDetailDbSet.Setup(d => d.AddRangeAsync(It.IsAny<IEnumerable<ReportDetail>>(), It.IsAny<CancellationToken>()))
                                  .Callback<IEnumerable<ReportDetail>, CancellationToken>((entitiesToAdd, ct) => {
                                      details.AddRange(entitiesToAdd);
                                  })
                                  .Returns(Task.CompletedTask); 
        }

      
        private MockHttpMessageHandler SetupHttpClient(HttpStatusCode statusCode, object? content = null)
        {
            var response = new HttpResponseMessage(statusCode);
            if (content != null)
            {
                response.Content = JsonContent.Create(content);
            }
            var mockHttpMessageHandler = new MockHttpMessageHandler(response);
            var httpClient = new HttpClient(mockHttpMessageHandler);

           
            _mockHttpClientFactory.Setup(f => f.CreateClient("ContactServiceClient"))
                                  .Returns(httpClient);
            return mockHttpMessageHandler;
        }


        [Fact]
        public async Task Handle_ValidReportIdAndSuccessfulContactServiceCall_ShouldUpdateReportToTamamlandiWithDetails()
        {
            // Arrange
             var initialReports = new List<Report> { new Report { Id = _testReportId, Durum = ReportStatus.Hazirlaniyor, RaporDetaylari = new List<ReportDetail>() } };
             var initialDetails = new List<ReportDetail>();
             SetupMockDbSetData(initialReports, initialDetails);


            var contactServiceData = new List<ContactPersonDto>
            {
                new ContactPersonDto { Id = Guid.NewGuid(), IletisimBilgileri = new List<ContactContactInfoDto> {
                    new ContactContactInfoDto { BilgiTipi = "Konum", BilgiIcerigi = "Ankara" },
                    new ContactContactInfoDto { BilgiTipi = "TelefonNumarasi", BilgiIcerigi = "123" }
                }},
                 new ContactPersonDto { Id = Guid.NewGuid(), IletisimBilgileri = new List<ContactContactInfoDto> {
                    new ContactContactInfoDto { BilgiTipi = "Konum", BilgiIcerigi = "Ankara" }
                }},
                  new ContactPersonDto { Id = Guid.NewGuid(), IletisimBilgileri = new List<ContactContactInfoDto> {
                    new ContactContactInfoDto { BilgiTipi = "Konum", BilgiIcerigi = "İstanbul" },
                     new ContactContactInfoDto { BilgiTipi = "TelefonNumarasi", BilgiIcerigi = "456" },
                      new ContactContactInfoDto { BilgiTipi = "TelefonNumarasi", BilgiIcerigi = "789" }
                }}
            };
            var httpMessageHandler = SetupHttpClient(HttpStatusCode.OK, contactServiceData);

            var command = new ProcessReportCommand(_testReportId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(Unit.Value); // Handler Unit döner
            var processedReport = initialReports.First(r => r.Id == _testReportId);
            processedReport.Durum.Should().Be(ReportStatus.Tamamlandi);

          
            initialDetails.Should().HaveCount(2);
            initialDetails.Should().ContainSingle(d => d.KonumBilgisi == "Ankara")
                          .Which.Should().Match<ReportDetail>(d => d.KisiSayisi == 2 && d.TelefonNumarasiSayisi == 1);
            initialDetails.Should().ContainSingle(d => d.KonumBilgisi == "İstanbul")
                          .Which.Should().Match<ReportDetail>(d => d.KisiSayisi == 1 && d.TelefonNumarasiSayisi == 2);

           
            httpMessageHandler.NumberOfCalls.Should().Be(1);

          
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

         [Fact]
        public async Task Handle_ContactServiceReturnsError_ShouldUpdateReportToHata()
        {
             // Arrange
             var initialReports = new List<Report> { new Report { Id = _testReportId, Durum = ReportStatus.Hazirlaniyor } };
             var initialDetails = new List<ReportDetail>();
             SetupMockDbSetData(initialReports, initialDetails);

            var httpMessageHandler = SetupHttpClient(HttpStatusCode.InternalServerError); 
            var command = new ProcessReportCommand(_testReportId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
             var processedReport = initialReports.First(r => r.Id == _testReportId);
             processedReport.Durum.Should().Be(ReportStatus.Hata);
             initialDetails.Should().BeEmpty(); 

         
             httpMessageHandler.NumberOfCalls.Should().Be(1);
           
             _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

         [Fact]
        public async Task Handle_ContactServiceReturnsEmptyList_ShouldUpdateReportToTamamlandiWithNoDetails()
        {
             // Arrange
             var initialReports = new List<Report> { new Report { Id = _testReportId, Durum = ReportStatus.Hazirlaniyor } };
             var initialDetails = new List<ReportDetail>();
             SetupMockDbSetData(initialReports, initialDetails);

            var contactServiceData = new List<ContactPersonDto>();
            var httpMessageHandler = SetupHttpClient(HttpStatusCode.OK, contactServiceData);
            var command = new ProcessReportCommand(_testReportId);

            // Act
             await _handler.Handle(command, CancellationToken.None);

             // Assert
             var processedReport = initialReports.First(r => r.Id == _testReportId);
             processedReport.Durum.Should().Be(ReportStatus.Tamamlandi);
             initialDetails.Should().BeEmpty(); 

             httpMessageHandler.NumberOfCalls.Should().Be(1);
             _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_ReportAlreadyProcessed_ShouldDoNothing()
        {
            // Arrange
            var initialReports = new List<Report> { new Report { Id = _testReportId, Durum = ReportStatus.Tamamlandi } }; 
            var initialDetails = new List<ReportDetail>();
            SetupMockDbSetData(initialReports, initialDetails);
            var httpMessageHandler = SetupHttpClient(HttpStatusCode.OK, new List<ContactPersonDto>()); 
            var command = new ProcessReportCommand(_testReportId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            httpMessageHandler.NumberOfCalls.Should().Be(0); 
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

         [Fact]
        public async Task Handle_ReportNotFound_ShouldDoNothing()
        {
            // Arrange
            var initialReports = new List<Report>(); //
            var initialDetails = new List<ReportDetail>();
             SetupMockDbSetData(initialReports, initialDetails);
            var httpMessageHandler = SetupHttpClient(HttpStatusCode.OK, new List<ContactPersonDto>());
            var command = new ProcessReportCommand(_testReportId); 

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            httpMessageHandler.NumberOfCalls.Should().Be(0);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}