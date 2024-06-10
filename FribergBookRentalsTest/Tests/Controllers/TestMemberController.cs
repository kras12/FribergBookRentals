using FribergbookRentals.Data.Exceptions;
using FribergbookRentals.Data.Models;
using FribergbookRentals.Data.Repositories;
using FribergBookRentals.Controllers.Member;
using FribergBookRentals.Models;
using FribergBookRentals.Services;
using FribergBookRentalsTest.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace FribergBookRentalsTest.Tests.Controllers
{
    public class TestMemberController : TestControllerBase
    {
        #region Constants

        private const int BookLoanTime = 14;

        #endregion

        #region Fields

        IBookRepository _bookRepository;

        IBookLoanRepository _bookLoanRepository;        

        #endregion

        #region Constructors

        public TestMemberController()
        {
            _bookRepository = new BookRepository(_dbContext);
            _bookLoanRepository = new BookLoanRepository(_dbContext);
        }

        #endregion

        #region Methods

        private async Task SeedBookLoans(User user, int activeLoans = 0, int closedLoans = 0, TimeSpan? startTimeOffset = null)
        {
            var books = await _bookRepository.GetBooksAsync();
            DateTime startTime = startTimeOffset != null ? DateTime.Now.Add(startTimeOffset.Value) : DateTime.Now;

            if (activeLoans > 0)
            {
                await BookLoanHelper.CreateBookLoans(_bookLoanRepository, user, books.Take(activeLoans).ToList(), createActiveLoans: true, startTime, TimeSpan.FromDays(BookLoanTime - 1));
            }

            if (closedLoans > 0)
            {
                await BookLoanHelper.CreateBookLoans(_bookLoanRepository, user, books.Skip(activeLoans).Take(closedLoans).ToList(), createActiveLoans: false, startTime, TimeSpan.FromDays(BookLoanTime - 1));
            }            
        }

        #endregion

        #region Methods

        public static IEnumerable<object[]> GetTestProlongActiveBookLoanInputData()
        {
            List<object[]> data = new List<object[]>();

            for (int i = -BookLoanTime + 1; i <= 0; i++)
            {
                data.Add(new object[] { true, i });
            }

            for (int i = -BookLoanTime + 1; i <= 0; i++)
            {
                data.Add(new object[] { false, i });
            }

            return data;
        }

        #endregion

        #region Tests

        private async Task<MemberController> CreateMemberController(bool isUserLoggedIn)
        {
            // User
            var user = await GetDefaultUser();
            ClaimsPrincipal claimsPrincipal = CreateClaimsPrincipal(user, isUserLoggedIn);

            // Controller context
            var controllerContextMock = CreateControllerContextMock(claimsPrincipal);

            // Member controller
            var tempDataHelperMock = new Mock<ITempDataHelper>();
            var controller = new MemberController(_bookLoanRepository, _autoMapper, _bookRepository, tempDataHelperMock.Object);
            controller.ControllerContext = controllerContextMock.Object;    

            return controller;
        }

        [InlineData(true)]
        [InlineData(false)]
        [Theory]
        public async Task TestListBookLoans(bool isUserAuthenticated)
        {
            // Arrange
            int activeLoans = 10;
            int closedLoans = 8;
            var user = await GetDefaultUser();
            var memberController = await CreateMemberController(isUserAuthenticated);
            await SeedBookLoans(user, activeLoans: activeLoans, closedLoans: closedLoans);

            // Act
            var result = await memberController.Index();

            // Assert
            if (isUserAuthenticated)
            {
                var viewResult = result as ViewResult;
                Assert.NotNull(viewResult);

                var model = viewResult!.Model as MemberBookLoansViewModel;
                Assert.NotNull(model);
                Assert.True(model.ActiveLoans.Count == activeLoans, "Failed active loans count");
                Assert.True(model.ClosedLoans.Count == closedLoans, "Failed closed loans count");
                Assert.All(model.ActiveLoans, x => Assert.Null(x.ClosedTime));
                Assert.All(model.ClosedLoans, x => Assert.NotNull(x.ClosedTime));
                Assert.All(model.ActiveLoans, x => Assert.Equal(user.Id, x.User.Id));
                Assert.All(model.ClosedLoans, x => Assert.Equal(user.Id, x.User.Id));
            }
            else
            {
                Assert.IsType<UnauthorizedResult>(result);
            }
        }

        [InlineData(true)]
        [InlineData(false)]
        [Theory]
        public async Task TestCloseActiveBookLoan(bool isUserAuthenticated)
        {
            // Arrange
            int numLoans = 1;
            var user = await GetDefaultUser();
            var memberController = await CreateMemberController(isUserAuthenticated);
            await SeedBookLoans(user, activeLoans: numLoans);
            BookLoan targetBookLoan = (await _bookLoanRepository.GetActiveBookLoansAsync(user.Id)).First();

            // Act
            var result = await memberController.CloseLoan(targetBookLoan.Id);
            var closedBookLoan = await _bookLoanRepository.GetBookLoanByIdAsync(targetBookLoan.Id);

            // Assert
            if (isUserAuthenticated)
            {
                Assert.IsType<RedirectToActionResult>(result);
                Assert.NotNull(closedBookLoan!.ClosedTime);
                Assert.Equal(closedBookLoan.ClosedTime.Value.Date, DateTime.Now.Date);
                Assert.Equal(targetBookLoan.StartTime, closedBookLoan.StartTime);
                Assert.Equal(targetBookLoan.EndTime, closedBookLoan.EndTime);
            }
            else
            {
                Assert.IsType<UnauthorizedResult>(result);
            }            
        }

        [InlineData(true)]
        [InlineData(false)]
        [Theory]
        public async Task TestCloseClosedBookLoan(bool isUserAuthenticated)
        {
            // Arrange
            int numLoans = 1;
            var user = await GetDefaultUser();
            var memberController = await CreateMemberController(isUserAuthenticated);
            await SeedBookLoans(user, closedLoans: numLoans);
            BookLoan targetBookLoan = (await _bookLoanRepository.GetClosedBookLoansAsync(user.Id)).First();

            if (isUserAuthenticated)
            {
                // Act
                // Assert
                await Assert.ThrowsAsync<BookLoanClosedException>(async () => await memberController.CloseLoan(targetBookLoan.Id));
            }
            else
            {
                // Act
                var result = await memberController.CloseLoan(targetBookLoan.Id);
                
                // Assert
                Assert.IsType<UnauthorizedResult>(result);
            }
        }

        [InlineData(true)]
        [InlineData(false)]
        [Theory]
        public async Task TestCloseNonExistentBookLoan(bool isUserAuthenticated)
        {
            // Arrange
            var memberController = await CreateMemberController(isUserAuthenticated);

            // Act
            var result = await memberController.CloseLoan(0);

            // Assert
            if (isUserAuthenticated)
            {
                Assert.IsType<NotFoundResult>(result);
            }
            else
            {
                Assert.IsType<UnauthorizedResult>(result);
            }
        }

        [MemberData(nameof(GetTestProlongActiveBookLoanInputData))]
        [Theory]
        public async Task TestProlongActiveBookLoan(bool isUserAuthenticated, int startTimeOffsetDays)
        {
            // Arrange
            int numLoans = 1;
            var user = await GetDefaultUser();
            var memberController = await CreateMemberController(isUserAuthenticated);
            DateTime seedTime = DateTime.Now;
            await SeedBookLoans(user, activeLoans: numLoans, startTimeOffset: TimeSpan.FromDays(startTimeOffsetDays));
            BookLoan targetBookLoan = (await _bookLoanRepository.GetActiveBookLoansAsync(user.Id)).First();

            // Act
            var result = await memberController.ProlongLoan(targetBookLoan.Id);
            var prolongedBookLoan = await _bookLoanRepository.GetBookLoanByIdAsync(targetBookLoan.Id);

            // Assert
            if (isUserAuthenticated)
            {
                Assert.IsType<RedirectToActionResult>(result);
                Assert.Null(targetBookLoan!.ClosedTime);
                Assert.Equal(targetBookLoan.StartTime, prolongedBookLoan!.StartTime);
                Assert.True(prolongedBookLoan.EndTime.Date == DateTime.Now.Date.AddDays(BookLoanTime - 1));
                Assert.Equal(user.Id, prolongedBookLoan.User.Id);
                Assert.Equal(targetBookLoan.Book.BookId, prolongedBookLoan.Book.BookId);
            }
            else
            {
                Assert.IsType<UnauthorizedResult>(result);
            }
        }

        [InlineData(true)]
        [InlineData(false)]
        [Theory]
        public async Task TestProlongClosedBookLoan(bool isUserAuthenticated)
        {
            // Arrange
            int numLoans = 1;
            var user = await GetDefaultUser();
            var memberController = await CreateMemberController(isUserAuthenticated);
            DateTime seedTime = DateTime.Now;
            await SeedBookLoans(user, closedLoans: numLoans);
            BookLoan targetBookLoan = (await _bookLoanRepository.GetClosedBookLoansAsync(user.Id)).First();

            if (isUserAuthenticated)
            {
                // Act
                // Assert
                await Assert.ThrowsAsync<BookLoanClosedException>(async () => await memberController.ProlongLoan(targetBookLoan.Id));
            }
            else
            {
                // Act
                var result = await memberController.ProlongLoan(targetBookLoan.Id);

                // Assert
                Assert.IsType<UnauthorizedResult>(result);
            }
        }

        [InlineData(true)]
        [InlineData(false)]
        [Theory]
        public async Task TestProlongNonExistentBookLoan(bool isUserAuthenticated)
        {
            // Arrange
            var memberController = await CreateMemberController(isUserAuthenticated);

            // Act
            var result = await memberController.ProlongLoan(0);

            // Assert
            if (isUserAuthenticated)
            {
                Assert.IsType<NotFoundResult>(result);
            }
            else
            {
                Assert.IsType<UnauthorizedResult>(result);
            }
        }

        #endregion
    }
}
