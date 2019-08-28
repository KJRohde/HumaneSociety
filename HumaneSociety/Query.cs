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
            switch (crudOperation)
            {
                case "create":
                    db.Employees.InsertOnSubmit(employee);
                    db.SubmitChanges();
                    break;
                case "read":
                    //Write a line similar to DisplayAnimalInfo
                    UserInterface.DisplayEmployeeInfo(employee);
                    break;
                case "update":
                    Console.WriteLine("What information would you like to change?\n1. First Name\n2. Last Name\n3. Email");
                    string input = Console.ReadLine();
                    GetUpdateChanges(employee, input);
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
        internal static void GetUpdateChanges(Employee employee, string input)
        {
            switch (input)
            {
                case "1":
                    Console.WriteLine("What is this employees first name?");
                    string givenFirstName = Console.ReadLine();
                    employee.FirstName = givenFirstName;
                    db.SubmitChanges();
                    break;
                case "2":
                    Console.WriteLine("What is this employees last name?");
                    string givenLastName = Console.ReadLine();
                    employee.LastName = givenLastName;
                    db.SubmitChanges();
                    break;
                case "3":
                    Console.WriteLine("What is this employees email address?");
                    string givenEmail = Console.ReadLine();
                    employee.Email = givenEmail;
                    db.SubmitChanges();
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
            foreach(KeyValuePair<int, string> el in updates)
            {
                GetAnimalUpdates(searchAnimal, updates);
            }
        }
        internal static void GetAnimalUpdates(Animal animal, Dictionary<int, string> updates)
        {
            foreach (KeyValuePair<int, string> el in updates)
                switch (el.Key)
            {
                    case 1:
                        animal.CategoryId = int.Parse(el.Value);
                        break;
                    case 2:
                        animal.Name = el.Value;
                        break;
                    case 3:
                        animal.Age = int.Parse(el.Value);
                        break;
                    case 4:
                        animal.Demeanor = el.Value;
                        break;
                    case 5:
                        if (el.Value == "yes" || el.Value == "y")
                        {
                            animal.KidFriendly = true;
                        }
                        else
                        {
                            animal.KidFriendly = false;
                        }
                        break;
                    case 6:
                        if (el.Value == "yes" ||  el.Value == "y")
                        {
                            animal.PetFriendly = true;
                        }
                        else
                        {
                            animal.PetFriendly = false;
                        }
                        break;
                    case 7:
                        animal.Weight = int.Parse(el.Value);
                        break;
                    case 8:
                        animal.AnimalId = int.Parse(el.Value);
                        break;

                }
        }
        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(animal);
            db.SubmitChanges();
        }
        
        // TODO: Animal Multi-Trait Search
        //Com
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            IQueryable<Animal> queryAnimals = db.Animals;
            foreach (KeyValuePair<int, string> update in updates)
            {
                switch (update.Key)
                {
                    case 1:
                        queryAnimals = queryAnimals.Where(a => a.CategoryId == int.Parse(update.Value));
                        break;
                    case 2:
                        queryAnimals = queryAnimals.Where(a => a.Name == update.Value);
                        break;
                    case 3:
                        queryAnimals = queryAnimals.Where(a => a.Age == int.Parse(update.Value));
                        break;
                    case 4:
                        queryAnimals = queryAnimals.Where(a => a.Demeanor == update.Value);
                        break;
                    case 5:
                        queryAnimals = queryAnimals.Where(a => a.KidFriendly == bool.Parse(update.Value));
                        break;
                    case 6:
                        queryAnimals = queryAnimals.Where(a => a.PetFriendly == bool.Parse(update.Value));
                        break;
                    case 7:
                        queryAnimals = queryAnimals.Where(a => a.Weight == int.Parse(update.Value));
                        break;
                }
            }
            return queryAnimals;
        }

        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
            //Make switch case of animal species with number values as returns
        {
            switch (categoryName)
            {
                case "beagle":
                    return 1;
                case "weenie":
                    return 2;
                case "poodle":
                    return 3;
                case "dalmation":
                    return 4;
                default:
                    Console.WriteLine("That is not a valid category");
                    return 0;
            }
        }
        internal static Room GetRoom(int animalId)
        {
            Room searchRoom = db.Rooms.Where(r => r.AnimalId == animalId).Single();
            return searchRoom;
        }
        
        internal static int GetDietPlanId(string dietPlanName)
                //make switch case of possible food plans with number values as returns
        {
                switch (dietPlanName)
                {
                    case "big dog":
                        return 1;
                    case "small dog":
                        return 2;
                    case "medium dog":
                        return 3;
                    default:
                        Console.WriteLine("That is not a valid food plan");
                        return 0;
                }
            }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            Adoption adoption = new Adoption();
            adoption.AnimalId = animal.AnimalId;
            adoption.ClientId = client.ClientId;
            adoption.ApprovalStatus = "pending";
            adoption.AdoptionFee = 75;
            adoption.PaymentCollected = null;
                db.Adoptions.InsertOnSubmit(adoption);

        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            IQueryable<Adoption> pendingAdoptions = db.Adoptions.Where(a => a.ApprovalStatus == "pending").OrderBy(a => a.ClientId);
            return pendingAdoptions;
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            if (isAdopted == true)
                adoption.ApprovalStatus = "approved";
            else
                adoption.ApprovalStatus = "denied";
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            Adoption searchAdoption = db.Adoptions.Where(a => a.AnimalId == animalId && a.ClientId == clientId).Single();
            db.Adoptions.DeleteOnSubmit(searchAdoption);
            db.SubmitChanges();

        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            throw new NotImplementedException();
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            throw new NotImplementedException();
        }
    }
}