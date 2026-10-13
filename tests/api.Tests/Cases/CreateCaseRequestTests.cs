using System.ComponentModel.DataAnnotations;
using api.DTOs.Cases;

namespace api.Tests.Cases
{
    public class CreateCaseRequestTests
    {
        [Fact]
        public void Validate_LatitudeAbove90_ReturnsValidationError()
        {
            // Arrange
            var request = new CreateCaseRequest
            {
                DateOfBirth = new DateTime(1990, 1, 1),
                Sex = "F",
                Latitude = 91,
                Longitude = -46.6
            };

            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                request,
                new ValidationContext(request),
                validationResults,
                validateAllProperties: true
            );

            // Assert
            Assert.False(isValid);
            Assert.Contains(
                validationResults,
                error => error.MemberNames.Contains(nameof(CreateCaseRequest.Latitude))
            );
        }

        [Fact]
        public void Validate_LongitudeAboce180_ReturnValidationError()
        {
            // Arrange
            var request = new CreateCaseRequest
            {
                DateOfBirth = new DateTime(1990, 1, 1),
                Sex = "F",
                Latitude = 71,
                Longitude = 181
            };

            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                request,
                new ValidationContext(request),
                validationResults,
                validateAllProperties: true
            );

            //Assert
            Assert.False(isValid);
            Assert.Contains(
                validationResults,
                error => error.MemberNames.Contains(nameof(CreateCaseRequest.Longitude))
            );
        }

        [Fact]
        public void Validate_FieldsWithinRanges_ReturnsValid()
        {
            // Arrange
            var request = new CreateCaseRequest
            {
                DateOfBirth = new DateTime(1990, 1, 1),
                Sex = "F",
                Latitude = 71,
                Longitude = -141
            };

            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                request,
                new ValidationContext(request),
                validationResults,
                validateAllProperties: true
            );

            // Assert
            Assert.True(isValid);
        }
    }
}