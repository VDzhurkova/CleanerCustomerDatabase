using Cleaners.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cleaners.Controllers
{
    public class HomeController : Controller
    {
        private CleanersCustomersEntities db = new CleanersCustomersEntities();

        public ActionResult Index() {
            return View();
        }

        public ActionResult Cleaner(string CleanerName, string searchString)
        {
            //for the dropdown list
            //var CleanersList = new List<string>();
            //var CleanersQuery = from c in db.Cleaners
            //                    orderby c.Name
            //                    select c.Name;
            //CleanersList.AddRange(CleanersQuery.Distinct());
            //ViewBag.CleanerName = new SelectList(CleanersList);

            var cleaners = from c in db.Cleaners select c;
            if (!String.IsNullOrEmpty(CleanerName)) {
                cleaners = cleaners.Where(x=>x.Name == CleanerName);
            }

            if (!String.IsNullOrEmpty(searchString)) {

                cleaners = cleaners.Where(x=>x.Name.Contains(searchString));
            }
            return View(cleaners);
        }

        

        public ActionResult Add() {
            return View();
        }

        [HttpPost]
        public ActionResult Add(Cleaner cleaner) {
            if (ModelState.IsValid) {
                db.Cleaners.Add(cleaner);
                db.SaveChanges();
                return RedirectToAction("Cleaner");
                    }
            return View(cleaner);
        }

        public ActionResult Details(int? id) {
            Cleaner cleaner = db.Cleaners.Find(id);
            return View(cleaner);
        }

        public ActionResult Edit(int? id)
        {
            Cleaner cleaner = db.Cleaners.Find(id);
            return View(cleaner);
        }

        [HttpPost]
        public ActionResult Edit(Cleaner cleaner)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cleaner).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Cleaner");
            }
            return View(cleaner);
        }

        public ActionResult Delete(int? id)
        {
            Cleaner cleaner = db.Cleaners.Find(id);
            return View(cleaner);
        }

        [HttpPost ActionName("Delete") ]
        public ActionResult DeleteConfirmed(int? id)
        {
                Cleaner cleaner = db.Cleaners.Find(id);
                db.Cleaners.Remove(cleaner);
                
            try
            {
                db.SaveChanges();
                return RedirectToAction("Cleaner");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "This Cleaner can't be removed from the datatable. It is still assigned to some existing Customers.";
                return RedirectToAction("Delete", cleaner.Id);
            }
          
        }
        [HttpGet]
        public ActionResult CleanerSchedule(int? id) {
            List<string> cleaningDays = new List<string>();
            var schedule = from c in db.Customers
                           where c.CleanerId == id
                           select c.CleaningDay;
            cleaningDays.AddRange(schedule.Distinct());
            
            Cleaner cleaner = db.Cleaners.Single(c => c.Id == id);
            var tuple = new Tuple<Cleaner, List<int>>(cleaner,convertDayToNumber(cleaningDays));
            return View(tuple);
        }

        public PartialViewResult DisplayCustomer(int? id, string day) {
            var customers = from c in db.Customers orderby c.StartingHour where c.CleanerId == id && c.CleaningDay == day select c;
            return PartialView(customers);
        }

        [HttpPost ActionName("Calculate")]
        public ActionResult CleanerSchedule(int[] customerChecked)
        {
            var customers = new List<Customer>();
            int cleanerId =0;
            foreach (var item in customerChecked) {
                Customer customer = db.Customers.Single(c=>c.Id == item);
                cleanerId = customer.CleanerId;
                customers.Add(customer);
            }
            Cleaner cleaner = db.Cleaners.Single(c=>c.Id == cleanerId);
            var tuple = new Tuple<Cleaner,List<Customer>>(cleaner,customers);
            return View("Calculate",tuple);
        }

      
       public ActionResult Calculate()
       {
           return View();

       }

        public ActionResult Customer(string CustomerName, string searchString)
        {
            var customers = from c in db.Customers select c;
            if (!String.IsNullOrEmpty(CustomerName))
            {
                customers = customers.Where(x => x.Name == CustomerName);
            }

            if (!String.IsNullOrEmpty(searchString))
            {

                customers = customers.Where(x => x.Name.Contains(searchString));
            }
            return View(customers);
        }

        public ActionResult AddCustomer()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddCustomer(Customer customer)
        {
            try {
                Cleaner cleaner = db.Cleaners.Single(c => c.Name == customer.CleanerName);
                customer.CleanerId = cleaner.Id;
            }
            catch (Exception e) {
                TempData["ErrorMessage"] = "The Cleaner do not exists.";
                return RedirectToAction("AddCustomer");
            }
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Customer");
            }
            return View(customer);
        }

        public ActionResult DetailsCustomer(int? id)
        {
            Customer customer = db.Customers.Find(id);
            Cleaner cleaner = db.Cleaners.Single(c => c.Id == customer.CleanerId);
            customer.CleanerName = cleaner.Name;
            return View(customer);
        }

        public ActionResult EditCustomer(int? id)
        {
            Customer customer = db.Customers.Find(id);
            
                Cleaner cleaner = db.Cleaners.Single(c => c.Id == customer.CleanerId);
                customer.CleanerName = cleaner.Name;
            
           
            return View(customer);
        }

        [HttpPost]
        public ActionResult EditCustomer(Customer customer)
        {
            Cleaner cleaner = db.Cleaners.Single(x => x.Name == customer.CleanerName);
            customer.CleanerId = cleaner.Id;

            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Customer");
            }
            return View(customer);
        }

        public ActionResult DeleteCustomer(int? id)
        {
            Customer customer = db.Customers.Find(id);
            return View(customer);
        }

        [HttpPost ActionName("DeleteCustomer")]
        public ActionResult DeleteCustomerConfirmed(int? id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Customer");
        }

        public List<int> convertDayToNumber(List<string> days) {
            List<int> dayNumbers = new List<int>();
            foreach (var day in days) {
                switch (day) {
                    case "Monday": dayNumbers.Add(1);break;
                    case "Tuesday": dayNumbers.Add(2); break;
                    case "Wednesday": dayNumbers.Add(3); break;
                    case "Thursday": dayNumbers.Add(4); break;
                    case "Friday": dayNumbers.Add(5); break;
                    case "Saturday": dayNumbers.Add(6); break;
                    case "Sunday": dayNumbers.Add(7); break;
                }
            }
            dayNumbers.Sort();
            return dayNumbers; 
        }

    }
}