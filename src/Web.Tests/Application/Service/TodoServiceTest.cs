using Data.Context;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Web.Tests.Application.Service;
public class TodoServiceTests
{
    private readonly ApplicationDataContext _context;
    //private readonly TodoService _sut; // System Under Test
    //private ApplicationDataContext GetInMemoryDbContext()
    //{

    //    // Mock the IUserProfileService
    //    var mockUserProfileService = new Mock<IUserProfileService>();
    //    mockUserProfileService.Setup(x => x.GetUserId()).Returns("test-user-id");

    //    // Configure InMemory database
    //    var options = new DbContextOptionsBuilder<ApplicationDataContext>()
    //        .UseInMemoryDatabase(databaseName: "TodoTestDb")
    //        .Options;

    //    var context = new ApplicationDataContext(options, mockUserProfileService.Object);

    //    // Ensure database is clean for each test
    //    context.Database.EnsureDeleted();
    //    context.Database.EnsureCreated();

    //    return context;
    //}
    public TodoServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDataContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        //_context = GetInMemoryDbContext();

        //_sut = new TodoService(_context);
    }

    //[Fact]
    //public async Task GetAllAsync_ShouldReturnAllTodoLists()
    //{
    //    // Arrange
    //    var expectedLists = new List<TodoList>
    //    {
    //        new() { Id = Guid.NewGuid().ToString(), Title = "Test List 1" },
    //        new() { Id = Guid.NewGuid().ToString(), Title = "Test List 2" }
    //    };
    //    await _context.TodoLists.AddRangeAsync(expectedLists);
    //    await _context.SaveChangesAsync();

    //    // Act
    //    var result = await _sut.GetAllAsync(CancellationToken.None);

    //    // Assert
    //    Assert.NotNull(result);
    //    Assert.Equal(expectedLists.Count, result.Data.Count());
    //    Assert.Equal(expectedLists, result.Data);
    //}

    //[Fact]
    //public async Task GetByIdAsync_ShouldReturnTodoList_WhenExists()
    //{
    //    // Arrange
    //    var todoId = Guid.NewGuid().ToString();
    //    var expectedList = new TodoList { Id = todoId, Title = "Test List" };
    //    await _context.TodoLists.AddAsync(expectedList);
    //    await _context.SaveChangesAsync();
    //    // Act
    //    var result = await _sut.GetByIdAsync(todoId, CancellationToken.None);

    //    // Assert
    //    Assert.NotNull(result);
    //    Assert.Equal(expectedList.Id, result.Data.Id);
    //    Assert.Equal(expectedList.Title, result.Data.Title);
    //}

    //[Fact]
    //public async Task CreateAsync_ShouldAddAndSaveTodoList()
    //{
    //    // Arrange
    //    var newList = new TodoList { Title = "New List" };
    //    var todoId = Guid.NewGuid().ToString();

    //    var context = GetInMemoryDbContext();
    //    // Act
    //    var result = await _sut.CreateAsync(newList, CancellationToken.None);

    //    // Assert
    //    Assert.NotNull(result);
    //    Assert.Equal(todoId, result.Data.Id);
    //    Assert.Equal(newList.Title, result.Data.Title);
    //    _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    //}

    //[Fact]
    //public async Task UpdateAsync_ShouldUpdateAndSaveChanges()
    //{
    //    var todoId = Guid.NewGuid().ToString();

    //    // Arrange
    //    var updatedList = new TodoList { Id = todoId, Title = "Updated List" };

    //    // Act
    //    await _sut.UpdateAsync(updatedList, CancellationToken.None);

    //    // Assert
    //    _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    //}

    //[Fact]
    //public async Task DeleteAsync_ShouldRemoveAndSaveChanges_WhenExists()
    //{
    //    var todoId = Guid.NewGuid().ToString();

    //    // Arrange
    //    var existingList = new TodoList { Id = todoId, Title = "Test List" };

    //    _contextMock
    //        .Setup(x => x.TodoLists.FindAsync(
    //            It.Is<object[]>(p => p[0].Equals(1)),
    //            It.IsAny<CancellationToken>()))
    //        .ReturnsAsync(existingList);

    //    // Act
    //    await _sut.DeleteAsync(todoId, CancellationToken.None);

    //    // Assert
    //    _contextMock.Verify(x => x.TodoLists.Remove(existingList), Times.Once);
    //    _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    //}

    //[Fact]
    //public async Task DeleteAsync_ShouldNotRemoveOrSave_WhenNotExists()
    //{
    //    var todoId = Guid.NewGuid().ToString();

    //    // Arrange
    //    _contextMock
    //        .Setup(x => x.TodoLists.FindAsync(
    //            It.Is<object[]>(p => p[0].Equals(1)),
    //            It.IsAny<CancellationToken>()))
    //        .ReturnsAsync((TodoList)null);

    //    // Act
    //    await _sut.DeleteAsync(todoId, CancellationToken.None);

    //    // Assert
    //    _contextMock.Verify(x => x.TodoLists.Remove(It.IsAny<TodoList>()), Times.Never);
    //    _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    //}
}
