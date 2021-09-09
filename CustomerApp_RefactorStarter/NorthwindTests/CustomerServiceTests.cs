using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using NorthwindData;
using NorthwindData.Services;


namespace NorthwindTests
{
	public class CustomerServiceTests
	{
		private CustomerService _sut;
		private NorthwindContext _context;
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			var options = new DbContextOptionsBuilder<NorthwindContext>()
				.UseInMemoryDatabase(databaseName: "Example_DB").Options;
			_context = new NorthwindContext(options);
			_sut = new CustomerService(_context);

			//seed the database
			_sut.CreateCustomer(new Customer { CustomerId = "Mand", ContactName = "Nish Mandal", CompanyName = "SpartaGlobal", City = "Paris" });
			_sut.CreateCustomer(new Customer { CustomerId = "FREN", ContactName = "Cathy French", CompanyName = "SpartaGlobal", City = "Ottawa" });
			_sut.CreateCustomer(new Customer { CustomerId = "KYOTO", ContactName = "Kyoto Samurai", CompanyName = "Oasis Industries", City = "Kyoto" });
		}

		[Test]
		public void GivenAValidId_CorrectCustomerIsReturned()
		{
			var result = _sut.GetCustomerByID("Mand");
			Assert.That(result, Is.TypeOf<Customer>());
			Assert.That(result.ContactName, Is.EqualTo("Nish Mandal"));
			Assert.That(result.CompanyName, Is.EqualTo("SpartaGlobal"));
			Assert.That(result.City, Is.EqualTo("Paris"));

		}

		[Test]
		public void GivenANewCustomer_CreateCustomeraddsItToTheDatabase()
		{
			//Arrange
			var numberOfCustomersBefore = _context.Customers.Count();
			var newCustomer = new Customer {CustomerId = "BEAR",ContactName ="Martin Beard", CompanyName="SpartaGlobal", City="Rome" };

			//act 
			_sut.CreateCustomer(newCustomer);

			var numberOfCustomerAfter = _context.Customers.Count();
			var result = _sut.GetCustomerByID("BEAR");

			//Assert
			Assert.That(numberOfCustomersBefore + 1, Is.EqualTo(numberOfCustomerAfter));
			Assert.That(result, Is.TypeOf<Customer>());
			Assert.That(result.ContactName, Is.EqualTo("Martin Beard"));
			Assert.That(result.CompanyName, Is.EqualTo("SpartaGlobal"));
			Assert.That(result.City, Is.EqualTo("Rome"));
			
			//Clean up
			_context.Customers.Remove(newCustomer);
			_context.SaveChanges();
		}

		[Test]
		public void GivenIWantToRemoveACustomer_RemoveCustomerRemovesFromTheDataBase()
		{
			//Arrange
			var numberOfcustomersBefore = _context.Customers.Count();
			var selectedCustomer = _sut.GetCustomerByID("KYOTO");
			_context.Customers.Remove(selectedCustomer);
			
			_context.SaveChanges();
			var numberOfCustomersAfter = _context.Customers.Count();
			//Assert
			Assert.That(numberOfcustomersBefore - 1, Is.EqualTo(numberOfCustomersAfter));
		}


	}
}
