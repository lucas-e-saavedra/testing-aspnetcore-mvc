using EmployeesApp.Validations;

namespace EmployeesApp.Tests.Validations
{
    public class AccountNumberValidationTests
    {
        private readonly AccountNumberValidation _validation;
        public AccountNumberValidationTests() => _validation = new AccountNumberValidation();

        [Fact]
        public void IsValid_ValidAccountNumber_ReturnsTrue() 
            => Assert.True(_validation.IsValid("123-1234567890-12"));

        [Theory]
        [InlineData("12-1234567890-12")]
        [InlineData("1234-1234567890-12")]
        public void IsValid_AccountNumberFirstPartWrong_ReturnsFalse(string accountNumber) 
            => Assert.False(_validation.IsValid(accountNumber));

        [Theory]
        [InlineData("123-123456789-12")]
        [InlineData("123-123456789012-12")]
        public void IsValid_AccountNumberMiddlePartWrong_ReturnsFalse(string accountNumber) 
            => Assert.False(_validation.IsValid(accountNumber));

        [Theory]
        [InlineData("123-1234567890-1")]
        [InlineData("123-1234567890-123")]
        public void IsValid_AccountNumberLastPartWrong_ReturnsFalse(string accountNumber) 
            => Assert.False(_validation.IsValid(accountNumber));

        [Theory]
        [InlineData("123+1234567890-12")]
        [InlineData("123-1234567890=12")]
        [InlineData("123=1234567890+12")]
        public void IsValid_InvalidDelimiters_ThrowsArgumentException(string accNumber) 
            => Assert.Throws<ArgumentException>(() => _validation.IsValid(accNumber));

    }
}
