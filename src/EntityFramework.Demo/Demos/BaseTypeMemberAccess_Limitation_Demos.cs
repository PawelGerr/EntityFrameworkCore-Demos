using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using EntityFramework.Demo.Model;
using FluentAssertions.Common;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace EntityFramework.Demo.Demos
{
   public class BaseTypeMemberAccess_Limitation_Demos : DemosBase
   {
      /// <inheritdoc />
      public BaseTypeMemberAccess_Limitation_Demos(DemoDbContext ctx, ILogger<BaseTypeMemberAccess_Limitation_Demos> logger)
         : base(ctx, logger)
      {
      }

      public void LoadData()
      {
         Expression<Func<ProductWrapper, bool>> predicate = GetPredicate<ProductWrapper>();

         // Generates statement:
         //    SELECT
         //       [p].[Id], [p].[GroupId], [p].[Name], [p].[RowVersion]
         //    FROM
         //       [Products] AS [p]
         //
         // Generates warning:
         //    The LINQ expression 'where (Convert(new ProductWrapper() {Product = [p]}, IProductWrapper).Product.Name != null)' could not be translated and will be evaluated locally.
         //
         // the filtering happens in .NET instead of in database
         var query = Context.Products
                            .Select(p => new ProductWrapper { Product = p })
                            .Where(predicate);

         var products = query.ToList();

         // Generates expected statement:
         //    SELECT
         //       [p].[Id], [p].[GroupId], [p].[Name], [p].[RowVersion]
         //    FROM
         //       [Products] AS [p]
         //    WHERE
         //       [p].[Name] IS NOT NULL
         //
         // the filtering happens in database
         var query2 = Context.Products
                             .Select(p => new ProductWrapper { Product = p })
                             .Where(wrapper => wrapper.Product.Name != null);

         var products2 = query2.ToList();

         // no success => conversion is not the culprit
         DoExperiment_RemoveConversion(predicate);

         // success => EF Core stops translating query
         // probably because the property "Product" in "Select" belongs to "ProductWrapper"
         // and the property "Product" in "GetPredicate" belongs to "IProductWrapper"
         DoExperiment_RewriteMemberAccess(predicate);
      }

      private void DoExperiment_RemoveConversion(Expression<Func<ProductWrapper, bool>> predicate)
      {
         var visitor = new ConversionExpressionEliminatingVisitor();
         var newPredicate = (Expression<Func<ProductWrapper, bool>>)visitor.Visit(predicate);

         // predicate:     wrapper => (Convert(wrapper, IProductWrapper).Product.Name != null)
         // new predicate: wrapper => (wrapper.Product.Name != null)

         // Generates statement:
         //    SELECT
         //       [p].[Id], [p].[GroupId], [p].[Name], [p].[RowVersion]
         //    FROM
         //       [Products] AS [p]
         //
         // Generates warning:
         //    The LINQ expression 'where (new ProductWrapper() {Product = [p]}.Product.Name != null)' could not be translated and will be evaluated locally.
         //
         // the filtering happens in .NET instead of in database
         var query = Context.Products
                            .Select(p => new ProductWrapper { Product = p })
                            .Where(newPredicate);

         var products = query.ToList();
      }

      private void DoExperiment_RewriteMemberAccess(Expression<Func<ProductWrapper, bool>> predicate)
      {
         var visitor = new MemberExpressionRelinqVisitor();
         var newPredicate = (Expression<Func<ProductWrapper, bool>>)visitor.Visit(predicate);

         // predicate:     wrapper => (Convert(wrapper, IProductWrapper).Product.Name != null)
         // new predicate: wrapper => (wrapper.Product.Name != null)

         // Generates expected statement:
         //    SELECT
         //       [p].[Id], [p].[GroupId], [p].[Name], [p].[RowVersion]
         //    FROM
         //       [Products] AS [p]
         //    WHERE
         //       [p].[Name] IS NOT NULL
         //
         // the filtering happens in database
         var query = Context.Products
                            .Select(p => new ProductWrapper { Product = p })
                            .Where(newPredicate);

         var products = query.ToList();
      }

      [NotNull]
      private static Expression<Func<T, bool>> GetPredicate<T>()
         where T : IProductWrapper
      {
         return wrapper => wrapper.Product.Name != null;
      }

      private class ProductWrapper : IProductWrapper
      {
         public Product Product { get; set; }
      }

      private interface IProductWrapper
      {
         Product Product { get; set; }
      }

      private class ConversionExpressionEliminatingVisitor : ExpressionVisitor
      {
         protected override Expression VisitUnary(UnaryExpression node)
         {
            if (node.NodeType == ExpressionType.Convert)
            {
               if (node.Type.IsAssignableFrom(node.Operand.Type))
                  return node.Operand;
            }

            return base.VisitUnary(node);
         }
      }

      private class MemberExpressionRelinqVisitor : ExpressionVisitor
      {
         protected override Expression VisitMember(MemberExpression node)
         {
            if (node.Expression.NodeType == ExpressionType.Convert)
            {
               var conversion = (UnaryExpression)node.Expression;

               if (conversion.Type.IsAssignableFrom(conversion.Operand.Type))
               {
                  var memberType = GetMemberReturnType(node.Member);
                  var member = (MemberInfo)conversion.Operand.Type.FindProperty(node.Member.Name, memberType)
                               ?? conversion.Operand.Type.FindField(node.Member.Name, memberType)
                               ?? throw new Exception("Member not found.");

                  return Expression.MakeMemberAccess(conversion.Operand, member);
               }
            }

            return base.VisitMember(node);
         }
      }

      [NotNull]
      private static Type GetMemberReturnType([NotNull] MemberInfo member)
      {
         if (member == null)
            throw new ArgumentNullException(nameof(member));

         if (member is PropertyInfo propInfo)
            return propInfo.PropertyType;

         if (member is FieldInfo fieldInfo)
            return fieldInfo.FieldType;

         if (member is MethodInfo methodInfo)
            return methodInfo.ReturnType;

         throw new ArgumentException($"Argument must be {nameof(FieldInfo)}, {nameof(PropertyInfo)} or {nameof(MethodInfo)}.", nameof(member));
      }
   }
}
