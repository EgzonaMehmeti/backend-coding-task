using Claims.DTOs;
using Claims.Models;
using Claims.Repositories.Interfaces;
using Claims.Services.Interfaces;
using Claims.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Claims.Tests
{
    public class ClaimServiceTests
    {
        private readonly Mock<IClaimRepository> _repositoryMock;
        private readonly Mock<IAuditService> _auditMock;
        private readonly Mock<ICoverRepository> _coverRepositoryMock;
        private readonly ClaimService _service;

        public ClaimServiceTests()
        {
            _repositoryMock = new Mock<IClaimRepository>();
            _auditMock = new Mock<IAuditService>();
            _coverRepositoryMock = new Mock<ICoverRepository>();

            _service = new ClaimService(
                _repositoryMock.Object,
                _auditMock.Object,
                _coverRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateAsync_Should_Create_Claim_And_Call_Audit()
        {
            var cover = new Cover
            {
                Id = "1",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            _coverRepositoryMock
                .Setup(r => r.GetByIdAsync("1"))
                .ReturnsAsync(cover);

            var dto = new CreateClaimDto
            {
                CoverId = "1",
                Created = DateTime.UtcNow.AddDays(1),
                Type = ClaimType.Collision,
                DamageCost = 50000
            };

            await _service.CreateAsync(dto);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Claim>()), Times.Once);
            _auditMock.Verify(a => a.AuditClaimAsync(It.IsAny<string>(), "POST"), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_DamageCost_Too_High()
        {
            var dto = new CreateClaimDto
            {
                CoverId = "1",
                Created = DateTime.UtcNow,
                DamageCost = 200000
            };

            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_Cover_Not_Found()
        {
            _coverRepositoryMock
                .Setup(r => r.GetByIdAsync("1"))
                .ReturnsAsync((Cover)null);

            var dto = new CreateClaimDto
            {
                CoverId = "1",
                Created = DateTime.UtcNow,
                DamageCost = 5000
            };

            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_Claim_Outside_Cover_Period()
        {
            var cover = new Cover
            {
                Id = "1",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(10)
            };

            _coverRepositoryMock
                .Setup(r => r.GetByIdAsync("1"))
                .ReturnsAsync(cover);

            var dto = new CreateClaimDto
            {
                CoverId = "1",
                Created = DateTime.UtcNow.AddDays(20),
                DamageCost = 5000
            };

            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync(dto));
        }
    }
}
