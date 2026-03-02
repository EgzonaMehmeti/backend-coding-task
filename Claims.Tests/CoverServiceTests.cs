using Claims.DTOs;
using Claims.Models;
using Claims.Repositories.Interfaces;
using Claims.Services;
using Claims.Services.Interfaces;
using Moq;
using Xunit;

namespace Claims.Tests
{
    public class CoverServiceTests
    {
        private readonly Mock<ICoverRepository> _repositoryMock;
        private readonly Mock<IAuditService> _auditMock;
        private readonly CoverService _service;

        public CoverServiceTests()
        {
            _repositoryMock = new Mock<ICoverRepository>();
            _auditMock = new Mock<IAuditService>();
            _service = new CoverService(_repositoryMock.Object, _auditMock.Object);
        }

        [Fact]
        public async Task CreateAsync_Should_Create_Cover_And_Call_Audit()
        {
            var dto = new CreateCoverDto
            {
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(40),
                Type = CoverType.Yacht
            };
            await _service.CreateAsync(dto);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Cover>()), Times.Once);
            _auditMock.Verify(a => a.AuditCoverAsync(It.IsAny<string>(), "POST"), Times.Once);
        }
        [Fact]
        public async Task CreateAsync_Should_Throw_When_StartDate_In_Past()
        {
            var dto = new CreateCoverDto
            {
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(10),
                Type = CoverType.Yacht
            };

            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_Period_Exceeds_One_Year()
        {
            var dto = new CreateCoverDto
            {
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(400),
                Type = CoverType.Yacht
            };

            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateAsync_Should_Calculate_Premium()
        {
            var start = DateTime.UtcNow.AddDays(1);
            var end = start.AddDays(40);

            var dto = new CreateCoverDto
            {
                StartDate = start,
                EndDate = end,
                Type = CoverType.Yacht
            };

            Cover capturedCover = null;

            _repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Cover>()))
                .Callback<Cover>(c => capturedCover = c)
                .Returns(Task.CompletedTask);

            await _service.CreateAsync(dto);

            Assert.NotNull(capturedCover);
            Assert.True(capturedCover.Premium > 0);
        }
    }
}
