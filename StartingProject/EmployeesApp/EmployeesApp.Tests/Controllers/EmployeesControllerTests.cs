using EmployeesApp.Contracts;
using EmployeesApp.Controllers;
using EmployeesApp.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EmployeesApp.Tests.Controllers
{
    public class EmployeesControllerTests
    {
        private readonly Mock<IEmployeeRepository> _mockRepo;
        private readonly EmployeesController _controller;
        public EmployeesControllerTests()
        {
            _mockRepo = new Mock<IEmployeeRepository>();
            _controller = new EmployeesController(_mockRepo.Object);
        }

        [Fact]
        public void Index_ActionExecutes_ReturnsViewForIndex()
        {
            var result = _controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Index_ActionExecutes_ReturnsExactNumberOfEmployees()
        {
            _mockRepo.Setup(repo => repo.GetAll()).Returns(new List<Employee>() { new Employee(), new Employee(), new Employee()});
            var result = _controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var employees = Assert.IsType<List<Employee>>(viewResult.Model);
            Assert.Equal(3, employees.Count);
        }

        [Fact]
        public void Create_ActionExecutes_ReturnsViewForCreate()
        {
            var result = _controller.Create();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Create_InvalidModelState_ReturnsView()
        {
            _controller.ModelState.AddModelError("Name", "Name is required");
            var employee = new Employee { Age = 33, AccountNumber = "123-1234567890-12" };
            var result = _controller.Create(employee);
            var viewResult = Assert.IsType<ViewResult>(result);
            var testingEmployee = Assert.IsType<Employee>(viewResult.Model);

            Assert.Equal(employee.AccountNumber, testingEmployee.AccountNumber);
            Assert.Equal(employee.Age, testingEmployee.Age);
        }

        [Fact]
        public void Create_InvalidModelState_CreateEmployeeNeverExecutes()
        {
            _controller.ModelState.AddModelError("Name", "Name is required");
            var employee = new Employee { Age = 33, AccountNumber = "123-1234567890-12" };
            _controller.Create(employee);

            _mockRepo.Verify(x=> x.CreateEmployee(employee), Times.Never);
        }

        [Fact]
        public void Create_ModelStateValid_CreateEmployeeCalledOnce()
        {
            Employee? savedEmployee = null;
            _mockRepo.Setup(x => x.CreateEmployee(It.IsAny<Employee>())).Callback<Employee>(x => savedEmployee = x);

            var employee = new Employee { 
                Name = "test",
                Age = 23,
                AccountNumber = "123-1234567890-12"
            };
            _controller.Create(employee);
            _mockRepo.Verify(x => x.CreateEmployee(employee), Times.Once);

            Assert.NotNull(savedEmployee);
            Assert.Equal(employee.Name, savedEmployee.Name);
            Assert.Equal(employee.Age, savedEmployee.Age);
            Assert.Equal(employee.AccountNumber, savedEmployee.AccountNumber);
        }

        [Fact]
        public void Create_ActionExecuted_RedirectsToIndexAction()
        {
            var employee = new Employee
            {
                Name = "test",
                Age = 23,
                AccountNumber = "123-1234567890-12"
            };
            var result = _controller.Create(employee);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
    }
}
