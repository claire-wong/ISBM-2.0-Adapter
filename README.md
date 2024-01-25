# ISBM-2.0-Server-Adapter

This is one of the three-part series of proof-of-concept projects with the primary objective of constructing an interoperable IoT information cluster. The focus is on utilizing non-proprietary Open Industrial Interoperability Ecosystem (OIIE) open standards. Each project within this series explores key facets of building a cohesive and scalable IoT infrastructure, demonstrating the potential of OIIE standards in promoting interoperability in the interconnect world.

![image](/Documents/Images/IoT-Demo.jpg)

Included in this three-part series are ISBM-Publication-Provider and ISBM-Publication-Consumer, hosted in their respective repositories.

### Title

Hello, this is some text to fill in this, [here](#place-2), is a link to the second place.


### Place 2

Place one has the fun times of linking here, but I can also link back [here](#place-1).

### Objectives

To create an ISA-95 Message Service Model (ISBM 2.0) messaging adapter that enables all IoT devices to send and receive messages using the ISBM 2.0 interface. None of the devices need to possess specific knowledge or interface for the proprietary message bus. The measured data in the OAGIS BOD message will be structured in CCOM format. The BOD message will be published in an interoperable manner, both at the transport and message levels, providing all necessary information to interpret the data. In this case, Microsoft Azure Bus is employed to internally deliver device messages, confirming the feasibility of transforming a proprietary messaging system to behave in an interoperable manner.

![image](/Documents/Images/IoT-Demo-ISBM-Server-Adapter.jpg)

### Project Information

#### Version 0.2

A proof-of-concept project implemented only the essential interface and features to support this demo. There is no channel security in this demo, as it is intended only for running on a local computer or devices within a local area network. 

Utilize the Swashbuckle page included in the ISBM 2.0 Adapter to create your own channel: http://yourlocalhost/swagger. A default channel "/services/general/publication" is included in the distributed database for this demo.

#### Implemented ISA-95 ISBM 2.0 RESTful Interface
     
     5.2 Channel Management Service
         5.2.1 Create Channel 
         5.2.4 Delete Channel
         5.2.6 Get Channels
     5.4 Provider Publication Service
         5.4.1 Open Publication Session
         5.4.2 Post Publication
         5.4.4 Close Publication Session  
     5.5 Consumer Publication Service
         5.5.1 Open Subscription Session
         5.5.2 Read Publication
         5.5.3 Remove Publication
         5.5.4 Close Subscription Session
         
#### Tools
     1.  Visual Studio 2022 Community
     2.  Microsoft SQL Server Express 2022
     3.  SQL Server Management Studio
     4.  Microsoft Azure Service Bus
     5.  My brain's limited knowledge lol
     
#### Dependencies
     1.  .Net Framework 4.7.2
     2.  Microsoft.ServiveBus v4.0.30319 @
     3.  Microsoft.Azure.ServiceBus v4.0.30319 @
     4.  NewtonSoft v12.0.3 @
     5.  Swashbukle.Core v4.0.30319 @
 
@ NuGet Packages



### Place's 3: other example

Place one has the fun times of linking here, but I can also link back [here](#places-3-other-example).



### Useful Links

http://www.openoandm.org/files/standards/ISBM-2.0.pdf

https://www.mimosa.org/mimosa-ccom/

https://visualstudio.microsoft.com/downloads/

* I use a $10/month Standard Azure Service Bus subscription.

https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-quickstart-topics-subscriptions-portal

https://www.microsoft.com/en-us/download/details.aspx?id=55994

https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15
