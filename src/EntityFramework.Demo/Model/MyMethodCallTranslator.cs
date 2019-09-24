using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFramework.Demo.Model
{
   public class MyMethodCallTranslator : IMethodCallTranslator
   {
      private static readonly MethodInfo _myDbFunction = typeof(DbFunctionsExtensions).GetMethod(nameof(DbFunctionsExtensions.MyDbFunction), BindingFlags.Static | BindingFlags.Public);

      public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments)
      {
         if (method == _myDbFunction)
         {
            return new SqlConstantExpression(Expression.Constant(42), RelationalTypeMapping.NullMapping);
         }

         return null;
      }
   }
}
