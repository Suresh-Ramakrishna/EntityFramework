using DatabaseModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace EntityFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            FetchPersonUsingLinq2Entities();
            FetchPersonUsingEntitySql();
            FetchPersonUsingLinqSql();

            DbContextOperations();

            EagerLoading();
            EagerLoadingMultipleLevels();
            ExplicitLoading();
            QueryCountWithoutLoading();

            OperationsOnEntitiesAsync();
            ConcurrenyControl();
        }

        static void FetchPersonUsingLinq2Entities()
        {
            using (var context = new SchoolEntities())
            {
                var person = context.Person.FirstOrDefault<Person>(s => s.PersonID == 1);
            }
        }

        static void FetchPersonUsingEntitySql()
        {
            using (var dbContext = new SchoolEntities())
            {
                string sqlString = $"SELECT VALUE st FROM SchoolEntities.Person AS st WHERE st.PersonID == 1";

                var objctx = (dbContext as IObjectContextAdapter).ObjectContext;
                ObjectQuery<Person> personQuery = objctx.CreateQuery<Person>(sqlString);
                Person person = personQuery.First();
            }
        }

        static void FetchPersonUsingLinqSql()
        {
            using (var ctx = new SchoolEntities())
            {
                var person = ctx.Person.SqlQuery("Select * from Person where PersonID=1").FirstOrDefault();
            }
        }

        static void DbContextOperations()
        {
            using (var ctx = new SchoolEntities())
            {
                var person = ctx.Person.FirstOrDefault<Person>(s => s.PersonID == 1);

                DbEntityEntry dbEntityEntry = ctx.Entry(person);
                EntityState state = dbEntityEntry.State; //gets the state of the entity.
                DbPropertyValues currentValues = dbEntityEntry.CurrentValues; //Gets current values of the entity.
                DbPropertyValues values = dbEntityEntry.GetDatabaseValues(); //Gets values from database

                foreach (var property in dbEntityEntry.CurrentValues.PropertyNames) //All property names in the entity
                    Console.WriteLine($"{property}: {dbEntityEntry.CurrentValues[property]}"); //Get value of the property for the given entity

                dbEntityEntry.Reload(); //reloads data for the entity from db. Replaces any modified value in the entity and changes state to Unchanged.

                ctx.SaveChanges(); //saves changes to db
                DbSet course = ctx.Set(typeof(Course)); //returns dbset instance of type Course.Same as the one that we access using ctx.Course.

                DbChangeTracker tracker = ctx.ChangeTracker; //Provides details on changes made to entities
                IEnumerable<DbEntityEntry> entries = tracker.Entries(); //Gets DbEntityEntry objects for all the entities tracked by this context. Tracked object can also be in unchanged state.
                tracker.DetectChanges(); //Checks for any changes and updates the state appropriately, No need to call this explicitly.
                bool hasChanges = tracker.HasChanges(); //Checks if there any any modified entities in the context.

                Database database = ctx.Database; //Creates a Database instance that allows for creation/deletion/modification of the underlying database.
                DbContextConfiguration config = ctx.Configuration; //Provides access to configuration options for the context.
            }
        }

        static void EagerLoading()
        {
            using (var ctx = new SchoolEntities())
            {
                var p = ctx.Person.Include("Course").Where(s => s.PersonID == 1).FirstOrDefault(); //Eagerly loading loads related entities even if lazy loading is disabled. 
            }
        }
        static void EagerLoadingMultipleLevels()
        {
            using (var ctx = new SchoolEntities())
            {
                var p = ctx.Person.Include("Course.StudentGrade").ToList(); //Loads all Person, all courses for each person and Student grade for each course.
            }
        }
        static void ExplicitLoading()
        {
            using (var ctx = new SchoolEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false; //Disable loading of related entities by default.
                var p = ctx.Person.Find(1);
                ctx.Entry(p).Collection(x => x.Course).Load(); //Loads related courses for the person.
            }
        }

        static void QueryCountWithoutLoading()
        {
            using (var ctx = new SchoolEntities())
            {
                ctx.Configuration.LazyLoadingEnabled = false; //Disable loading of related entities by default.
                var p = ctx.Person.Find(1);
                var count = ctx.Entry(p).Collection(x => x.Course).Query().Count(); //returns total count of Course for person
            }
        }
        static async void OperationsOnEntitiesAsync()
        {
            using (var ctx = new SchoolEntities())
            {
                ctx.Person.AddRange(new List<Person> { });
                await ctx.SaveChangesAsync();
                await ctx.Person.FirstOrDefaultAsync();
            }
        }
        static void ConcurrenyControl()
        {
            Person personOne = null;
            Person personTwo = null;

            using (var context = new SchoolEntities())
                personOne = context.Person.Where(s => s.PersonID == 1).Single();

            using (var context = new SchoolEntities())
                personTwo = context.Person.Where(s => s.PersonID == 1).Single();

            personOne.FirstName = "Rama";
            personTwo.FirstName = "Krishna";

            using (var context = new SchoolEntities()) //saves personOne to db
            {
                try
                {
                    context.Entry(personOne).State = EntityState.Modified;
                    context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine("User1: Optimistic Concurrency exception occurred");
                }
            }

            using (var context = new SchoolEntities()) //saves personTwo to db, throws error.
            {
                try
                {
                    context.Entry(personTwo).State = EntityState.Modified;
                    context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var data = ex.Entries.Single();
                    data.OriginalValues.SetValues(data.GetDatabaseValues()); //Updates original values with database values. Now when we do save, edited values will be written to db
                    context.SaveChanges();
                    Console.WriteLine("User2: Optimistic Concurrency exception occurred");
                }
            }
        }
        static void StoredProcedure()
        {
            using(var ctx = new SchoolEntities())
            {
                ctx.DeletePerson(1); //DeletePerson is a procedure
            }
        }
    }
}
