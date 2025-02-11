namespace CryptoInvestment.Domain.Customers;

public class CustomerQuestion
{
    public int IdCustomerQuestions { get; set; }
    public int IdCustomer { get; set; }
    public int IdQuestion { get; set; }
    public string Response { get; set; }
    public int Order { get; set; }
}

