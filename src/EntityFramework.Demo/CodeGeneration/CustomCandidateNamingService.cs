using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

#pragma warning disable EF1001

namespace EntityFramework.Demo.CodeGeneration
{
   public class CustomCandidateNamingService : CandidateNamingService
   {
      /// <inheritdoc />
      public override string GetDependentEndCandidateNavigationPropertyName(IForeignKey foreignKey)
      {
         if (foreignKey.PrincipalKey.IsPrimaryKey())
            return foreignKey.PrincipalEntityType.ShortName();

         return base.GetDependentEndCandidateNavigationPropertyName(foreignKey);
      }
   }
}
