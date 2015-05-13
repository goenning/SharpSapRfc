SharpSapRfc
===========

## What is SharpSapRfc?
	
SAP NCo 3 is a library developed by SAP that allows .Net Developers connect to SAP through very easy to use API.
**Sharp SAP RFC** is a top-level library that makes it **even easier** to call remote functions on SAP systems. 
Just read the examples bellow and you will see how powerfull it is.

Instead of this code:

```C#
RfcDestination destination = RfcDestinationManager.GetDestination("TST");
RfcRepository repository = destination.Repository;
IRfcFunction function = repository.CreateFunction("Z_SSRT_SUM");
function.SetValue("i_num1", 2);
function.SetValue("i_num2", 4);
function.Invoke(destination);
int result = function.GetInt("e_result");
```

You can write this:

```C#
using (SapRfcConnection conn = new PlainSapRfcConnection("TST"))
{
    var result = conn.ExecuteFunction("Z_SSRT_SUM", new {
        i_num1 = 2,
        i_num2 = 4
    });

    int result = result.GetOutput<int>("e_result");
}
```

You might be asking: It's almost the same!
Well, yes, it is.

But what about write this:

```C#
using (SapRfcConnection conn = new PlainSapRfcConnection("TST"))
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
But because of this class we can work with strongly-typed parameters!

Check this out.

```C#
using (SapRfcConnection conn = new PlainSapRfcConnection("TST"))
{
    var result = conn.ExecuteFunction("Z_SSRT_GET_ALL_CUSTOMERS");
    var customers = result.GetTable<ZCustomer>("t_customers");
}
```

It`s way easier than a iterating on each row and building the object by yourself.
It also works for input parameters!

## Two options available

SharpSapRfc comes with two flavors: RFC and SOAP. Both options have the same interface, so you can swap between them with a single line of code. ABAP requiriments are the same for both libraries, just create an RFC-enable Function Module and you're good to go, be it with RFC or SOAP.

## Which one should I use?

We generally recommend RFC protocol because it is faster than SOAP. The main advantage of SOAP is that enables you to publish you application outside your LAN and connect to SAP thought a common protocol (HTTP/HTTPS) combined with DMZ and Reverse Proxy, for example. 

## How to swap between Plain RFC and SOAP

All examples on this document are using Plain RFC (class is **PlainSapRfcConnection**). If you want to use SOAP, just change to **SoapSapRfcConnection**. The configuration file is different for each library. Examples are on the bottom of this document.

## How to install

For Plain RFC x86 apps

	PM> Install-Package SharpSapRfc.Plain.x86
	
For Plain RFC x64 apps
	
	PM> Install-Package SharpSapRfc.Plain.x64

For SOAP

	PM> Install-Package SharpSapRfc.Soap


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
using (SapRfcConnection conn = new PlainSapRfcConnection("TST"))
{
    var scarr = conn.ReadTable<AirlineCompany>("SCARR");
    Assert.AreEqual(18, scarr.Count());
}

//Reading two fields with where clause
using (SapRfcConnection conn = new PlainSapRfcConnection("TST"))
{
    var scarr = conn.ReadTable<AirlineCompany>("SCARR", 
        fields: new string[] { "CARRID", "CARRNAME" }, 
        where: new string[] { "CARRID = 'DL'" }
    );
}
```

## Configuration Example

###### Plain RFC

```xml
<configuration>
  <configSections>
    <sectionGroup name="SAP.Middleware.Connector">
      <section name="GeneralSettings" type="SAP.Middleware.Connector.RfcGeneralConfiguration,sapnco" />
      <sectionGroup name="ClientSettings">
        <section name="DestinationConfiguration" type="SAP.Middleware.Connector.RfcDestinationConfiguration, sapnco"/>
      </sectionGroup>
    </sectionGroup>
  </configSections>
  
  <SAP.Middleware.Connector>
    <ClientSettings>
      <DestinationConfiguration>
        <destinations >
          <add NAME="TST" USER="bcuser" PASSWD="sapadmin2" CLIENT="001"
             LANG="EN" ASHOST="sap-vm" SYSNR="00" />
        </destinations>
      </DestinationConfiguration>
    </ClientSettings>
  </SAP.Middleware.Connector>
</configuration>
```


###### SOAP

```xml
<configuration>
  <configSections>
    <section name="sapSoapRfc" type="SharpSapRfc.Soap.Configuration.SapSoapRfcConfigurationSection, SharpSapRfc.Soap" />
  </configSections>

  <sapSoapRfc>
    <destinations>
      <add name="TST-SOAP" 
           rfcUrl="http://sap-vm:8000/sap/bc/soap/rfc"
           wsdlUrl="http://sap-vm:8000/sap/bc/soap/wsdl"
           client="001" 
           user="bcuser" 
           password="sapadmin2"/>
    </destinations>    
  </sapSoapRfc>
</configuration>
```

## All Features

- Mapping for Structures and Tables
- 2 remote fields (DATE and TIME) mapped to a single DateTime property
- Enum Mapping (both numbers and strings)
- Boolean. Use True and False instead of "X" and "
- Shortcut to RFC_READ_TABLE
- Easier RFC calls

Take a look at the **tests** project for more examples.
