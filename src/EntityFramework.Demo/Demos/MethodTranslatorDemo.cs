using System;
using System.Linq;
using EntityFramework.Demo.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EntityFramework.Demo.Demos
{
   public class MethodTranslatorDemo
   {
      private readonly DemoDbContext _ctx;
      private readonly ILogger<MethodTranslatorDemo> _logger;

      public MethodTranslatorDemo(DemoDbContext ctx, ILogger<MethodTranslatorDemo> logger)
      {
         _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
         _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      }

      public void MethodCallWithOneColumnIsTranslated()
      {
         /**
          * Generated query:
          *
          * SELECT 42 AS [Foo]
          * FROM [Products] AS [p]
          */
         var result = _ctx.Products
                          .Select(p => new
                                       {
                                          Foo = EF.Functions.MyDbFunction(p.Id)
                                       })
                          .ToList();

         _logger.LogInformation($"Custom function is translated: \"EF.Functions.MyDbFunction(p.Id)\". Value of Foo is '{result.First().Foo}'");
      }

      public void MethodCallWithNewExpressionThrows()
      {
         try
         {
            var result = _ctx.Products
                             .Select(p => new
                                          {
                                             Foo = EF.Functions.MyDbFunction(new
                                                                             {
                                                                                p.Id,
                                                                                p.Name
                                                                             })
                                          })
                             .ToList();

            _logger.LogInformation($"Custom function is translated: \"EF.Functions.MyDbFunction(new {{ p.Id, p.Name }})\". Value of Foo is '{result.First().Foo}'");
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, "Error during translation of custom function: \"EF.Functions.MyDbFunction(new {{ p.Id, p.Name }})\".");
         }
      }

      public void MethodCallWithArrayThrows()
      {
         try
         {
            var result = _ctx.Products
                             .Select(p => new
                                          {
                                             Foo = EF.Functions.MyDbFunction(new object[] { p.Id, p.Name })
                                          })
                             .ToList();

            _logger.LogInformation($"Custom function is translated: \"EF.Functions.MyDbFunction(new object[] {{ p.Id, p.Name }})\". Value of Foo is '{result.First().Foo}'");
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, $"Error during translation of custom function: \"EF.Functions.MyDbFunction(new object[] {{ p.Id, p.Name }})\".");
         }
      }
   }
}
