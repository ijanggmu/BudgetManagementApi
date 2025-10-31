//using Business.BeemaEdgeApi;
//using Data.Context;
//using Data.Entities.Todo;
//using Infrastructure.Common.UserProfile;
//using Microsoft.EntityFrameworkCore;
//using Moq;

//namespace Web.Tests.Integration;

//public class TodoServiceIntegrationTests : IDisposable
//{
//    private readonly ApplicationDataContext _context;
//    private readonly TodoService _todoService;
//    private readonly string _testUserId = "test-user-id";

//    public TodoServiceIntegrationTests()
//    {
//        var mockUserProfileService = new Mock<IUserProfileService>();
//        mockUserProfileService.Setup(x => x.GetUserId()).Returns(_testUserId);

//        var options = new DbContextOptionsBuilder<ApplicationDataContext>()
//            .UseInMemoryDatabase(databaseName: $"TodoTestDb_{Guid.NewGuid()}")
//            .Options;

//        _context = new ApplicationDataContext(options, mockUserProfileService.Object);
//        _todoService = new TodoService(_context);
//    }

//    [Fact]
//    public async Task CreateAndRetrieveTodoList_ShouldWorkEndToEnd()
//    {
//        // Arrange
//        var newTodoList = new TodoList
//        {
//            Title = "Integration Test Todo List",
//        };

//        // Act - Create
//        var createResult = await _todoService.CreateAsync(newTodoList, CancellationToken.None);

//        // Assert - Create
//        Assert.NotNull(createResult);
//        Assert.True(createResult.IsSuccess);
//        Assert.NotNull(createResult.Data);
//        Assert.NotNull(createResult.Data.Id);

//        // Act - Retrieve
//        var getResult = await _todoService.GetByIdAsync(createResult.Data.Id, CancellationToken.None);

//        // Assert - Retrieve
//        Assert.NotNull(getResult);
//        Assert.True(getResult.IsSuccess);
//        Assert.Equal(newTodoList.Title, getResult.Data.Title);
//    }

//    [Fact]
//    public async Task UpdateTodoList_ShouldUpdateAllFields()
//    {
//        // Arrange
//        var todoList = new TodoList
//        {
//            Title = "Original Title",
//        };
//        await _context.TodoLists.AddAsync(todoList);
//        await _context.SaveChangesAsync();

//        // Act
//        todoList.Title = "Updated Title";
//        var updateResult = await _todoService.UpdateAsync(todoList, CancellationToken.None);

//        // Assert
//        Assert.NotNull(updateResult);
//        Assert.True(updateResult.IsSuccess);

//        var updatedTodoList = await _context.TodoLists.FindAsync(todoList.Id);
//        Assert.NotNull(updatedTodoList);
//        Assert.Equal("Updated Title", updatedTodoList.Title);
//    }

//    [Fact]
//    public async Task DeleteTodoList_ShouldRemoveFromDatabase()
//    {
//        // Arrange
//        var todoList = new TodoList
//        {
//            Title = "To Be Deleted",
//        };
//        await _context.TodoLists.AddAsync(todoList);
//        await _context.SaveChangesAsync();

//        // Act
//        var deleteResult = await _todoService.DeleteAsync(todoList.Id, CancellationToken.None);

//        // Assert
//        Assert.NotNull(deleteResult);
//        Assert.True(deleteResult.IsSuccess);

//        var deletedTodoList = await _context.TodoLists.FindAsync(todoList.Id);
//        Assert.Null(deletedTodoList);
//    }

//    public void Dispose()
//    {
//        _context.Database.EnsureDeleted();
//        _context.Dispose();
//    }
//}
