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

            return employeeWithUserName != null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            if(crudOperation == "create")
            {
                db.Employees.InsertOnSubmit(employee);
                db.SubmitChanges();
            }
            else if(crudOperation == "delete")
            {
                Employee employeeFromDb = db.Employees.Where(m => m.EmployeeNumber == employee.EmployeeNumber && m.LastName == employee.LastName).Single();
                db.Employees.DeleteOnSubmit(employeeFromDb);
                db.SubmitChanges();
            }
            else if(crudOperation == "read")
            {
                Employee employeeFromDb = db.Employees.Where(m => m.EmployeeNumber == employee.EmployeeNumber).Single();//try FirstorDefault

                Console.WriteLine($"name: {employeeFromDb.FirstName} {employeeFromDb.LastName}" +
                    $"\nemployee number: {employeeFromDb.UserName}" +
                    $"\nemail: {employeeFromDb.Email}");
                Console.ReadLine();
            }
            else if(crudOperation == "update")
            {
                Employee employeeFromDb = db.Employees.Where(m => m.EmployeeNumber == employee.EmployeeNumber && m.LastName == employee.LastName).Single();

                employeeFromDb.FirstName = employee.FirstName;
                employeeFromDb.LastName = employee.LastName;
                employeeFromDb.EmployeeNumber = employee.EmployeeNumber;
                employeeFromDb.Email = employee.Email;

                db.SubmitChanges();

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
            Animal animalFromDb = db.Animals.Where(a => a.AnimalId == id).FirstOrDefault();

            if (animalFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return animalFromDb;
            }
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            Animal animalFromDb = db.Animals.Where(a => a.AnimalId == animalId).Single();



            animalFromDb.CategoryId = Convert.ToInt32(updates[1]);
            animalFromDb.Name = updates[2];
            animalFromDb.Age = Convert.ToInt32(updates[3]);
            animalFromDb.Demeanor = updates[4];
            animalFromDb.KidFriendly = Convert.ToBoolean(updates[5]);
            animalFromDb.PetFriendly = Convert.ToBoolean(updates[6]);
            animalFromDb.Weight = Convert.ToInt32(updates[7]);
            animalFromDb.AnimalId = Convert.ToInt32(updates[8]);



            //Start from the whole db
            //var animals = db.animals
            //foreach (updates[int] in updates)//keyvalue pair in updates
            //{
            //    //switch case
            //    animals = animals.where
            //}

            db.SubmitChanges();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            Animal animalFromDb = db.Animals.Where(a => a.AnimalId == animal.AnimalId).Single();
            db.Animals.DeleteOnSubmit(animalFromDb);
            db.SubmitChanges();
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            Category categoryFromDb = null;
            if (updates.ContainsKey(1))
            {
                categoryFromDb = db.Categories.Where(m => m.Name == updates[1]).Single();
            }
            bool kidFriendly = UserInterface.UserResponseToBool(updates[5]);
            bool petFriendly = UserInterface.UserResponseToBool(updates[6]);

            var animalsFromDb = db.Animals.Where(m =>
              (m.CategoryId == categoryFromDb.CategoryId || categoryFromDb == null) &&
              (m.Name == updates[2] || updates[2] == null) &&
              (m.Age == Convert.ToInt32(updates[3]) || updates[3] == null) &&
              (m.Demeanor == updates[4] || updates[4] == null) &&
              (m.KidFriendly == kidFriendly || updates[5] == null) &&
              (m.PetFriendly == petFriendly || updates[6] == null) &&
              (m.Weight == Convert.ToInt32(updates[7]) || updates[7] == null) &&
              (m.AnimalId == Convert.ToInt32(updates[8]) || updates[8] == null));                       
            
            return animalsFromDb;
        }
         
        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            Category categoryFromDb = db.Categories.Where(c => c.Name == categoryName).FirstOrDefault();

            if (categoryFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return categoryFromDb.CategoryId;
            }
        }
        
        internal static Room GetRoom(int animalId)
        {
            Room roomFromDb = db.Rooms.Where(r => r.AnimalId == animalId).FirstOrDefault();
            
            if (roomFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return roomFromDb;
            }

        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            DietPlan dietPlanFromDb = db.DietPlans.Where(d => d.Name == dietPlanName).FirstOrDefault();

            if (dietPlanFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return dietPlanFromDb.DietPlanId;
            }
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {

            Adoption adoptionToDB = new Adoption { Animal = animal, Client = client};

            if (animal == null || client == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                db.Adoptions.InsertOnSubmit(adoptionToDB);
                db.SubmitChanges();
            }
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {

            IQueryable<Adoption> pendingAdoptionsFromDB = db.Adoptions.Where(a => a.ApprovalStatus == "Pending");
            
            return pendingAdoptionsFromDB;
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            if (isAdopted)
            {
                db.Adoptions.Where(a => a.AnimalId == adoption.AnimalId).Single();
                adoption.ApprovalStatus = "Adopted";
                db.Adoptions.InsertOnSubmit(adoption);
                db.SubmitChanges();
            }
            else
            {
                db.Adoptions.Where(a => a.AnimalId == adoption.AnimalId).Single();
                adoption.ApprovalStatus = "Available";
                db.Adoptions.InsertOnSubmit(adoption);
                db.SubmitChanges();
            }
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            Adoption adoptionFromDb = db.Adoptions.Where(a => a.AnimalId == animalId && a.ClientId == clientId).Single();
            

            if (adoptionFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                db.Adoptions.DeleteOnSubmit(adoptionFromDb);
                db.SubmitChanges();
            }
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