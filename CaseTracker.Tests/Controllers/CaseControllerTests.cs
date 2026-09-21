using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CaseTracker.Controllers.Cases;
using CaseTrackerApplication.DTOs.Cases;
using CaseTrackerApplication.DTOs.Common;
using CaseTrackerApplication.Interfaces.Services.Cases;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CaseTracker.Tests.Controllers
{
    public class CaseControllerTests
    {
        [Fact]
        public async Task GetAll_ShouldReturnOkWithStandardApiResponse()
        {
            // Arrange
            var mockCaseService = new Mock<ICaseService>();
            var sampleCases = new List<CaseDto>
            {
                new CaseDto { CaseId = Guid.NewGuid(), CaseNumber = "WP/100/2026", CaseTitle = "Party A vs Party B" }
            };

            mockCaseService.Setup(s => s.GetAllCasesAsync(null))
                .ReturnsAsync(sampleCases);

            var controller = new CaseController(mockCaseService.Object);

            // Act
            var actionResult = await controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var response = Assert.IsType<ApiResponse<IEnumerable<CaseDto>>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Single(response.Data!);
        }

        [Fact]
        public async Task GetById_ShouldReturnOkWithCaseDetail()
        {
            // Arrange
            var mockCaseService = new Mock<ICaseService>();
            var caseId = Guid.NewGuid();
            var detailDto = new CaseDetailDto
            {
                CaseId = caseId,
                CaseNumber = "WP/200/2026",
                CaseTitle = "Appellant vs Respondent",
                CaseStatus = "Pending"
            };

            mockCaseService.Setup(s => s.GetCaseByIdAsync(caseId))
                .ReturnsAsync(detailDto);

            var controller = new CaseController(mockCaseService.Object);

            // Act
            var actionResult = await controller.GetById(caseId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var response = Assert.IsType<ApiResponse<CaseDetailDto>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(caseId, response.Data!.CaseId);
            Assert.Equal("WP/200/2026", response.Data!.CaseNumber);
        }

        [Fact]
        public async Task GetByCnr_ShouldReturnOkWithMatchedCase()
        {
            // Arrange
            var mockCaseService = new Mock<ICaseService>();
            var cnr = "TNHC010001232026";
            var caseDto = new CaseDto
            {
                CaseId = Guid.NewGuid(),
                CnrNumber = cnr,
                CaseNumber = "WP/300/2026"
            };

            mockCaseService.Setup(s => s.GetCaseByCnrAsync(cnr))
                .ReturnsAsync(caseDto);

            var controller = new CaseController(mockCaseService.Object);

            // Act
            var actionResult = await controller.GetByCnr(cnr);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var response = Assert.IsType<ApiResponse<CaseDto>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(cnr, response.Data!.CnrNumber);
        }
    }
}
