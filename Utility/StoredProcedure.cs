using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// Stored procedure class
    /// </summary>
    public static class StoredProcedure
    {
        /// <summary>
        /// Retrieves the next available number and uses try/catch block to handle any exception.
        /// </summary>
        /// <param name="discriminator"></param>
        /// <returns>The next available number</returns>
        public static long? NextNumber(string discriminator)
        {
            try
            {
                long? returnValue = 0;

                SqlConnection connection = new SqlConnection("Data Source=LASHMAN\\EARTH; " +
                              "Initial Catalog=BITCollege_LSContext;Integrated Security=True");
                SqlCommand storedProcedure = new SqlCommand("next_number", connection);
                storedProcedure.CommandType = CommandType.StoredProcedure;
                storedProcedure.Parameters.AddWithValue("@Discriminator", discriminator);
                SqlParameter outputParameter = new SqlParameter("@NewVal", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Output
                };
                storedProcedure.Parameters.Add(outputParameter);
                connection.Open();
                storedProcedure.ExecuteNonQuery();
                connection.Close();
                returnValue = (long?)outputParameter.Value;
                return returnValue;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
