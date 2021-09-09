using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindData.Services
{
	public interface ICustomerService
	{
		List<Customer> GetCustomers();
		public Customer GetCustomerById();
		public void CreateCustomer(Customer c);
		public void SaveCustomerChanges();
		public void RemoveCustomer(Customer c);


	}
}
