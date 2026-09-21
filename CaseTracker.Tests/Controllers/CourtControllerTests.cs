using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CaseTracker.Controllers.Courts;
using CaseTrackerApplication.DTOs.Common;
using CaseTrackerApplication.DTOs.Courts;
using CaseTrackerApplication.Interfaces.Services.Courts;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CaseTracker.Tests.Controllers
{
    public class CourtControllerTests
    {
        [Fact]
        public async Task GetStates_ShouldReturnOkWithStatesList()
        {
            // Arrange
            var mockCourtService = new Mock<ICourtMasterService>();
            var states = new List<StateDto>
            {
                new StateDto { StateId = Guid.NewGuid(), StateCode = "TN", StateName = "Tamil Nadu" },
                new StateDto { StateId = Guid.NewGuid(), StateCode = "PY", StateName = "Puducherry" }
            };

            mockCourtService.Setup(s => s.GetStatesAsync())
                .ReturnsAsync(states);

            var controller = new CourtController(mockCourtService.Object);

            // Act
            var actionResult = await controller.GetStates();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var response = Assert.IsType<ApiResponse<IEnumerable<StateDto>>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(2, System.Linq.Enumerable.Count(response.Data!));
        }

        [Fact]
        public async Task GetDistricts_ShouldReturnDistrictsForState()
        {
            // Arrange
            var mockCourtService = new Mock<ICourtMasterService>();
            var stateId = Guid.NewGuid();
            var districts = new List<DistrictDto>
            {
                new DistrictDto { DistrictId = Guid.NewGuid(), StateId = stateId, DistrictCode = "01", DistrictName = "Chennai" }
            };

            mockCourtService.Setup(s => s.GetDistrictsAsync(stateId))
                .ReturnsAsync(districts);

            var controller = new CourtController(mockCourtService.Object);

            // Act
            var actionResult = await controller.GetDistricts(stateId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var response = Assert.IsType<ApiResponse<IEnumerable<DistrictDto>>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Single(response.Data!);
        }
    }
}
