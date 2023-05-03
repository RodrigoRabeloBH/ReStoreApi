using ReStore.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace ReStore.Domain.Utils
{
    [ExcludeFromCodeCoverage]
    public static class EmailTemplates
    {
        public static string OrderReceived(Order order, User user)
        {
            string items = GetOrderItems(order);

            string template = $@"<div style=""background-color:#f9fbe7; padding: 20px; width: 95%; font-family: 'Courier New', Courier, monospace;"">
                                    <h2>Confirmation Number: [{order.Id}]</h2>
                                    <h3>Hello {user.UserName.ToUpper()}</h3>
                                    <p>We're happy to let you know that we've received your order.</p>                         
                                    <p>Once your package ships, we will send you an email with a tracking number and link so you can see the movement of your package.</p>
                                    <p>If you have any question, contact us here or call us on [5555-4444].</p>
                                    <p>We are here to help!</p>
                                    <div>
                                        <table style=""border:1px solid black; border-collapse: collapse;"">  
                                            <thead>
                                               <tr>
                                                  <th style=""border:1px solid black; border-collapse: collapse; padding: 10px"">Image</th>
                                                  <th style=""border:1px solid black; border-collapse: collapse; padding: 10px"">Name</th>
                                                  <th style=""border:1px solid black; border-collapse: collapse; padding: 10px"">Price</th>
                                                  <th style=""border:1px solid black; border-collapse: collapse; padding: 10px"">Quantity</th>                                            
                                               </tr>
                                            </thead>
                                            <tbody>                                          
                                                 {items}
                                                <tr>
                                                    <td style=""font-weight: bold; padding: 5px;"">Total:</td>
                                                    <td></td>
                                                    <td></td>                                               
                                                    <td style=""font-weight: bold; text-align: center; padding: 5px;""> R${order.GetTotal()}</td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>";

            return template;
        }

        private static string GetOrderItems(Order order)
        {
            string orderItems = string.Empty;

            foreach (var item in order.OrderItems)
            {
                orderItems += @$"<tr>
                                     <td style=""border:1px solid black; border-collapse: collapse; text-align: center;"">
                                        <img src=""https://restoreapi.azurewebsites.net/{item.ItemOrdered.PictureUrl}"" style=""width: 50px;"">
                                     </td>
                                     <td style=""border:1px solid black; border-collapse: collapse; text-align: center;"">{item.ItemOrdered.Name}</td>
                                     <td style=""border:1px solid black; border-collapse: collapse; text-align: center;"">R${item.Price}</td>
                                     <td style=""border:1px solid black; border-collapse: collapse; text-align: center;"">{item.Quantity}</td>                                 
                                </tr>";
            }

            return orderItems;
        }
    }
}
