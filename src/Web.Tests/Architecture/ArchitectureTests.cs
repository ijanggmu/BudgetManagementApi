//using System.Reflection;
//using Business.BeemaEdgeApi;
//using Data.Context;
//using Data.Entities;
//using Models.Common;
//using Xunit;

//namespace Web.Tests.Architecture;

//public class ArchitectureTests
//{
//    [Fact]
//    public void Domain_Entities_Should_Be_In_Data_Layer()
//    {
//        // Arrange
//        var entityBaseType = typeof(BaseEntity);
//        var entityAssembly = Assembly.GetAssembly(typeof(BaseEntity));

//        // Act
//        var entityTypes = entityAssembly.GetTypes()
//            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(entityBaseType));

//        // Assert
//        foreach (var entityType in entityTypes)
//        {
//            Assert.StartsWith("Data.Entities", entityType.Namespace,
//                $"Entity {entityType.Name} should be in the Data.Entities namespace");
//        }
//    }

//    [Fact]
//    public void Services_Should_Be_In_Business_Layer()
//    {
//        // Arrange
//        var serviceAssembly = Assembly.GetAssembly(typeof(TodoService));

//        // Act
//        var serviceTypes = serviceAssembly.GetTypes()
//            .Where(t => t.Name.EndsWith("Service") && !t.IsInterface);

//        // Assert
//        foreach (var serviceType in serviceTypes)
//        {
//            Assert.StartsWith("Business", serviceType.Namespace,
//                $"Service {serviceType.Name} should be in the Business namespace");
//        }
//    }

//    [Fact]
//    public void Models_Should_Not_Have_Dependencies_On_Other_Layers()
//    {
//        // Arrange
//        var modelAssembly = Assembly.GetAssembly(typeof(CommonResponseModel));
//        var forbiddenNamespaces = new[] { "Data", "Business", "Infrastructure" };

//        // Act
//        var modelTypes = modelAssembly.GetTypes()
//            .Where(t => t.Namespace?.StartsWith("Models") == true);

//        // Assert
//        foreach (var modelType in modelTypes)
//        {
//            var references = modelType.GetReferencedTypes();
//            foreach (var reference in references)
//            {
//                foreach (var forbiddenNamespace in forbiddenNamespaces)
//                {
//                    Assert.False(reference.Namespace?.StartsWith(forbiddenNamespace) == true,
//                        $"Model {modelType.Name} should not reference {reference.FullName}");
//                }
//            }
//        }
//    }
//}

//public static class TypeExtensions
//{
//    public static IEnumerable<Type> GetReferencedTypes(this Type type)
//    {
//        var types = new HashSet<Type>();
        
//        // Properties
//        types.UnionWith(type.GetProperties().Select(p => p.PropertyType));
        
//        // Methods
//        types.UnionWith(type.GetMethods().SelectMany(m => 
//            m.GetParameters().Select(p => p.ParameterType)
//            .Concat(new[] { m.ReturnType })));

//        return types;
//    }
//} 
