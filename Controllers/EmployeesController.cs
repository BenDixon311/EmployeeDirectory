
using EmpDirectory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using System.Configuration;
using EmployeeDB;

namespace EmpDirectory.Controllers
{
    public class EmployeesController : Controller
    {
        static string _connectionString = null;

        static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                    _connectionString = ConfigurationManager.ConnectionStrings["TestDBConnectionStrings"].ConnectionString;

                return _connectionString;
            }
        }
        // GET: Employees
        public ActionResult Index()
        {
            List<EmployeeModel> empList = new List<EmployeeModel>();

            try
            {
                using (EmployeeDirectoryDB db = new EmployeeDirectoryDB(ConnectionString))
                {
                    foreach (Employee dbEmp in db.ReadAllEmployees())
                    {
                        EmployeeModel model = new EmployeeModel()
                        {
                            Id = dbEmp.Id.ToString(),
                            FirstName = dbEmp.FirstName,
                            LastName = dbEmp.LastName,
                            EmployeeID = dbEmp.EmployeeID,
                            Gender = dbEmp.Gender.ToString(),
                            Race = dbEmp.Race.ToString(),
                            BirthDate = dbEmp.BirthDate,
                            Department = dbEmp.Department.ToString()
                        };

                        empList.Add(model);
                    }
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal server error.");
            }
            return View(empList);
        }

        // GET: Employees/Details/5
        public ActionResult Details(int id)
        {
            EmployeeModel model = null;
            try
            {
                using (EmployeeDirectoryDB db = new EmployeeDirectoryDB(ConnectionString))
                {
                    Employee dbEmp = db.ReadEmployee(id);
                    if (dbEmp == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound, string.Format("Student with id = {0}, unknown.", id));

                        model = new EmployeeModel()
                        {
                            Id = dbEmp.Id.ToString(),
                            FirstName = dbEmp.FirstName,
                            LastName = dbEmp.LastName,
                            EmployeeID = dbEmp.EmployeeID,
                            BirthDate = dbEmp.BirthDate,
                            Department = dbEmp.Department.ToString(),
                            Gender = dbEmp.Gender.ToString(),
                            Race = dbEmp.Race.ToString(),
                            RaceOptions = GetRaceOptions()
                        };
                        foreach (SelectListItem item in model.RaceOptions)
                        {
                            if (item.Value == model.Race)
                                item.Selected = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error");
            }
            return View(model);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            EmployeeModel model = new EmployeeModel();
            model.RaceOptions = GetRaceOptions();

            return View(model);
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeModel newEmp)
        {
            try
            {
            
                using (EmployeeDirectoryDB db = new EmployeeDirectoryDB(ConnectionString))
                {
                    Employee dbEmp = new Employee();

                   
                    dbEmp.FirstName = newEmp.FirstName;
                    if (string.IsNullOrEmpty(dbEmp.FirstName))
                        ModelState.AddModelError("FirstName", "First name is required.");

                    dbEmp.LastName = newEmp.LastName;
                    if (string.IsNullOrEmpty(dbEmp.LastName))
                        ModelState.AddModelError("LastName", "Last name is required.");


                    dbEmp.EmployeeID = newEmp.EmployeeID;
                    if (string.IsNullOrEmpty(dbEmp.EmployeeID))
                        ModelState.AddModelError("EmployeeID", "Employee ID is required.");
                    int x;
                    if (!int.TryParse(dbEmp.EmployeeID, out x))
                        ModelState.AddModelError("EmployeeID", "Employee ID must only contain numbers");
                    if (dbEmp.EmployeeID.Length != 8)
                        ModelState.AddModelError("EmployeeID", "Employee ID must be 8 digts.");


                    if (string.Compare(newEmp.Gender, "Male", true) == 0)
                        dbEmp.Gender = Gender.Male;
                    else if (string.Compare(newEmp.Gender, "Female", true) == 0)
                        dbEmp.Gender = Gender.Female;
                    else
                        ModelState.AddModelError("Gender", "Gender must be either male or female");

                    dbEmp.Race = newEmp.Race;
                      
                    dbEmp.BirthDate = newEmp.BirthDate;


                    switch (newEmp.Race.ToLower())
                    {
                        case "hr":
                        case "it":
                        case "management":
                        case "sales":
                        case "customer service":
                            dbEmp.Department = newEmp.Department;
                            break;

                        default:
                            ModelState.AddModelError("Department", "Invalid entry. Options are: HR, IT, Management, Sales, and Customer Service");
                            break;
                    }

                    dbEmp.FullTime = newEmp.FullTime;

                    if (!ModelState.IsValid)
                    {
                        newEmp.RaceOptions = GetRaceOptions();
                        return View(newEmp);
                    }

                    db.CreateEmployee(dbEmp);
                }

               


                return RedirectToAction("Index");
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal server error.");
            }
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int id)
        {
            EmployeeModel model = null;
            try
            {
                using (EmployeeDirectoryDB db = new EmployeeDirectoryDB(ConnectionString))
                {
                    Employee dbEmp = db.ReadEmployee(id);
                    if(dbEmp == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound, string.Format("Employee with ID {0} unknown", id));
                    }

                    model = new EmployeeModel()
                    {
                        Id = dbEmp.Id.ToString(),
                        FirstName = dbEmp.FirstName,
                        LastName = dbEmp.LastName,
                        EmployeeID = dbEmp.EmployeeID,
                        Gender = dbEmp.Gender.ToString(),
                        Race = dbEmp.Race,
                        Department = dbEmp.Department,
                        BirthDate = dbEmp.BirthDate,
                        RaceOptions = GetRaceOptions()

                    };
                foreach (SelectListItem item in model.RaceOptions)
                    {
                        if (item.Value == model.Race)
                            item.Selected = true;
                    }
                }
            }
            catch(Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal server error.");
            }
            return View(model);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EmployeeModel newEmp)
        {
            try
            {
                
                using (EmployeeDirectoryDB db = new EmployeeDirectoryDB(ConnectionString))
                {
                    Employee dbEmp = new Employee();

                    dbEmp.FirstName = newEmp.FirstName;
                    if (string.IsNullOrEmpty(dbEmp.FirstName))
                        ModelState.AddModelError("FirstName", "First name is required.");

                    dbEmp.LastName = newEmp.LastName;
                    if (string.IsNullOrEmpty(dbEmp.LastName))
                        ModelState.AddModelError("LastName", "Last name is required.");


                    dbEmp.EmployeeID = newEmp.EmployeeID;
                    if (string.IsNullOrEmpty(dbEmp.EmployeeID))
                        ModelState.AddModelError("EmployeeID", "Employee ID is required.");
                    int x;
                    if (!int.TryParse(dbEmp.EmployeeID, out x))
                        ModelState.AddModelError("EmployeeID", "Employee ID must only contain numbers");
                    if (dbEmp.EmployeeID.Length != 8)
                        ModelState.AddModelError("EmployeeID", "Employee ID must be 8 digts.");


                    if (string.Compare(newEmp.Gender, "Male", true) == 0)
                        dbEmp.Gender = Gender.Male;
                    else if (string.Compare(newEmp.Gender, "Female", true) == 0)
                        dbEmp.Gender = Gender.Female;
                    else
                        ModelState.AddModelError("Gender", "Gender must be either male or female");

                    dbEmp.Race = newEmp.Race;

                    dbEmp.BirthDate = newEmp.BirthDate;


                    switch (newEmp.Race.ToLower())
                    {
                        case "hr":
                        case "it":
                        case "management":
                        case "sales":
                        case "customer service":
                            dbEmp.Department = newEmp.Department;
                            break;

                        default:
                            ModelState.AddModelError("Department", "Invalid entry. Options are: HR, IT, Management, Sales, and Customer Service");
                            break;
                    }

                    dbEmp.FullTime = newEmp.FullTime;

                    if (!ModelState.IsValid)
                    {
                        newEmp.RaceOptions = GetRaceOptions();
                        return View(newEmp);
                    }


                    db.UpdateEmployee(dbEmp);

                }

                return RedirectToAction("Index");
            }
            catch (KeyNotFoundException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, string.Format("Student with id = {0}, unknown.", id));
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal server error.");
            }
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int id)
        {
            EmployeeModel model = null;
            try
            {
                using (EmployeeDirectoryDB db = new EmployeeDirectoryDB(ConnectionString))
                {
                    Employee dbEmp = db.ReadEmployee(id);
                    if (dbEmp == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound, string.Format("Student with id = {0}, unknown.", id));

                        model = new EmployeeModel()
                        {
                            Id = dbEmp.Id.ToString(),
                            FirstName = dbEmp.FirstName,
                            LastName = dbEmp.LastName,
                            EmployeeID = dbEmp.EmployeeID,
                            BirthDate = dbEmp.BirthDate,
                            Department = dbEmp.Department.ToString(),
                            Gender = dbEmp.Gender.ToString(),
                            Race = dbEmp.Race.ToString()
                        };
                    }
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error");
            }
            return View(model);
        }

        // POST: Employees/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, EmployeeModel newEmp)
        {
            try
            {
                using (EmployeeDirectoryDB db = new EmployeeDirectoryDB(ConnectionString))
                {
                    db.DeleteEmployee(id);
                }

                    return RedirectToAction("Index");
            }
            catch (KeyNotFoundException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, string.Format("Student with id = {0}, unknown.", id));
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal server error.");
            }
        }

        static List<SelectListItem> _raceOptions = null;

        static List<SelectListItem> GetRaceOptions()
        {
            if (_raceOptions == null)
            {
                _raceOptions = new List<SelectListItem>();
                _raceOptions.Add(new SelectListItem() { Value = "White", Text = "Caucasian" });
                _raceOptions.Add(new SelectListItem() { Value = "AfricanAmerican", Text = "African American" });
                _raceOptions.Add(new SelectListItem() { Value = "Hispanic", Text = "Hispanic" });
                _raceOptions.Add(new SelectListItem() { Value = "Asian", Text = "Asian" });
                _raceOptions.Add(new SelectListItem() { Value = "Other", Text = "Other" });
            }
                foreach(SelectListItem item in _raceOptions)
                {
                    item.Selected = false;
                }

            return _raceOptions;
                
        }
    }
}
