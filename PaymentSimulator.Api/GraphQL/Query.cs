namespace PaymentSimulator.Api.GraphQL;

[QueryType]
public class Query
{
    public string Instructions() => "This is a Payment Simulator API. Use the SubmitPayment mutation to process transactions.";
}