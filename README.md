SharpSapRfc
===========

To install Sharp SAP RFC, run the following command in the Package Manager Console

For x86 apps

	PM> Install-Package SharpSapRfc.x86
	
For x64 apps
	
	PM> Install-Package SharpSapRfc.x64
	
SAP NCo 3 has a very easy to use API.
**Sharp SAP RFC** makes it **even easier** to call remote functions on SAP systems.

Instead of this:

```C#
RfcDestination destination = RfcDestinationManager.GetDestination("TST");
RfcRepository repository = destination.Repository;
IRfcFunction function = repository.CreateFunction("Z_SSRT_SUM");
function.SetValue("i_num1", 2);
function.SetValue("i_num2", 4);
function.Invoke(destination);
int result = function.GetInt("e_result");
```

You can write:

```C#
using (SharpSapRfcConnection conn = new SharpSapRfcConnection("TST"))
{
    var result = conn.ExecuteFunction("Z_SSRT_SUM", new {
        i_num1 = 2,
        i_num2 = 4
    });

    int result = result.GetOutput<int>("e_result");
}
```

You might be asking. It's almost the same!
Well, yes, it is.

But what about write this:

```C#
using (SharpSapRfcConnection conn = new SharpSapRfcConnection("TST"))
{
    var customer = new ZCustomer(3, "Some Company", true);
    var result = conn.ExecuteFunction("Z_SSRT_ADD_CUSTOMER", new {
       i_customer = customer
    });
}

public class ZCustomer
{
    public int Id { get; set; }
    public string Name { get; set; }

    [RfcStructureField("ACTIVE")]
    public bool IsActive { get; set; }

	public ZCustomer(int id, string name, bool isActive)
	{
		this.Id = id;
		this.Name = name;
		this.IsActive = isActive;
	}
}
```

Instead of:

```C#
RfcDestination destination = RfcDestinationManager.GetDestination("TST");
RfcRepository repository = destination.Repository;
IRfcFunction function = repository.CreateFunction("Z_SSRT_ADD_CUSTOMER");

IRfcStructure customer = function.GetStructure("i_customer");
customer.SetValue("ID", 3);
customer.SetValue("NAME", "Some Company");
customer.SetValue("ACTIVE", true);

function.SetValue("i_customer", customer);
function.Invoke(destination);
```

That's more code, but it's because of `Customer` class.
But because of this class we can work with tables parameters!

Check this out.

```C#
using (SharpSapRfcConnection conn = new SharpSapRfcConnection("TST"))
{
    var result = conn.ExecuteFunction("Z_SSRT_GET_ALL_CUSTOMERS");
    var customers = result.GetTable<ZCustomer>("t_customers");
}
```

It`s way easier than a iterating on each row and building the object by yourself.
It also works for input parameters!

## RFC Read Table

There's also a shortcut to the RFC_READ_TABLE function.

You can use it like this:

```C#
public class AirlineCompany
{
	[RfcStructureField("MANDT")]
	public int Client { get; set; }
	[RfcStructureField("CARRID")]
	public string Code { get; set; }
	[RfcStructureField("CARRNAME")]
	public string Name { get; set; }
	[RfcStructureField("CURRCODE")]
	public string Currency { get; set; }
	[RfcStructureField("URL")]
	public string Url { get; set; }
}

//Reading all table entries
using (SharpSapRfcConnection conn = new SharpSapRfcConnection("TST"))
{
    var scarr = conn.ReadTable<AirlineCompany>("SCARR");
    Assert.AreEqual(18, scarr.Count());
}

//Reading two fields with where clause
using (SharpSapRfcConnection conn = new SharpSapRfcConnection("TST"))
{
    var scarr = conn.ReadTable<AirlineCompany>("SCARR", 
        fields: new string[] { "CARRID", "CARRNAME" }, 
        where: new string[] { "CARRID = 'DL'" }
    );
}
```
