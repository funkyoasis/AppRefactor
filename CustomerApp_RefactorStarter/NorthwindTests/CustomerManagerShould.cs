using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using NorthwindBusiness;
using NorthwindData;
using NorthwindData.Services;
using Microsoft.EntityFrameworkCore;
namespace NorthwindTests
{
	public class CustomerManagerShould
	{
		//private CustomerManager sut;

		[Test]
		public void BeAbleTobeContructed()
		{
			//arrange
			var mockCustomerService = new Mock<ICustomerService>();
			//act
			var sut = new CustomerManager(mockCustomerService.Object);
			//Assert
			Assert.That(sut, Is.InstanceOf<CustomerManager>());
		
		}
		[Test]
		public void ReturnTrue_WhenUpdateIsCalled_WithValidId()
		{
			//arrange
			var mockCustomerService = new Mock<ICustomerService>();
			var originalCustomer = new Customer
			{
				CustomerId = "ROCK"
			};
			mockCustomerService.Setup(cs=> cs.GetCustomerByID("ROCK")).Returns(originalCustomer);
			var sut = new CustomerManager(mockCustomerService.Object);

			//act
			var result = sut.Update("ROCK", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
			//assert
			Assert.That(result);

		}
		[Test]
		public void UpdateSelectedCustomer_WhenupdateIsCalled_WithValidId()
		{
			//arrange
			var mockCustomerService = new Mock<ICustomerService>();
			var originalCustomer = new Customer
			{
				CustomerId = "ROCK",
				ContactName = "Rocky Raccoon",
				CompanyName = "Zoo UK",
				City = "Telford"
			};
			mockCustomerService.Setup(cs => cs.GetCustomerByID("ROCK")).Returns(originalCustomer);
			var sut  = new CustomerManager(mockCustomerService.Object);

			//act
			var result = sut.Update("ROCK", "Rocky Raccon", "UK", "Chester", null);
			//assert
			Assert.That(sut.SelectedCustomer.ContactName, Is.EqualTo("Rocky Raccon"));
			Assert.That(sut.SelectedCustomer.CompanyName, Is.EqualTo("Zoo UK"));
			Assert.That(sut.SelectedCustomer.Country, Is.EqualTo("UK"));
			Assert.That(sut.SelectedCustomer.City, Is.EqualTo("Chester"));
		}
		[Test]
		public void ReturnFalse_WhenUpdateIsCalled_WithInvalidId()
		{
			// arrange
			var mockCustomerService = new Mock<ICustomerService>();

			mockCustomerService.Setup(cs => cs.GetCustomerByID("ROCK")).Returns((Customer)null);

			var sut  = new CustomerManager(mockCustomerService.Object);

			// act
			var result = sut.Update("ROCK", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

			// Assert
			Assert.That(result, Is.False);
		}


		[Test]
		public void DoesNotUpdateSelectedCustomer_WhenUpdateIsCalled_WithInvalidId()
		{
			// arrange
			var mockCustomerService = new Mock<ICustomerService>();
			var originalCustomer = new Customer
			{
				CustomerId = "ROCK",
				ContactName = "Rocky Raccoon",
				CompanyName = "Zoo UK",
				City = "Telford"
			};

			mockCustomerService.Setup(cs => cs.GetCustomerByID("ROCK")).Returns((Customer)null);

			var sut  = new CustomerManager(mockCustomerService.Object);
			sut.SelectedCustomer = originalCustomer;
			// act
			var result = sut.Update("ROCK", "Rocky Raccoon", "UK", "Chester", null);

			// Assert
			Assert.That(sut.SelectedCustomer.ContactName, Is.EqualTo("Rocky Raccoon"));
			Assert.That(sut.SelectedCustomer.CompanyName, Is.EqualTo("Zoo UK"));
			Assert.That(sut.SelectedCustomer.Country, Is.EqualTo(null));
			Assert.That(sut.SelectedCustomer.City, Is.EqualTo("Telford"));
		}

		[Test]
		public void DeleteSelectedCustomer_WhenDeleteIscalled()
		{
			//arrange
			var mockCustomerService = new Mock<ICustomerService>();
			var originalCustomer = new Customer
			{
				CustomerId = "ROCK",
				ContactName = "Rocky Raccoon",
				CompanyName = "Zoo UK",
				City = "Telford"
			};
			mockCustomerService.Setup(cs => cs.GetCustomerByID("ROCK")).Returns(originalCustomer);
			var sut  = new CustomerManager(mockCustomerService.Object);
			//act
			var result = sut.Delete("ROCK");
			Assert.That(result, Is.True);

		}

		[Test]
		public void DeletingAUser_RemovesUser_WhenThereIsAnInvalidId()
		{
			var mockCustomerService = new Mock<ICustomerService>();
			mockCustomerService.Setup(cs => cs.GetCustomerByID("ROCK")).Returns((Customer)null);
			var sut  = new CustomerManager(mockCustomerService.Object);
			var result = sut.Delete("ROCK");
			Assert.That(result, Is.False);
		}

		[Test]
		public void ReturnFalse_WhenUpdateIsCalled_AndADatabaseExceptionIsThrown()
		{
			//arrange
			var mockCustomerService = new Mock<ICustomerService>();
			var originalCustomer = new Customer();
			mockCustomerService.Setup(cs => cs.GetCustomerByID(It.IsAny<string>())).Returns(originalCustomer);
			mockCustomerService.Setup(cs => cs.SaveCustomerChanges()).Throws<DbUpdateConcurrencyException>();
			var sut  = new CustomerManager(mockCustomerService.Object);
			//act
			var result = sut.Update("ROCK", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
			Assert.That(result, Is.False);

		}

		[Test]
		public void NotChangeTheSelectedCustomer_WhenUpdateIsCalled_AndADatabaseExceptionIsThrown()
		{
			// arrange
			var mockCustomerService = new Mock<ICustomerService>();
			var originalCustomer = new Customer
			{
				CustomerId = "ROCK",
				ContactName = "Rocky Raccoon",
				CompanyName = "Zoo UK",
				City = "Telford"
			};

			mockCustomerService.Setup(cs => cs.GetCustomerByID("ROCK")).Returns(originalCustomer);
			mockCustomerService.Setup(cs => cs.SaveCustomerChanges()).Throws<DbUpdateConcurrencyException>();

			var sut = new CustomerManager(mockCustomerService.Object);
			sut.SelectedCustomer = new Customer
			{
				CustomerId = "ROCK",
				ContactName = "Rocky Raccoon",
				CompanyName = "Zoo UK",
				City = "Telford"
			};
			// act
			var result = sut.Update("ROCK", "Rocky Raccoon", "UK", "Chester", null);

			// Assert
			Assert.That(sut.SelectedCustomer.ContactName, Is.EqualTo("Rocky Raccoon"));
			Assert.That(sut.SelectedCustomer.CompanyName, Is.EqualTo("Zoo UK"));
			Assert.That(sut.SelectedCustomer.Country, Is.EqualTo(null));
			Assert.That(sut.SelectedCustomer.City, Is.EqualTo("Telford"));
		}

		[Test]
		public void CallSaveCustomerChanges_WhenUpdateIsCalled_WithValidId()
		{
			var mockCustomerService = new Mock<ICustomerService>();
			mockCustomerService.Setup(cs => cs.GetCustomerByID("ROCK")).Returns(new Customer());
			var sut = new CustomerManager(mockCustomerService.Object);
			var result = sut.Update("ROCK", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

			//Assert
			mockCustomerService.Verify(cs => cs.SaveCustomerChanges(), Times.Once);
		}

		[Test]
		public void LetsSeeWhatHappens_WhenUpdateIsCalled_AndAllInvocationsArentSetUp()
		{
			var mockCustomerService = new Mock<ICustomerService>(MockBehavior.Strict);
			mockCustomerService.Setup(cs => cs.GetCustomerByID("ROCK")).Returns(new Customer());
			mockCustomerService.Setup(cs => cs.SaveCustomerChanges());
			var sut = new CustomerManager(mockCustomerService.Object);
			var result = sut.Update("ROCK", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

			//Assert
			Assert.That(result);

		}
	}

}
