# ISBM-2.0-Adapter

Proof of concept using open standards to create a universal data adapter for IoT devices. 

### Objectives

To build a messaging adapter allows all the IoT devices to send and receive messages just using ISBM 2.0 interface. The standardized measured data in the message will be structured in CCOM format. All devices do not need to know any specific knowledge of the service bus. The measured data will be in a predictable structure with all the necessary information to interpret the data. In this case, I use Microsoft Azure Bus to deliver device messages internally.

### Useful Links

https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-quickstart-topics-subscriptions-portal

https://www.microsoft.com/en-us/download/details.aspx?id=55994

https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15
