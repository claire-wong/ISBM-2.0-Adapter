# ISBM-2.0-Adapter

Proof of concept using open standards to create a universal data adapter for IoT devices. 

### Objectives

To build a messaging adapter allows all the IoT devices to send and receive messages just using the ISBM 2.0 interface. The standardized measured data in the message will be structured in CCOM format. All devices do not need to know any specific knowledge of the service bus. The measured data will be in a predictable structure with all the necessary information to interpret the data. In this case, I use Microsoft Azure Bus to deliver device messages internally.

### Project Information

#### Version 0.1

#### Tools
     1.  Visual Studio 2019 Community
     2.  Microsoft SQL Server Express 2017
     3.  SQL Server Management Studio
     4.  Microsoft Azure Service Bus
     5.  My brain's limited knowledge lol
     
#### Dependencies
     1.  .Net Framework 4.7.2
     2.  Microsoft.ServiveBus v4.0.30319 *
     3.  Microsoft.Azure.ServiceBus v4.0.30319 *
     4.  NewtonSoft v4.0.30319 *
     5.  Swashbukle.Core v4.0.30319 *
 
* NuGet Packages

### Useful Links

http://www.openoandm.org/files/standards/ISBM-2.0.pdf

https://www.mimosa.org/mimosa-ccom/

https://visualstudio.microsoft.com/downloads/

* I use a $10/month Standard service 

https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-quickstart-topics-subscriptions-portal

https://www.microsoft.com/en-us/download/details.aspx?id=55994

https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15
