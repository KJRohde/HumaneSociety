using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        // switch case to decide which operation to do
        // create: userEmployee.CreateNewEmployee
        // update: userEmployee.UpdateEmployeeInfo
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            Console.WriteLine("Would you like to:\n1. Create a new employee\n2. Read employee info\n3. Update employee info\n4. Remove and employee from the database");
            switch (crudOperation)
            {
                case "create":
                    db.Employees.InsertOnSubmit(employee);
                    db.SubmitChanges();
                    break;
                case "read":
                    var employeeInfo = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).Select(e => e);
                    Console.WriteLine(employeeInfo);
                    break;
                case "update":
                    Employee searchEmployee = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).Single();
                    Console.WriteLine("What information would you like to change?\n1. First Name\n2. Last Name\n3. Email");
                    switch(Console.ReadLine())
                    {
                        case "1":
                            Console.WriteLine("What is this employees first name?");
                            string givenFirstName = Console.ReadLine();
                            searchEmployee.FirstName = givenFirstName;
                            db.SubmitChanges();
                            break;
                        case "2":
                            Console.WriteLine("What is this employees last name?");
                            string givenLastName = Console.ReadLine();
                            searchEmployee.LastName = givenLastName;
                            db.SubmitChanges();
                            break;
                        case "3":
                            Console.WriteLine("What is this employees email address?");
                            string givenEmail = Console.ReadLine();
                            searchEmployee.Email = givenEmail;
                            db.SubmitChanges();
                            break;
                    }
                    break;
                case "delete":
                    db.Employees.DeleteOnSubmit(employee);
                    db.SubmitChanges();
                    break;
                default:
                    Console.WriteLine("Please choose a valid option.");
                    RunEmployeeQueries(employee, crudOperation);
                    break;
            }
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id)
        {
            Animal searchAnimal = db.Animals.Where(a => a.AnimalId == id).Single();
            return searchAnimal;          
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            Animal searchAnimal = db.Animals.Where(a => a.AnimalId == animalId).Single();
            Console.WriteLine("What would you like to update?\n1. Name\n2. Weight\n3. Age\n4. Demeanor\n5. Kid Friendly\n6. Pet Friendly\n7. Gender\n8. AdoptionStatus\n9. CategoryID\n10. Diet Plan ID\n11. EmployeeID");
            switch(Console.ReadLine())
            {
                case "1":
                    Console.WriteLine("What is this animals name?");
                    string givenName = Console.ReadLine();
                    searchAnimal.Name = givenName;
                    db.SubmitChanges();
                    break;
                case "2":
                    Console.WriteLine("What is this animals weight?");
                    int givenWeight = int.Parse(Console.ReadLine());
                    searchAnimal.Weight = givenWeight;
                    db.SubmitChanges();
                    break;
                case "3":
                    Console.WriteLine("What is this animals age?");
                    int givenAge = int.Parse(Console.ReadLine());
                    searchAnimal.Age = givenAge;
                    db.SubmitChanges();
                    break;
                case "4":
                    Console.WriteLine("What is this animals demeanor?");
                    string givenDemeanor = Console.ReadLine();
                    searchAnimal.Demeanor = givenDemeanor;
                    db.SubmitChanges();
                    break;
                case "5":
                    if (searchAnimal.KidFriendly = true)
                    {
                        Console.WriteLine("This animal was listed as kid friendly and is now listead as NOT kid friendly.");
                    }
                    else
                    {
                        Console.WriteLine("This animal was listed as NOT kid friendly and is now kid friendly.");
                    }
                    searchAnimal.KidFriendly = !searchAnimal.KidFriendly;
                    db.SubmitChanges();
                    break;
                case "6":
                    if (searchAnimal.PetFriendly = true)
                    {
                        Console.WriteLine("This animal was listed as pet friendly and is now listead as NOT pet friendly.");
                    }
                    else
                    {
                        Console.WriteLine("This animal was listed as NOT pet friendly and is now pet friendly.");
                    }
                    searchAnimal.PetFriendly = !searchAnimal.PetFriendly;
                    db.SubmitChanges();
                    break;
                case "7":
                    Console.WriteLine("What is this animals gender?");
                    string givenGender = Console.ReadLine();
                    searchAnimal.Gender = givenGender;
                    db.SubmitChanges();
                    break;
                case "8":
                    Console.WriteLine("What is this animals adoption status?");
                    string givenStatus = Console.ReadLine();
                    searchAnimal.AdoptionStatus = givenStatus;
                    db.SubmitChanges();
                    break;
                case "9":
                    Console.WriteLine("What is this animals category ID?");
                    int givenCategoryId = int.Parse(Console.ReadLine());
                    searchAnimal.CategoryId = givenCategoryId;
                    db.SubmitChanges();
                    break;
                case "10":
                    Console.WriteLine("What is this animals Diet Plan ID?");
                    int givenDietId = int.Parse(Console.ReadLine());
                    searchAnimal.DietPlanId = givenDietId;
                    db.SubmitChanges();
                    break;
                case "11":
                    Console.WriteLine("What is this animals primary care employee ID?");
                    int givenEmployeeId = int.Parse(Console.ReadLine());
                    searchAnimal.EmployeeId = givenEmployeeId;
                    db.SubmitChanges();
                    break;
                default:
                    Console.WriteLine("Please enter a valid option.");
                    UpdateAnimal(animalId, updates);
                    break;
            }
        }

        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(animal);
            db.SubmitChanges();
        }
        
        // TODO: Animal Multi-Trait Search
        //Completed. All meta data the instructors helped with deleted due to having to reclone repo 
        
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            IQueryable<Animal> queryAnimals = db.Animals;

            foreach (KeyValuePair<int, string> update in updates)
            {
                switch (update.Key)
                {
                    case 1:
                        queryAnimals = queryAnimals.Where(q => q.CategoryId == int.Parse(update.Value));
                        break;
                    case 2:
                        queryAnimals = queryAnimals.Where(q => q.Name == update.Value);
                        break;
                    case 3:
                        queryAnimals = queryAnimals.Where(q => q.Age == int.Parse(update.Value));
                        break;
                    case 4:
                        queryAnimals = queryAnimals.Where(q => q.Demeanor == update.Value);
                        break;
                    case 5:
                        queryAnimals = queryAnimals.Where(q => q.KidFriendly == bool.Parse(update.Value));
                        break;
                    case 6:
                        queryAnimals = queryAnimals.Where(q => q.PetFriendly == bool.Parse(update.Value));
                        break;
                    case 7:
                        queryAnimals = queryAnimals.Where(q => q.Weight == int.Parse(update.Value));
                        break;
                }
            }
            return queryAnimals;
           
        }

        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            Category searchCategory = db.Categories.Where(c => c.Name == categoryName).Single();

        }
        
        internal static Room GetRoom(int animalId)
        {
            throw new NotImplementedException();
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            throw new NotImplementedException();
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            throw new NotImplementedException();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            throw new NotImplementedException();
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            throw new NotImplementedException();
        }

        // TODO: Shots Stuff
        //Iqueryable named giveAnimalShots
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            IQueryable<AnimalShot> giveAnimalShot = db.AnimalShots.Where(a => a.AnimalId == animal.AnimalId);
            return giveAnimalShot;
        }


        //DB Info--> Shots (ShotId INTEGER IDENTITY (1,1) PRIMARY KEY, Name VARCHAR(50));
        //// Add the new object to the Orders collection --->  db.Orders.InsertOnSubmit(ord);
        //The added entity will not appear in query results until after "SubmitChanges" has been called.
        internal static void UpdateShot(string shotName, Animal animal)
        {
            AnimalShot newShot = new AnimalShot();
            newShot.AnimalId = animal.AnimalId;

            var shotGiven = db.Shots.Where(s => s.Name == shotName).Single();


            newShot.ShotId = shotGiven.ShotId;
            newShot.DateReceived = DateTime.Today;
            db.AnimalShots.Where(c => c.AnimalId == shotGiven.ShotId);


            db.AnimalShots.InsertOnSubmit(newShot);
            db.SubmitChanges();
        }

    }
    }
}