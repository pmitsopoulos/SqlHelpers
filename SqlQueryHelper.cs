using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlHelpers
{
    public static class SqlQueryHelper
    {
      static string CreateBulkInsertSql<T>(IEnumerable<T> importEntities)
        {
            var props = typeof(T).GetProperties();
            var tableName = typeof(T).Name;  
            var propIndexingDictionary = new Dictionary<string, int>();
            var indexer = 0;
            foreach (var prop in props)
            {
                propIndexingDictionary.Add(prop.Name,indexer);
                indexer++;
            }
            var columnNames = propIndexingDictionary.Keys.ToArray();
            var initStatement = $"INSERT INTO {tableName} ({String.Join(",",columnNames)}) VALUES {Environment.NewLine}";
            var valuesPlaceholder = "({0})";
            var valueStatementsStrings = new List<string>();
            foreach (var entity in importEntities)
            {
                var entityValues = new object[columnNames.Length];
                foreach(var prop in props)
                {
                    var index = -1;
                    if(propIndexingDictionary.TryGetValue(prop.Name, out index))
                    {
                        object val;
                        if (!prop.GetValue(entity).GetType().IsNumeric())
                        {
                            val = $"'{prop.GetValue(entity)}'";
                        }
                        else
                        {
                            val= prop.GetValue(entity);
                        }
                        entityValues[index] = val;
                    } 
                }
                valueStatementsStrings.Add($"{String.Format(valuesPlaceholder,String.Join(",", entityValues))}");
            }
            return initStatement + String.Join($",{Environment.NewLine}", valueStatementsStrings);
        }
      static bool IsNumeric(this Type _type)
        {
            switch (Type.GetTypeCode(_type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                case TypeCode.Object:
                    return false;
                default:
                    return false;
            }
        }  
    }
}