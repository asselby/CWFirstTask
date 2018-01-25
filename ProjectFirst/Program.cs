using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFirst
{
    public class Program
    {
        public static decimal Sort(decimal num)
        {
            if (num > 5000)
                return 0;
            else if (num < 1000 && num > 4999)
                return 1;
            else return 2;
        }
        static void Main(string[] args)
        {

            /*Сгруппировать всех клиентов по странам и вывести список этих стран,
             * отсортированных по количеству клиентов в стране **/
            Entities model = new Entities();
            DateTime dateNow = DateTime.Now;
            var customers = from p in model.Customers
                            orderby p.Country
                            select new
                            {
                                p.CustomerID,
                                p.Country
                            };


            /*Сгруппировать все заказы по сотрудникам и вывести в следующем формате -
             * ID сотрудника и список заказов, с которыми работает данный сотрудник */

            var employeesOrder = from emp in model.Employees
                                 join p in model.Orders
                                 on emp.EmployeeID equals p.EmployeeID
                                 orderby emp.EmployeeID
                                 select new
                                 {
                                     emp = emp.EmployeeID,
                                     order = from p in model.Orders
                                             select p.OrderID
                                 };

            /* Сгруппировать заказы по общей стоимости - дороже 5000$, от 1000$ до 4999$, от 1$ до 999$ */

            var order = from p in model.Order_Details
                        let num = p.UnitPrice
                        group p by Sort(num);



            /*Для каждого заказа вывести названия продуктов в этом заказе */

            var products = from orderF in model.Orders
                           join orderD in model.Order_Details
                           on orderF.OrderID equals orderD.OrderID
                           join prod in model.Products
                           on orderD.ProductID equals prod.ProductID
                           select new
                           {
                               orderF.OrderID,
                               orderD.Products.ProductName
                           };

            /*Для каждого клиента показать список сотрудников, с которыми он уже работал */
            var client = from emp in model.Employees
                         join ord in model.Orders
                         on emp.EmployeeID equals ord.EmployeeID
                         join cust in model.Customers
                         on ord.CustomerID equals cust.CustomerID
                         select new
                         {
                             emp.FirstName,
                             cust.CustomerID
                         };

            /*Для каждого продукта вывести список клиентов, которые хоть раз заказывали этот продукт */

            var product = from prod in model.Products
                          join ordD in model.Order_Details
                          on prod.ProductID equals ordD.ProductID
                          join or in model.Orders
                          on ordD.OrderID equals or.OrderID
                          join cust in model.Customers
                          on or.CustomerID equals cust.CustomerID
                          select new
                          {
                              prod.ProductID,
                              cust.CustomerID
                          };

            /*Показать ТОП-3 наиболее продаваемых продуктов*/
            var unitSum = (from ord in model.Orders
                           join ordD in model.Order_Details
                           on ord.OrderID equals ordD.OrderID
                           join prod in model.Products
                           on ordD.ProductID equals prod.ProductID
                           orderby ord.OrderID descending
                           select prod.ProductID).Take(3);



            /*Показать ТОП - 3 стран, куда поставляется наибольшее количество заказов */

            var country = (from ord in model.Orders
                          group ord by ord.ShipCountry).Take(3);


            foreach (var item in country)
            {
                Console.WriteLine(item);
            }

            Console.ReadLine();
        }
    }
}
