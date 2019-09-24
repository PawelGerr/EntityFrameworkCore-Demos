using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Query;

namespace EntityFramework.Demo.Model
{
   public class MyMethodCallTranslatorPlugin : IMethodCallTranslatorPlugin
   {
      public IEnumerable<IMethodCallTranslator> Translators { get; }

      public MyMethodCallTranslatorPlugin()
      {
         Translators = new[] { new MyMethodCallTranslator() };
      }
   }
}
