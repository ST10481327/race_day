using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Moq;
using RaceDay_poe_part2.Controllers;
using RaceDay_poe_part2.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Testingp2
{
    public class UnitTest1
    {
        // Helper to set up the fake HTTP Context so your controllers can read the Session
        private void SetupSession(ControllerBase controller, string role, int userId)
        {
            var httpContext = new Mock<HttpContext>();
            var session = new Mock<ISession>();

            byte[] roleBytes = System.Text.Encoding.UTF8.GetBytes(role);
            byte[] userIdBytes = System.Text.Encoding.UTF8.GetBytes(userId.ToString());

            session.Setup(s => s.TryGetValue("Role", out roleBytes)).Returns(true);
            session.Setup(s => s.TryGetValue("UserId", out userIdBytes)).Returns(true);

            httpContext.Setup(c => c.Session).Returns(session.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext.Object
            };
        }

        // ---------------------------------------------------------
        // 1. Authentication Logic Tests
        // ---------------------------------------------------------
        [Fact]
        public void AuthController_Register_WithEmptyName_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AuthController();

            // Act
            var result = controller.RegisterInfo("", "Doe", 25, "test@test.com", "Pass123", AuthController.role.Participant);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest.Value.ToString().Should().Contain("Please fill in all fields.");
        }

        // ---------------------------------------------------------
        // 2. Unauthenticated Access Tests
        // ---------------------------------------------------------
        [Fact]
       
        public void UserProfile_GetProfile_WithoutSession_ReturnsUnauthorized()
        {
            // Arrange
            var controller = new UserProfileController();

            // Set up a fake session that returns null for the Role
            var httpContext = new Mock<HttpContext>();
            var session = new Mock<ISession>();
            httpContext.Setup(c => c.Session).Returns(session.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext.Object
            };

            // Act
            var result = controller.GetProfile();

            // Assert
            var unauthorizedResult = result as UnauthorizedObjectResult;

            // 1. Make sure it IS a 401 Unauthorized result
            unauthorizedResult.Should().NotBeNull();
            // 2. Make sure the status code is 401
            unauthorizedResult.StatusCode.Should().Be(401);
            // 3. Make sure the message is correct
            unauthorizedResult.Value.ToString().Should().Be("Please log in first.");
        }

        // ---------------------------------------------------------
        // 3. Role-Based Access Tests
        // ---------------------------------------------------------
        [Fact]
        public void Events_CreateEvent_AsParticipant_Returns403Forbidden()
        {
            // Arrange
            var controller = new EventsController();
            SetupSession(controller, "Participant", 3); // Set role to Participant

            // Act
            var result = controller.CreateEvent("Test", DateTime.Now.AddDays(5), "Location", 10, EventsController.eventType.Run, 50, EventsController.eventStatus.Open);

            // Assert
            var statusCodeResult = result as ObjectResult;
            statusCodeResult.Should().NotBeNull();
            statusCodeResult.StatusCode.Should().Be(403);
            statusCodeResult.Value.ToString().Should().Contain("Only organizers can create events.");
        }

        [Fact]
        public void Enrolments_Enrol_AsOrganizer_Returns403Forbidden()
        {
            // Arrange
            var controller = new EnrolmentsController();
            SetupSession(controller, "Organizer", 1); // Set role to Organizer

            // Act
            var result = controller.Enrol(1, 1);

            // Assert
            var statusCodeResult = result as ObjectResult;
            statusCodeResult.Should().NotBeNull();
            statusCodeResult.StatusCode.Should().Be(403);
            statusCodeResult.Value.ToString().Should().Contain("Only participants can enrol in events.");
        }

        // ---------------------------------------------------------
        // 4. Organizer/Event Management Tests
        // ---------------------------------------------------------
        [Fact]
        public void Events_CreateEvent_WithPastDate_ReturnsBadRequest()
        {
            // Arrange
            var controller = new EventsController();
            SetupSession(controller, "Organizer", 1);
            DateTime pastDate = DateTime.Now.AddDays(-5);

            // Act
            var result = controller.CreateEvent("Test", pastDate, "Location", 10, EventsController.eventType.Run, 50, EventsController.eventStatus.Open);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest.Value.ToString().Should().Contain("Event date cannot be in the past.");
        }
    }
}