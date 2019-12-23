using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFramework.Demo.Model
{
   public class RowVersionValueConverter : ValueConverter<ulong, byte[]>
   {
      public RowVersionValueConverter()
         : base(GetToBytesExpression(), GetToNumberExpression())
      {
      }

      private static Expression<Func<byte[], ulong>> GetToNumberExpression()
      {
         if (!BitConverter.IsLittleEndian)
            return bytes => ConvertBytes(bytes);

         return bytes => ConvertBytes(ReverseLong(bytes));
      }

      private static ulong ConvertBytes(byte[] bytes)
      {
         return bytes.Length == 0 ? 0 : BitConverter.ToUInt64(bytes, 0);
      }

      private static Expression<Func<ulong, byte[]>> GetToBytesExpression()
      {
         if (!BitConverter.IsLittleEndian)
            return rowVersion => BitConverter.GetBytes(rowVersion);

         return rowVersion => ReverseLong(BitConverter.GetBytes(rowVersion));
      }

      private static byte[] ReverseLong(byte[] bytes)
      {
         if (bytes.Length == 0)
            return bytes;

         return new[]
                {
                   bytes[7],
                   bytes[6],
                   bytes[5],
                   bytes[4],
                   bytes[3],
                   bytes[2],
                   bytes[1],
                   bytes[0]
                };
      }
   }
}
