using System;
using System.Data;
using System.Data.SqlClient;

namespace Ado.Net
{
    public class Person
    {
        public int PersonID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public string Discriminator { get; set; }
    }

    class Program
    {
        readonly static string connectionString = "data source=localhost; database=School; integrated security=SSPI";

        static void Main(string[] args)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))  //Establish a connection
            {
                try
                {
                    connection.Open(); //Opens connection
                    string queryString = "SELECT * FROM Person WHERE FirstName = 'Kim'";   //query to be fired on database.
                    SqlDataAdapter adapter = new SqlDataAdapter(queryString, connection);

                    DataSet dataSet = new DataSet(); //An in-memory representation of result data.
                    adapter.Fill(dataSet, "Person");

                    foreach(DataRow p in dataSet.Tables["Person"].Rows) //Need to typecast result to .Net objects
                    {
                        var person = new Person();
                        person.PersonID = Convert.ToInt32(p[0]);
                        person.LastName = Convert.ToString(p[1]);
                        person.FirstName = Convert.ToString(p[2]);
                        person.HireDate = p[3] as DateTime?;
                        person.EnrollmentDate = p[4] as DateTime?;
                        person.Discriminator = Convert.ToString(p[5]);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("OOPs, something went wrong." + e);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
