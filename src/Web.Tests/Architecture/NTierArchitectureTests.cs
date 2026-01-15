using System.Reflection;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Data.Context;

namespace BeemaEdgeApi.Tests.Architecture;

public class NTierArchitectureTests
{
    private readonly Assembly _presentationLayer;  // Web API
    private readonly Assembly _businessLayer;      // Business
    private readonly Assembly _dataLayer;          // Data

    public NTierArchitectureTests()
    {
        _presentationLayer = Assembly.GetAssembly(typeof(BaseApiController));
        //_businessLayer = Assembly.GetAssembly(typeof(TodoService));
        _dataLayer = Assembly.GetAssembly(typeof(ApplicationDataContext));
        //_modelLayer = Assembly.GetAssembly(typeof(CommonResponseModel));
    }

    [Fact]
    public void PresentationLayer_Should_Only_Reference_Business_And_Models()
    {
        // Arrange
        var allowedReferences = new[] { "Business", "Models", "Microsoft", "System" };
        var references = _presentationLayer.GetReferencedAssemblies();

        // Assert
        foreach (var reference in references)
        {
            bool isAllowed = allowedReferences.Any(allowed =>
                reference.Name.StartsWith(allowed, StringComparison.OrdinalIgnoreCase));
            Assert.True(isAllowed,
                $"Presentation layer should not reference {reference.Name}");
        }
    }

    [Fact]
    public void BusinessLayer_Should_Not_Reference_Presentation()
    {
        // Arrange
        var references = _businessLayer.GetReferencedAssemblies();

        // Assert
        Assert.False(references.Any(r =>
            r.Name.StartsWith("BeemaEdgeApi", StringComparison.OrdinalIgnoreCase)),
            "Business layer should not reference presentation layer");
    }

    [Fact]
    public void DataLayer_Should_Not_Reference_Business_Or_Presentation()
    {
        // Arrange
        var forbiddenReferences = new[] { "BeemaEdgeApi", "Business" };
        var references = _dataLayer.GetReferencedAssemblies();

        // Assert
        foreach (var reference in references)
        {
            Assert.False(forbiddenReferences.Any(forbidden =>
                reference.Name.StartsWith(forbidden, StringComparison.OrdinalIgnoreCase)),
                $"Data layer should not reference {reference.Name}");
        }
    }

    [Fact]
    public void Controllers_Should_Use_Services()
    {
        // Arrange
        var controllerTypes = _presentationLayer.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Controller"));

        // Assert
        foreach (var controllerType in controllerTypes)
        {
            var constructor = controllerType.GetConstructors().FirstOrDefault();
            Assert.NotNull(constructor);

            var parameters = constructor.GetParameters();
            if (parameters.Length == 0)
                return;
            Assert.Contains(parameters, p =>
                p.ParameterType.Name.EndsWith("Service"));
        }
    }

    //[Fact]
    //public void Services_Should_Use_Repository_Or_DbContext()
    //{
    //    // Arrange
    //    var serviceTypes = _businessLayer.GetTypes()
    //        .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service"));

    //    // Assert
    //    foreach (var serviceType in serviceTypes)
    //    {
    //        var constructor = serviceType.GetConstructors().FirstOrDefault();
    //        Assert.NotNull(constructor);

    //        var parameters = constructor.GetParameters();
    //        Assert.Contains(parameters, p =>
    //            p.ParameterType == typeof(ApplicationDataContext) ||
    //            p.ParameterType.Name.EndsWith("Repository"));
    //    }
    //}

    //[Fact]
    //public void Models_Should_Be_Pure_Data_Objects()
    //{
    //    // Arrange
    //    var modelTypes = _modelLayer.GetTypes()
    //        .Where(t => t.Namespace?.StartsWith("Models") == true);

    //    // Assert
    //    foreach (var modelType in modelTypes)
    //    {
    //        // Models should not have dependencies on other layers
    //        var methods = modelType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
    //        var nonPropertyMethods = methods.Where(m => !m.IsSpecialName);  // Excludes property getters/setters

    //        Assert.Empty(nonPropertyMethods.Where(m => !m.Name.Equals("ToString")),
    //            $"Model {modelType.Name} should only contain properties and basic methods");
    //    }
    //}

    //[Fact]
    //public void Entities_Should_Not_Have_Service_Dependencies()
    //{
    //    // Arrange
    //    var entityTypes = _dataLayer.GetTypes()
    //        .Where(t => t.Namespace?.StartsWith("Data.Entities") == true);

    //    // Assert
    //    foreach (var entityType in entityTypes)
    //    {
    //        var references = entityType.GetReferencedTypes();
    //        Assert.False(references.Any(r => r.Name.EndsWith("Service")),
    //            $"Entity {entityType.Name} should not have dependencies on services");
    //    }
    //}
}
