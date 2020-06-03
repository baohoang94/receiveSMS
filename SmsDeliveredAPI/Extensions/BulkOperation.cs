using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SmsDeliveredAPI.Extensions
{
    public static class SqlServerBulkOperation
    {
        /// <summary>
        /// Insert parent child(if exist) in database using SQL bulk copy 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="parentTableName"></param>
        public static bool BulkInsert<T>(this DbContext db, IEnumerable<T> list, string parentTableName) where T : class
        {

            using (SqlConnection conn = new SqlConnection(db.Database.GetDbConnection().ConnectionString))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    using (var bulkCopy = new SqlBulkCopy(db.Database.GetDbConnection().ConnectionString, SqlBulkCopyOptions.Default) { DestinationTableName = parentTableName })
                    {
                        using (var childBulkCopy = new SqlBulkCopy(db.Database.GetDbConnection().ConnectionString, SqlBulkCopyOptions.Default))
                        {
                            var table = new DataTable();
                            var childTable = new DataTable();
                            var columnNames = new List<string>();
                            var childColumnNames = new List<string>();
                            var items = list.ToList();
                            // Lấy các Properties của class T
                            PropertyInfo[] props = typeof(T).GetProperties();
                            foreach (PropertyInfo prop in props)
                            {
                                // Lấy các attributes của property
                                object[] attributes = prop.GetCustomAttributes(true);
                                var attrs = attributes.Where(x => x.GetType() == typeof(ColumnAttribute)).ToList();
                                foreach (ColumnAttribute attr in attrs)
                                {
                                    if (attr != null)
                                    {
                                        Type propertyType;
                                        //// check if nullable type if yes then select type 
                                        if (prop.PropertyType.IsGenericType &&
                                            prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                        {
                                            propertyType = prop.PropertyType.GetGenericArguments()[0];
                                        }
                                        else
                                        {
                                            propertyType = prop.PropertyType;
                                        }
                                        table.Columns.Add(new DataColumn(attr.Name, propertyType));
                                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(attr.Name, attr.Name));
                                        columnNames.Add(prop.Name);
                                    }
                                }
                                if (attributes.FirstOrDefault(x => x.GetType() == typeof(ChildAttribute)) is ChildAttribute childAttrs)
                                {
                                    childBulkCopy.DestinationTableName = childAttrs.Name;
                                    var childObject = prop.PropertyType;
                                    Type childObjectType;
                                    //// check if child type is single object or list of object
                                    if (childObject.Name.Contains("ICollection"))
                                    {
                                        childObjectType = childObject.GetGenericArguments().FirstOrDefault();
                                    }
                                    else
                                    {
                                        childObjectType = childObject;
                                    }
                                    if (childObjectType != null)
                                    {
                                        string parentName = string.Empty;
                                        PropertyInfo[] childProps = childObjectType.GetProperties();
                                        foreach (PropertyInfo cProp in childProps)
                                        {
                                            object[] childAttributes = cProp.GetCustomAttributes(true);
                                            var childsAttrs = childAttributes.Where(x => x.GetType() == typeof(ColumnAttribute)).ToList();
                                            if (childAttributes.FirstOrDefault(x => x.GetType() == typeof(ForeignKeyAttribute)) is ForeignKeyAttribute parentAttrsName)
                                                parentName = parentAttrsName.Name;
                                            foreach (ColumnAttribute cAttr in childsAttrs)
                                            {
                                                if (cAttr != null)
                                                {
                                                    Type propertyType;
                                                    if (cProp.PropertyType.IsGenericType &&
                                                        cProp.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                                    {
                                                        propertyType = cProp.PropertyType.GetGenericArguments()[0];
                                                    }
                                                    else
                                                    {
                                                        propertyType = cProp.PropertyType;
                                                    }
                                                    childTable.Columns.Add(new DataColumn(cAttr.Name, propertyType));
                                                    childBulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(cAttr.Name, cAttr.Name));
                                                    childColumnNames.Add(cProp.Name);
                                                }
                                            }
                                        }
                                        for (int itemIndex = 0; itemIndex < items.Count; itemIndex++)
                                        {
                                            dynamic childItems = items[itemIndex].GetType().GetProperty(prop.Name).GetValue(items[itemIndex], null);
                                            bool isEnumerable = (childItems as System.Collections.IEnumerable) != null;
                                            if (isEnumerable)
                                            {
                                                for (int i = 0; i < childItems.Count; i++)
                                                {
                                                    var dataItems = new List<object>();
                                                    for (int j = 0; j < childColumnNames.Count; j++)
                                                    {
                                                        object value = null;
                                                        if (parentName == childColumnNames[j])
                                                        {
                                                            PropertyInfo[] parentProps = items[itemIndex].GetType().GetProperties();
                                                            foreach (PropertyInfo pProp in parentProps)
                                                            {
                                                                object[] parentAttributes = pProp.GetCustomAttributes(true);
                                                                var parentAttrs = parentAttributes.FirstOrDefault(x => x.GetType() == typeof(KeyAttribute));
                                                                if (parentAttrs != null)
                                                                {
                                                                    value = items[itemIndex].GetType().GetProperty(pProp.Name).GetValue(items[itemIndex], null);
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            value = childItems[i].GetType().GetProperty(childColumnNames[j]).GetValue(childItems[i], null);
                                                        }

                                                        if (value != null)
                                                        {
                                                            ////assign the value
                                                            dataItems.Add(value);
                                                        }
                                                        else
                                                        {
                                                            dataItems.Add(DBNull.Value);
                                                        }
                                                    }
                                                    childTable.Rows.Add(dataItems.ToArray());
                                                }
                                            }
                                            else
                                            {
                                                var dataItems = new List<object>();
                                                for (int j = 0; j < childColumnNames.Count; j++)
                                                {
                                                    object value = childItems.GetType().GetProperty(columnNames[j]).GetValue(childItems, null);
                                                    if (value != null)
                                                    {
                                                        ////assign the value
                                                        dataItems.Add(value);
                                                    }
                                                    else
                                                    {
                                                        dataItems.Add(DBNull.Value);
                                                    }
                                                }
                                                childTable.Rows.Add(dataItems.ToArray());
                                            }
                                        }
                                    }
                                }
                            }
                            for (int i = 0; i < items.Count; i++)
                            {
                                var dataItems = new List<object>();
                                for (int j = 0; j < columnNames.Count; j++)
                                {
                                    object value = items[i].GetType().GetProperty(columnNames[j]).GetValue(items[i], null);
                                    if (value != null)
                                    {
                                        ////assign the value
                                        dataItems.Add(value);
                                    }
                                    else
                                    {
                                        dataItems.Add(DBNull.Value);
                                    }
                                }
                                table.Rows.Add(dataItems.ToArray());
                            }
                            bulkCopy.WriteToServer(table);
                            if (childTable.Rows.Count > 0)
                                childBulkCopy.WriteToServer(childTable);
                            tran.Commit();
                        }
                    }
                }
            }
            return true;
        }
    }
}
