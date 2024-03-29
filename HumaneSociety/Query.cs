﻿using System;
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
                    var employeeToRead = db.Employees.FirstOrDefault(e => e.EmployeeNumber == employee.EmployeeNumber);
                    List<string> info = new List<string>() { "Employee Number: " + employeeToRead.EmployeeNumber, "Name: " + employeeToRead.FirstName + " " + employeeToRead.LastName, "Email: " + employeeToRead.Email };
                    UserInterface.DisplayUserOptions(info);
                    Console.ReadLine();
                    break;
                case "update":
                    var employeeToUpdate = db.Employees.FirstOrDefault(e => e.EmployeeNumber == employee.EmployeeNumber);
                    employeeToUpdate.FirstName = employee.FirstName;
                    employeeToUpdate.LastName = employee.LastName;
                    employeeToUpdate.Email = employee.Email;
                    db.SubmitChanges();
                    break;
                case "delete":
                    var employeeToDelete = db.Employees.FirstOrDefault(e => e.EmployeeNumber == employee.EmployeeNumber && e.LastName == employee.LastName);
                    db.Employees.DeleteOnSubmit(employeeToDelete);
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
                        animal.CategoryId = GetCategoryId(el.Value);
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
            db.SubmitChanges();
        }
        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(animal);
            db.SubmitChanges();
        }
        //Completed. All meta data the instructors helped with deleted due to having to reclone repo 
        
        // TODO: Animal Multi-Trait Search
        
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            IQueryable<Animal> queryAnimals = db.Animals;

            foreach (KeyValuePair<int, string> update in updates)
            {
                switch (update.Key)
                {
                    case 1:
                        queryAnimals = queryAnimals.Where(q => q.CategoryId == GetCategoryId(update.Value));
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
            var categoryId = db.Categories.Where(c => c.Name == categoryName).Select(c => c.CategoryId).Single();
            return categoryId;
        }
        internal static Room GetRoom(int animalId)
        {
            Room searchRoom = db.Rooms.Where(r => r.AnimalId == animalId).FirstOrDefault();
            return searchRoom;
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            var dietPlanId = db.DietPlans.Where(d => d.Name == dietPlanName).Select(c => c.DietPlanId).Single();
            return dietPlanId;
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            Adoption adoption = new Adoption();
            adoption.AnimalId = animal.AnimalId;
            adoption.ClientId = client.ClientId;
            adoption.ApprovalStatus = "pending";
            adoption.AdoptionFee = 75;
            adoption.PaymentCollected = false;
            animal.AdoptionStatus = "pending";
                db.Adoptions.InsertOnSubmit(adoption);
            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            IQueryable<Adoption> pendingAdoptions = db.Adoptions.Where(a => a.ApprovalStatus == "pending").OrderBy(a => a.ClientId);
            return pendingAdoptions;
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            if (isAdopted == true)
            {
                adoption.ApprovalStatus = "approved";
            }
            else
            {
                adoption.ApprovalStatus = "denied";
            }
            db.SubmitChanges();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            Adoption searchAdoption = db.Adoptions.Where(a => a.AnimalId == animalId && a.ClientId == clientId).Single();
            db.Adoptions.DeleteOnSubmit(searchAdoption);
            db.SubmitChanges();

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
            newShot.DateReceived = DateTime.Now;
            db.AnimalShots.Where(c => c.AnimalId == shotGiven.ShotId);


            db.AnimalShots.InsertOnSubmit(newShot);
            db.SubmitChanges();
        }

    }
 }